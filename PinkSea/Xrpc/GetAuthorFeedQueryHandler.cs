using PinkSea.AtProto.Server.Xrpc;
using PinkSea.AtProto.Shared.Xrpc;
using PinkSea.Lexicons.Queries;
using PinkSea.Services;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.shinolabs.pinksea.getAuthorFeed" xrpc query. Returns the feed for an author.
/// </summary>
[Xrpc("com.shinolabs.pinksea.getAuthorFeed")]
public class GetAuthorFeedQueryHandler(FeedBuilder feedBuilder)
    : IXrpcQuery<GetAuthorFeedQueryRequest, GenericTimelineQueryResponse>
{
    /// <inheritdoc />
    public async Task<XrpcErrorOr<GenericTimelineQueryResponse>> Handle(GetAuthorFeedQueryRequest request)
    {
        var limit = Math.Clamp(request.Limit, 1, 50);
        var since = request.Since ?? DateTimeOffset.Now.AddMinutes(5);

        var feed = await feedBuilder
            .Where(o => o.ParentId == null && o.AuthorDid == request.Did)
            .Since(since.UtcDateTime)
            .Limit(limit)
            .GetFeed();

        return XrpcErrorOr<GenericTimelineQueryResponse>.Ok(new GenericTimelineQueryResponse
        {
            Oekaki = feed
        });
    }
}