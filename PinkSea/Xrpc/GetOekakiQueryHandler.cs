using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Server.Xrpc;
using PinkSea.AtProto.Shared.Xrpc;
using PinkSea.Database;
using PinkSea.Lexicons.Objects;
using PinkSea.Lexicons.Queries;
using PinkSea.Models;
using PinkSea.Services;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.shinolabs.pinksea.getOekaki" xrpc call. Retrieves an oekaki post and its children.
/// </summary>
[Xrpc("com.shinolabs.pinksea.getOekaki")]
public class GetOekakiQueryHandler(
    PinkSeaDbContext dbContext,
    FeedBuilder feedBuilder,
    UserService userService,
    IOptions<AppViewConfig> opts)
    : IXrpcQuery<GetOekakiQueryRequest, GetOekakiQueryResponse>
{
    /// <inheritdoc />
    public async Task<XrpcErrorOr<GetOekakiQueryResponse>> Handle(GetOekakiQueryRequest request)
    {
        var parent = await dbContext.Oekaki
            .Include(o => o.TagOekakiRelations)
            .Include(o => o.Author)
            .ThenInclude(u => u.Avatar)
            .FirstOrDefaultAsync(o => o.AuthorDid == request.Did && o.OekakiTid == request.RecordKey);

        if (parent == null)
            return XrpcErrorOr<GetOekakiQueryResponse>.Fail("NotFound", "Could not find this record.");
        
        var childrenFeed = await feedBuilder
            .StartWithOrdering(c => c.IndexedAt)
            .Where(c => c.ParentId == parent.Key)
            .GetFeed();

        return XrpcErrorOr<GetOekakiQueryResponse>.Ok(new GetOekakiQueryResponse()
        {
            Parent =
                !parent.Tombstone
            ? HydratedOekaki.FromOekakiModel(
                parent, 
                parent.Author,
                opts.Value.ImageProxyTemplate)
            : TombstoneOekaki.FromOekakiModel(
                parent),
            
            Children = childrenFeed.ToArray()
        });
    }
}