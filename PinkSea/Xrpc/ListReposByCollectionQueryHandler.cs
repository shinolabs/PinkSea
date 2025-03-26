using System.Web;
using Microsoft.EntityFrameworkCore;
using PinkSea.AtProto.Server.Xrpc;
using PinkSea.AtProto.Shared.Lexicons.AtProto;
using PinkSea.AtProto.Shared.Xrpc;
using PinkSea.Database;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.atproto.sync.listReposByCollection" XRPC query. This is a stub to allow easy bootstrapping of
/// new PinkSea instances from another PinkSea instance.
/// </summary>
[Xrpc("com.atproto.sync.listReposByCollection")]
public class ListReposByCollectionQueryHandler(PinkSeaDbContext dbContext)
    : IXrpcQuery<GetReposByCollectionRequest, GetReposByCollectionResponse>
{
    /// <inheritdoc />
    public async Task<XrpcErrorOr<GetReposByCollectionResponse>> Handle(GetReposByCollectionRequest request)
    {
        if (request.Collection != "com.shinolabs.pinksea.oekaki")
        {
            return XrpcErrorOr<GetReposByCollectionResponse>.Ok(new GetReposByCollectionResponse
            {
                Repos = []
            });
        }

        var limit = Math.Clamp(request.Limit, 1, 2000);

        var query = dbContext.Users
            .AsNoTracking();
        
        if (DateTimeOffset.TryParse(request.Cursor, out var since))
            query = query.Where(u => u.CreatedAt > since);
        
        var users = await query
            .OrderBy(u => u.CreatedAt)
            .Take(limit)
            .ToListAsync();

        return XrpcErrorOr<GetReposByCollectionResponse>.Ok(new GetReposByCollectionResponse
        {
            Repos = users.Select(user => new GetReposByCollectionResponse.Repo
            {
                Did = user.Did
            }).ToList(),
            
            Cursor = users.Count > 0
                ? HttpUtility.UrlEncode(users[^1].CreatedAt.ToString("o"))
                : null
        });
    }
}