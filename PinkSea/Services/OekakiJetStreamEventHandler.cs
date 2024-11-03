using System.Text.Json;
using PinkSea.AtProto.Streaming.JetStream;
using PinkSea.AtProto.Streaming.JetStream.Events;
using PinkSea.Lexicons.Records;

namespace PinkSea.Services;

/// <summary>
/// The oekaki JetStream event handler.
/// </summary>
public class OekakiJetStreamEventHandler(
    OekakiService oekakiService,
    ILogger<OekakiJetStreamEventHandler> logger) : IJetStreamEventHandler
{
    /// <inheritdoc />
    public async Task HandleEvent(JetStreamEvent @event)
    {
        if (@event.Kind != "commit")
            return;

        var commit = @event.Commit!;
        if (commit.Operation != "create")
            return;

        await ProcessCreatedOekaki(
            commit,
            @event.Did);
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
        
        var oekakiRecord = commit.Record!
            .Value
            .Deserialize<Oekaki>()!;
        
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
}