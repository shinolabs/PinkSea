using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Shared.Lexicons.AtProto;
using PinkSea.AtProto.Xrpc.Client;
using PinkSea.Database;
using PinkSea.Database.Models;
using PinkSea.Extensions;
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
    UserService userService,
    IOptions<AppViewConfig> appViewConfig,
    IXrpcClientFactory xrpcClientFactory,
    IDidResolver didResolver,
    ILogger<FirstTimeRunAssistantService> logger)
{
    /// <summary>
    /// Runs the service.
    /// </summary>
    /// <param name="stoppingToken">The cancellation token.</param>
    public async Task Run(CancellationToken stoppingToken)
    {
        await Migrate();

        var config = await TryGetConfiguration();
        if (config is not null)
        {
            if (config.SynchronizedAccountStates != true)
            {
                await SynchronizeAccountStates();
            }

            if (config.ImportedProfiles != true)
            {
                await ImportProfiles();
            }
            
            return;
        }
        
        logger.LogInformation("Hiya! Detected PinkSea running on a fresh installation, hang on while we set things up for you :)");
        BuildConfiguration();
        await Backfill();
    }

    /// <summary>
    /// Tries to get the configuration.
    /// </summary>
    /// <returns>The configuration if it exists.</returns>
    private async Task<ConfigurationModel?> TryGetConfiguration()
    {
        try
        {
            var config = await dbContext.Configuration.FirstOrDefaultAsync();
            return config;
        }
        catch (NpgsqlException)
        {
            return null;
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
    /// Synchronizes the account states. Used after migrating from an earlier version of PinkSea.
    /// </summary>
    private async Task SynchronizeAccountStates()
    {
        logger.LogInformation(" - Migrating users and checking their activity status...");
        
        // Start fetching all the accounts.
        foreach (var user in await userService.GetAllUsers())
        {
            var did = await didResolver.GetDocumentForDid(user.Did);
            if (did is null)
            {
                logger.LogWarning("Failed to resolve the DID document for {Did}. Cautiously this user as active.",
                    user.Did);

                continue;
            }

            var xrpcClient = await xrpcClientFactory.GetWithoutAuthentication(did.GetPds()!);
            var response = await xrpcClient.Query<GetRepoStatusResponse>(
                "com.atproto.sync.getRepoStatus",
                new GetRepoStatusRequest
                {
                    Did = user.Did
                });

            if (!response.IsSuccess)
            {
                if (response.Error?.Error == "RepoNotFound")
                {
                    logger.LogWarning("Repo not found for did {Did}. Probably removed.", user.Did);
                    await userService.UpdateRepoStatus(user.Did, UserRepoStatus.Deleted);
                }
                
                continue;
            }
            
            // First, update this user's handle.
            var handle = did.GetHandle()!;
            await userService.UpdateHandle(user.Did, handle);

            var repoStatus = response.Value!;
            if (repoStatus.Active)
                continue;

            var userRepoStatus = repoStatus.Status?.ToRepoStatus() ?? UserRepoStatus.Unknown;
            
            logger.LogInformation("User with DID {Did} ({Handle}) has now a status of {Status}",
                user.Did, handle, userRepoStatus);

            await userService.UpdateRepoStatus(user.Did, userRepoStatus);
        }
        
        // Remember that we've already done it.
        await configurationService.EditConfiguration(cfg =>
        {
            cfg.SynchronizedAccountStates = true;
        });
    }

    /// <summary>
    /// Imports PinkSea profiles. Used after migrating from an earlier version of PinkSea.
    /// </summary>
    private async Task ImportProfiles()
    {
        logger.LogInformation(" - Importing profiles for existing users...");
        
        // Start fetching all the accounts.
        foreach (var user in await userService.GetAllUsers())
        {
            await BackfillProfile(user.Did);
        }

        await configurationService.EditConfiguration(cfg =>
        {
            cfg.ImportedProfiles = true;
        });
    }

    /// <summary>
    /// Backfills posts.
    /// </summary>
    private async Task Backfill()
    {
        if (string.IsNullOrEmpty(appViewConfig.Value.BackfillSource))
            return;
        
        logger.LogInformation(" - Backfilling posts...");
        using var xrpcClient = await xrpcClientFactory.GetWithoutAuthentication(
            appViewConfig.Value.BackfillSource);

        // TODO: Use the cursor.
        var repos = await xrpcClient.Query<GetReposByCollectionResponse>(
            "com.atproto.sync.listReposByCollection",
            new GetReposByCollectionRequest
            {
                Collection = "com.shinolabs.pinksea.oekaki",
                Limit = 2000 // The max limit as defined.
            });

        if (!repos.IsSuccess)
        {
            logger.LogError("Failed to backfill posts. {Error}",
                repos.Error);
            return;
        }

        // Store the children to be added after their parents.
        var children = new List<(string, string, string, Oekaki)>();
        foreach (var did in repos.Value!.Repos.Select(r => r.Did))
        {
            try
            {
                children.AddRange(await BackfillForDid(did));
                await BackfillProfile(did);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to backfill posts for DID {Did}. {Error}", did, ex.Message);
            }
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
    /// Backfills a profile for a DID.
    /// </summary>
    /// <param name="did">The DID.</param>
    private async Task BackfillProfile(string did)
    {
        var document = await didResolver.GetDocumentForDid(did);
        if (document is null)
        {
            logger.LogWarning("Couldn't get the DID document for {Did}.", did);
            return;
        }

        using var xrpcClient = await xrpcClientFactory.GetWithoutAuthentication(document.GetPds()!);
        
        var response = await xrpcClient.Query<ListRecordsResponse<Profile>>(
            "com.atproto.repo.listRecords",
            new ListRecordsRequest
            {
                Repo = did,
                Collection = "com.shinolabs.pinksea.profile"
            });
        
        if (!response.IsSuccess)
            return;

        var profile = response.Value!
            .Records
            .FirstOrDefault(r => r.AtUri.Contains("self"));

        if (profile is null)
            return;

        logger.LogInformation("Creating a profile for {Did}", did);
        
        if (!await userService.UserExists(did))
            await userService.Create(did);

        await userService.UpdateProfile(did, profile.Value);
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

            if (!response.IsSuccess)
                break;

            cursor = response.Value!.Cursor;

            foreach (var oekaki in response.Value.Records)
            {
                if (appViewConfig.Value.BackfillSkipDimensionsVerification != true &&
                    !await ValidateRemoteOekakiDimensions(xrpcClient, oekaki.Value, did))
                {
                    continue;
                }
                
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

        if (!response.IsSuccess)
        {
            logger.LogWarning("Failed to fetch blob for {Did}/{Cid}.",
                authorDid,
                record.Image.Blob.Reference.Link);
            return false;
        }

        var data = await response.Value!.Content.ReadAsByteArrayAsync();
        return PngHeaderHelper.ValidateDimensionsForOekaki(data);
    }
}