using PinkSea.AtProto.Server.Xrpc;
using PinkSea.AtProto.Shared.Xrpc;
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
    public async Task<XrpcErrorOr<PutOekakiProcedureResponse>> Handle(UploadOekakiRequest request)
    {
        var state = contextAccessor.HttpContext?.GetStateToken();
        if (state is null)
            return XrpcErrorOr<PutOekakiProcedureResponse>.Fail("NoAuthToken", "Missing authorization token.");

        var result = await oekakiService.ProcessUploadedOekaki(request, state);
        
        return result.State switch
        {
            OekakiUploadState.NotAuthorized => XrpcErrorOr<PutOekakiProcedureResponse>.Fail(
                result.State.ToString(),
                "This user was not authorized to upload this oekaki."),
            OekakiUploadState.NotAPng => XrpcErrorOr<PutOekakiProcedureResponse>.Fail(
                result.State.ToString(),
                "This oekaki was not a PNG file."),
            OekakiUploadState.UploadTooBig => XrpcErrorOr<PutOekakiProcedureResponse>.Fail(
                result.State.ToString(),
                "This oekaki exceedes the size limit imposed by the lexicon."),
            OekakiUploadState.FailedToUploadBlob => XrpcErrorOr<PutOekakiProcedureResponse>.Fail(
                result.State.ToString(),
                "Failed to upload the blob to the user's PDS."),
            OekakiUploadState.FailedToUploadRecord => XrpcErrorOr<PutOekakiProcedureResponse>.Fail(
                result.State.ToString(),
                "Failed to upload the record to the user's PDS."),
            OekakiUploadState.ExceedsDimensions => XrpcErrorOr<PutOekakiProcedureResponse>.Fail(
                result.State.ToString(),
                "This oekaki exceeds the dimension limit imposed by the AppView."),
            _ => XrpcErrorOr<PutOekakiProcedureResponse>.Ok(new PutOekakiProcedureResponse
            {
                AtLink = $"at://{result.Oekaki!.AuthorDid}/com.shinolabs.pinksea.oekaki/{result.Oekaki.OekakiTid}",
                RecordKey = result.Oekaki.OekakiTid
            })
        };
    }
}