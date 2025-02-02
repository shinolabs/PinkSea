using Microsoft.EntityFrameworkCore;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Server.Xrpc;
using PinkSea.Database;
using PinkSea.Lexicons.Objects;
using PinkSea.Lexicons.Queries;
using PinkSea.Services;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.shinolabs.pinksea.getOekaki" xrpc call. Retrieves an oekaki post and its children.
/// </summary>
[Xrpc("com.shinolabs.pinksea.getOekaki")]
public class GetOekakiQueryHandler(PinkSeaDbContext dbContext, FeedBuilder feedBuilder, IDidResolver didResolver)
    : IXrpcQuery<GetOekakiQueryRequest, GetOekakiQueryResponse>
{
    /// <inheritdoc />
    public async Task<GetOekakiQueryResponse?> Handle(GetOekakiQueryRequest request)
    {
        var parent = await dbContext.Oekaki
            .Include(o => o.TagOekakiRelations)
            .FirstOrDefaultAsync(o => o.AuthorDid == request.Did && o.OekakiTid == request.RecordKey);

        if (parent == null)
            return null;

        var childrenFeed = await feedBuilder
            .StartWithOrdering(c => c.IndexedAt)
            .Where(c => c.ParentId == parent.Key)
            .GetFeed();

        return new GetOekakiQueryResponse()
        {
            Parent =
                !parent.Tombstone
            ? HydratedOekaki.FromOekakiModel(
                parent, 
                await didResolver.GetHandleFromDid(parent.AuthorDid) ?? "invalid.handle")
            : TombstoneOekaki.FromOekakiModel(
                parent),
            
            Children = childrenFeed.ToArray()
        };
    }
}