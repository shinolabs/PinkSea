using Microsoft.EntityFrameworkCore;
using PinkSea.AtProto.Server.Xrpc;
using PinkSea.Database;
using PinkSea.Lexicons.Queries;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.shinolabs.pinksea.getParentForReply" query. Gets the parent for a reply.
/// </summary>
[Xrpc("com.shinolabs.pinksea.getParentForReply")]
public class GetParentForReplyQueryHandler(PinkSeaDbContext dbContext)
    : IXrpcQuery<GetParentForReplyQueryRequest, GetParentForReplyQueryResponse>
{
    /// <inheritdoc />
    public async Task<GetParentForReplyQueryResponse?> Handle(GetParentForReplyQueryRequest request)
    {
        var parent = await dbContext.Oekaki
            .AsNoTracking()
            .Include(o => o.Parent)
            .Where(o => o.AuthorDid == request.AuthorDid && o.OekakiTid == request.RecordKey)
            .Select(o => o.Parent)
            .FirstOrDefaultAsync();

        if (parent is null)
            return null;

        return new GetParentForReplyQueryResponse()
        {
            AuthorDid = parent.AuthorDid,
            RecordKey = parent.OekakiTid
        };
    }
}