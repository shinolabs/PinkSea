using PinkSea.AtProto.Server.Xrpc;
using PinkSea.Extensions;
using PinkSea.Models;
using PinkSea.Services;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.shinolabs.pinksea.putOekaki" procedure. Uploads an oekaki.
/// </summary>
[Xrpc("com.shinolabs.pinksea.putOekaki")]
public class PutOekakiProcedure(
    IHttpContextAccessor contextAccessor,
    OekakiService oekakiService) : IXrpcProcedure<UploadOekakiRequest, string>
{
    /// <inheritdoc />
    public async Task<string> Handle(UploadOekakiRequest request)
    {
        var state = contextAccessor.HttpContext?.GetStateToken();
        if (state is null)
            return null!;

        return await oekakiService.ProcessUploadedOekaki(request, state) switch
        {
            OekakiUploadResult.NotAPng => null!,
            OekakiUploadResult.UploadTooBig => null!,
            OekakiUploadResult.FailedToUploadBlob => null!,
            OekakiUploadResult.FailedToUploadRecord => null!,
            _ => "ok" // TODO
        };
    }
}