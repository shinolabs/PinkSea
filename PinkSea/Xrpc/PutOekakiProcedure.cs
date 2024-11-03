using PinkSea.AtProto.Server.Xrpc;
using PinkSea.Models;
using PinkSea.Models.Dto;
using PinkSea.Services;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.shinolabs.pinksea.putOekaki" procedure. Uploads an oekaki.
/// </summary>
public class PutOekakiProcedure(
    IHttpContextAccessor contextAccessor,
    OekakiService oekakiService) : IXrpcProcedure<UploadOekakiRequest, string>
{
    /// <inheritdoc />
    public async Task<string> Handle(UploadOekakiRequest request)
    {
        var context = contextAccessor.HttpContext!;
        
        var stateClaim = context.User.Claims.FirstOrDefault(c => c.Type == "state")?.Value;
        if (stateClaim is null)
            return null!;

        return await oekakiService.ProcessUploadedOekaki(request, stateClaim) switch
        {
            OekakiUploadResult.NotAPng => null!,
            OekakiUploadResult.UploadTooBig => null!,
            OekakiUploadResult.FailedToUploadBlob => null!,
            OekakiUploadResult.FailedToUploadRecord => null!,
            _ => "ok" // TODO
        };
    }
}