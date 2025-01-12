using PinkSea.AtProto.Server.Xrpc;
using PinkSea.Extensions;
using PinkSea.Lexicons;
using PinkSea.Lexicons.Procedures;
using PinkSea.Services;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.shinolabs.pinksea.deleteOekaki" xrpc procedure. Deletes an oekaki post.
/// </summary>
[Xrpc("com.shinolabs.pinksea.deleteOekaki")]
public class DeleteOekakiProcedureHandler(
    IHttpContextAccessor contextAccessor,
    OekakiService oekakiService) : IXrpcProcedure<DeleteOekakiProcedureRequest, Empty>
{
    /// <inheritdoc />
    public async Task<Empty?> Handle(DeleteOekakiProcedureRequest request)
    {
        var state = contextAccessor.HttpContext?.GetStateToken();
        if (state is null)
            return null!;

        await oekakiService.ProcessDeletedOekaki(
            request.RecordKey,
            state);
        
        return new Empty();
    }
}