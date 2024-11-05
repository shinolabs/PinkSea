using Microsoft.EntityFrameworkCore;
using PinkSea.AtProto.Models.Did;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Server.Xrpc;
using PinkSea.Database;
using PinkSea.Lexicons.Queries;
using PinkSea.Models.Dto;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.shinolabs.pinksea.getRecent" query. Gets the most recent posts on the timeline.
/// </summary>
[Xrpc("com.shinolabs.pinksea.getRecent")]
public class GetRecentQueryHandler(
    IDidResolver didResolver,
    PinkSeaDbContext dbContext) : IXrpcQuery<GenericTimelineQueryRequest, GenericTimelineQueryResponse>
{
    /// <inheritdoc />
    public async Task<GenericTimelineQueryResponse?> Handle(GenericTimelineQueryRequest request)
    {
        var oekaki = await dbContext.Oekaki
            .Where(o => o.ParentId == null)
            .Include(o => o.Author)
            .OrderByDescending(o => o.IndexedAt)
            .ToListAsync();

        var dids = oekaki
            .Select(o => o.AuthorDid)
            .Distinct();

        var map = new Dictionary<string, string>();
        foreach (var did in dids)
        {
            var document = await didResolver.GetDidResponseForDid(did);
            map[did] = document!.AlsoKnownAs[0]
                .Replace("at://", "");
        }

        var oekakiDtos = oekaki.Select(o => new OekakiDto
        {
            AuthorDid = o.AuthorDid,
            AuthorHandle = map[o.AuthorDid],
            CreationTime = o.IndexedAt,
            ImageLink =
                $"https://cdn.bsky.app/img/feed_fullsize/plain/{o.AuthorDid}/{o.BlobCid}",
            Tags = [],

            AtProtoLink = $"at://{o.AuthorDid}/com.shinolabs.pinksea.oekaki/{o.OekakiTid}",
            OekakiCid = o.RecordCid
        });

        return new GenericTimelineQueryResponse
        {
            Oekaki = oekakiDtos
        };
    }
}