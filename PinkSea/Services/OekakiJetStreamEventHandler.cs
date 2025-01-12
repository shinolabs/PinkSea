using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Streaming.JetStream;
using PinkSea.AtProto.Streaming.JetStream.Events;
using PinkSea.Helpers;
using PinkSea.Lexicons.Records;

namespace PinkSea.Services;

/// <summary>
/// The oekaki JetStream event handler.
/// </summary>
public class OekakiJetStreamEventHandler(
    OekakiService oekakiService,
    IDidResolver didResolver,
    IHttpClientFactory httpClientFactory,
    ILogger<OekakiJetStreamEventHandler> logger,
    IMemoryCache memoryCache) : IJetStreamEventHandler
{
    /// <inheritdoc />
    public async Task HandleEvent(JetStreamEvent @event)
    {
        if (@event.Kind != "commit")
            return;

        var commit = @event.Commit!;
        if (commit.Operation == "create")
        {
            await ProcessCreatedOekaki(
                commit,
                @event.Did);
        }
        else if (commit.Operation == "delete")
        {
            await ProcessDeletedOekaki(
                commit,
                @event.Did);
        }
    }

    /// <summary>
    /// Processes deleted oekaki.
    /// </summary>
    /// <param name="commit">The commit.</param>
    /// <param name="authorDid">The author's DID.</param>
    private async Task ProcessDeletedOekaki(
        AtProtoCommit commit,
        string authorDid)
    {
        if (!await oekakiService.OekakiRecordExists(authorDid, commit.RecordKey))
        {
            logger.LogInformation($"Received a removal commit for at://{authorDid}/com.shinolabs.pinksea.oekaki/{commit.RecordKey} but we don't have it in the database.");
            return;
        }

        logger.LogInformation($"Received a removal commit for at://{authorDid}/com.shinolabs.pinksea.oekaki/{commit.RecordKey}.");
        await oekakiService.MarkOekakiAsDeleted(
            authorDid,
            commit.RecordKey);
    }

    /// <summary>
    /// Processes created oekaki.
    /// </summary>
    /// <param name="commit">The commit.</param>
    /// <param name="authorDid">The author's DID.</param>
    private async Task ProcessCreatedOekaki(
        AtProtoCommit commit,
        string authorDid)
    {
        if (await oekakiService.OekakiRecordExists(authorDid, commit.RecordKey))
        {
            logger.LogInformation($"Received duplicate oekaki record for at://{authorDid}/com.shinolabs.pinksea.oekaki/{commit.RecordKey}");
            return;
        }

        if (memoryCache.TryGetValue($"lock:{commit.Cid}", out _))
        {
            logger.LogInformation($"Raced JetStream to insert an oekaki record at://{authorDid}/com.shinolabs.pinksea.oekaki/{commit.RecordKey}");
            return;
        }
        
        var oekakiRecord = commit.Record!
            .Value
            .Deserialize<Oekaki>()!;

        if (!await ValidateRemoteOekakiDimensions(oekakiRecord, authorDid))
        {
            logger.LogInformation($"Received oekaki exceeding dimension limits for at://{authorDid}/com.shinolabs.pinksea.oekaki/{commit.RecordKey}");
            return;
        }
        
        // Try to get the parent.
        var parent = oekakiRecord.InResponseTo is not null
            ? await oekakiService.GetParentForPost(oekakiRecord.InResponseTo.Uri)
            : null;

        await oekakiService.InsertOekakiIntoDatabase(
            oekakiRecord,
            parent,
            commit.Cid!,
            authorDid,
            commit.RecordKey);
        
        logger.LogInformation($"Indexed new oekaki record: at://{authorDid}/com.shinolabs.pinksea.oekaki/{commit.RecordKey}");
    }

    /// <summary>
    /// Validates the remote oekaki dimensions.
    /// </summary>
    /// <param name="record">The record.</param>
    /// <param name="authorDid">The DID of the author.</param>
    /// <returns>Whether they fit within the dimensions.</returns>
    private async Task<bool> ValidateRemoteOekakiDimensions(
        Oekaki record,
        string authorDid)
    {
        // Get the PDS of the author.
        var authorDidResponse = await didResolver.GetDocumentForDid(authorDid);
        if (authorDidResponse is null)
            return false;

        var pds = authorDidResponse.GetPds()!;
        using var client = httpClientFactory.CreateClient();
        var response =
            await client.GetAsync(
                $"{pds}/xrpc/com.atproto.sync.getBlob?did={authorDid}&cid={record.Image.Blob.Reference.Link}");

        if (!response.IsSuccessStatusCode)
            return false;

        var data = await response.Content.ReadAsByteArrayAsync();
        return PngHeaderHelper.ValidateDimensionsForOekaki(data);
    }
}