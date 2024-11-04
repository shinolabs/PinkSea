using Microsoft.EntityFrameworkCore;
using PinkSea.AtProto.Server.Xrpc;
using PinkSea.Database;
using PinkSea.Lexicons.Queries;
using PinkSea.Models.Dto;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.shinolabs.pinksea.getOekaki" xrpc call. Retrieves an oekaki post and its children.
/// </summary>
[Xrpc("com.shinolabs.pinksea.getOekaki")]
public class GetOekakiQueryHandler(PinkSeaDbContext dbContext)
    : IXrpcQuery<GetOekakiQueryRequest, GetOekakiQueryResponse>
{
    /// <inheritdoc />
    public async Task<GetOekakiQueryResponse?> Handle(GetOekakiQueryRequest request)
    {
        var parent = await dbContext.Oekaki
            .FirstOrDefaultAsync(o => o.AuthorDid == request.Did && o.OekakiTid == request.RecordKey);

        if (parent == null)
            return null;

        var children = await dbContext.Oekaki
            .Where(o => o.ParentId == parent.Key)
            .OrderByDescending(o => o.IndexedAt)
            .ToListAsync();

        return new GetOekakiQueryResponse()
        {
            Parent = OekakiDto.FromOekakiModel(parent, ""),
            Children = children.Select(
                    c => OekakiDto.FromOekakiModel(c, ""))
                .ToArray()
        };
    }
}