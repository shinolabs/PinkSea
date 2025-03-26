using PinkSea.AtProto.Server.Xrpc;
using PinkSea.AtProto.Shared.Xrpc;
using PinkSea.Lexicons.Queries;
using PinkSea.Services;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.shinolabs.pinksea.getRecent" query. Gets the most recent posts on the timeline.
/// </summary>
[Xrpc("com.shinolabs.pinksea.getRecent")]
public class GetRecentQueryHandler(
    FeedBuilder feedBuilder) : IXrpcQuery<GenericTimelineQueryRequest, GenericTimelineQueryResponse>
{
    /// <inheritdoc />
    public async Task<XrpcErrorOr<GenericTimelineQueryResponse>> Handle(GenericTimelineQueryRequest request)
    {
        var limit = Math.Clamp(request.Limit, 1, 50);
        var since = request.Since ?? DateTimeOffset.Now.AddMinutes(5);

        var feed = await feedBuilder
            .Where(o => o.ParentId == null)
            .Since(since.UtcDateTime)
            .Limit(limit)
            .GetFeed();

        return XrpcErrorOr<GenericTimelineQueryResponse>.Ok(new GenericTimelineQueryResponse
        {
            Oekaki = feed
        });
    }
}