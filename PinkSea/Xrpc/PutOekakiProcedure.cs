using PinkSea.AtProto.Server.Xrpc;
using PinkSea.Extensions;
using PinkSea.Lexicons.Procedures;
using PinkSea.Models;
using PinkSea.Models.Oekaki;
using PinkSea.Services;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.shinolabs.pinksea.putOekaki" procedure. Uploads an oekaki.
/// </summary>
[Xrpc("com.shinolabs.pinksea.putOekaki")]
public class PutOekakiProcedure(
    IHttpContextAccessor contextAccessor,
    OekakiService oekakiService) : IXrpcProcedure<UploadOekakiRequest, PutOekakiProcedureResponse>
{
    /// <inheritdoc />
    public async Task<PutOekakiProcedureResponse?> Handle(UploadOekakiRequest request)
    {
        var state = contextAccessor.HttpContext?.GetStateToken();
        if (state is null)
            return null!;

        var result = await oekakiService.ProcessUploadedOekaki(request, state);
        
        return result.State switch
        {
            OekakiUploadState.NotAPng => null!,
            OekakiUploadState.UploadTooBig => null!,
            OekakiUploadState.FailedToUploadBlob => null!,
            OekakiUploadState.FailedToUploadRecord => null!,
            _ => new PutOekakiProcedureResponse
            {
                AtLink = $"at://{result.Oekaki!.AuthorDid}/com.shinolabs.pinksea.oekaki/{result.Oekaki.OekakiTid}",
                RecordKey = result.Oekaki.OekakiTid
            }
        };
    }
}