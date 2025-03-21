using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using PinkSea.AtProto.Lexicons.AtProto;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Xrpc.Client;
using PinkSea.Database;
using PinkSea.Helpers;
using PinkSea.Lexicons.Records;
using PinkSea.Models;

namespace PinkSea.Services.Hosting;

/// <summary>
/// The service responsible for managing the first time run QoL for PinkSea.
/// </summary>
public class FirstTimeRunAssistantService(
    PinkSeaDbContext dbContext,
    OekakiService oekakiService,
    ConfigurationService configurationService,
    IOptions<AppViewConfig> appViewConfig,
    IXrpcClientFactory xrpcClientFactory,
    IDidResolver didResolver,
    ILogger<FirstTimeRunAssistantService> logger) : BackgroundService
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Migrate();
        
        if (await HasPerformedFirstTimeRun())
            return;
        
        logger.LogInformation("Hiya! Detected PinkSea running on a fresh installation, hang on while we set things up for you :)");
        BuildConfiguration();
        await Backfill();
    }

    /// <summary>
    /// Check if we have performed a first time run.
    /// </summary>
    /// <returns>Whether we have performed a first time run.</returns>
    private async Task<bool> HasPerformedFirstTimeRun()
    {
        try
        {
            var config = await dbContext.Configuration.FirstOrDefaultAsync();
            return config != null;
        }
        catch (NpgsqlException exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Builds the configuration.
    /// </summary>
    private void BuildConfiguration()
    {
        logger.LogInformation(" - Building configuration...");
        var config = configurationService.Configuration;
        logger.LogInformation(" - We now use this public key: {PublicKey}", config.ClientPublicKey);
    }

    /// <summary>
    /// Migrates the database.
    /// </summary>
    private async Task Migrate()
    {
        logger.LogInformation(" - Performing migrations...");
        await dbContext.Database.MigrateAsync();
    }

    /// <summary>
    /// Backfills posts.
    /// </summary>
    private async Task Backfill()
    {
        if (string.IsNullOrEmpty(appViewConfig.Value.BackfillRelay))
            return;
        
        logger.LogInformation(" - Backfilling posts...");
        using var xrpcClient = await xrpcClientFactory.GetWithoutAuthentication(
            appViewConfig.Value.BackfillRelay);

        // TODO: Use the cursor.
        var repos = await xrpcClient.Query<GetReposByCollectionResponse>(
            "com.atproto.sync.listReposByCollection",
            new GetReposByCollectionRequest
            {
                Collection = "com.shinolabs.pinksea.oekaki",
                Limit = 2000 // The max limit as defined.
            });

        if (repos is null)
        {
            logger.LogError("Failed to backfill posts. Got no repos from the relay.");
            return;
        }

        // Store the children to be added after their parents.
        var children = new List<(string, string, string, Oekaki)>();
        foreach (var did in repos.Repos.Select(r => r.Did))
        {
            children.AddRange(await BackfillForDid(did));
        }

        // Now add in the children.
        foreach (var (did, at, cid, oekaki) in children)
        {
            await InsertOekakiIntoDatabase(
                did,
                at,
                cid,
                oekaki);
        }
        
        logger.LogInformation(" - Backfilling complete!");
    }

    /// <summary>
    /// Backfills for a single did.
    /// </summary>
    /// <param name="did">The DID of the repo.</param>
    /// <returns>The children this repo has made.</returns>
    private async Task<IList<(string, string, string, Oekaki)>> BackfillForDid(string did)
    {
        logger.LogInformation(" - Backfilling posts for {Did}", did);
        var document = await didResolver.GetDocumentForDid(did);
        if (document is null)
        {
            logger.LogWarning("Couldn't get the DID document for {Did}.", did);
            return [];
        }

        using var xrpcClient = await xrpcClientFactory.GetWithoutAuthentication(document.GetPds()!);

        var children = new List<(string, string, string, Oekaki)>();
        
        // Get the records.
        string? cursor = null;
        do
        {
            var response = await xrpcClient.Query<ListRecordsResponse<Oekaki>>(
                "com.atproto.repo.listRecords",
                new ListRecordsRequest
                {
                    Repo = did,
                    Collection = "com.shinolabs.pinksea.oekaki",
                    Cursor = cursor
                });

            if (response is null)
                break;

            cursor = response.Cursor;

            foreach (var oekaki in response.Records)
            {
                if (!await ValidateRemoteOekakiDimensions(xrpcClient, oekaki.Value, did))
                    continue;
                
                if (oekaki.Value.InResponseTo != null)
                {
                    children.Add((did, oekaki.AtUri, oekaki.Cid, oekaki.Value));
                    continue;
                }

                await InsertOekakiIntoDatabase(did, oekaki.AtUri, oekaki.Cid, oekaki.Value);
            }
            
        } while (cursor != null);
        return children;
    }

    /// <summary>
    /// Inserts an oekaki into the database.
    /// </summary>
    /// <param name="authorDid">The author DID</param>
    /// <param name="atUri">The AT protocol URI.</param>
    /// <param name="cid">The CID of the oekaki.</param>
    /// <param name="oekaki">The oekaki model.</param>
    private async Task InsertOekakiIntoDatabase(
        string authorDid,
        string atUri,
        string cid,
        Oekaki oekaki)
    {
        // Get the record key.
        var recordKey = atUri.Split('/')[^1];

        if (await oekakiService.OekakiRecordExists(authorDid, recordKey))
            return;
        
        // Try to get the parent.
        var parent = oekaki.InResponseTo is not null
            ? await oekakiService.GetParentForPost(oekaki.InResponseTo.Uri)
            : null;

        await oekakiService.InsertOekakiIntoDatabase(
            oekaki,
            parent,
            cid,
            authorDid,
            recordKey,
            useRecordIndexedAt: true);
    }
    
    /// <summary>
    /// Validates the remote oekaki dimensions.
    /// </summary>
    /// <param name="repoXrpcClient">The repo xrpc client.</param>
    /// <param name="record">The record.</param>
    /// <param name="authorDid">The DID of the author.</param>
    /// <returns>Whether they fit within the dimensions.</returns>
    private async Task<bool> ValidateRemoteOekakiDimensions(
        IXrpcClient repoXrpcClient,
        Oekaki record,
        string authorDid)
    {
        var response =
            await repoXrpcClient.Query<HttpResponseMessage>(
                "com.atproto.sync.getBlob",
                new
                {
                    did = authorDid,
                    cid = record.Image.Blob.Reference.Link
                });

        if (!response!.IsSuccessStatusCode)
            return false;

        var data = await response.Content.ReadAsByteArrayAsync();
        return PngHeaderHelper.ValidateDimensionsForOekaki(data);
    }
}