using Microsoft.AspNetCore.Mvc;
using PinkSea.AtProto.Lexicons.AtProto;
using PinkSea.AtProto.Lexicons.Types;
using PinkSea.AtProto.Xrpc.Client;
using PinkSea.Helpers;
using PinkSea.Lexicons.Records;
using PinkSea.Models;

namespace PinkSea.Controllers;

/// <summary>
/// A temporary api controller until we finally get XRPC working :pray:.
/// </summary>
[Route("/api")]
public class ApiController(
    IXrpcClientFactory xrpcClientFactory) : Controller
{
    /// <summary>
    /// Uploads an oekaki.
    /// </summary>
    /// <returns>The oekaki.</returns>
    [Route("upload-oekaki")]
    [HttpPost]
    public async Task<IActionResult> UploadOekaki(
        [FromBody] UploadOekakiRequest request)
    {
        var stateClaim = User.Claims.FirstOrDefault(c => c.Type == "state");
        if (stateClaim is null)
            return Unauthorized();

        var pds = User.Claims.FirstOrDefault(c => c.Type == "pds")!.Value;
        var did = User.Claims.FirstOrDefault(c => c.Type == "did")!.Value;

        using var xrpcClient = await xrpcClientFactory.GetForOAuthStateId(stateClaim.Value);
        if (xrpcClient is null)
            return Unauthorized();
        
        var (mime, bytes) = DataUrlHelper.ParseDataUrl(request.Data);
        
        // We'll only deal with PNG files.
        if (!mime.Equals("image/png;base64", StringComparison.CurrentCultureIgnoreCase))
            return StatusCode(StatusCodes.Status415UnsupportedMediaType);
        
        // As per the lexicon: maxSize=1048576
        if (bytes.Length > 1048576)
            return StatusCode(StatusCodes.Status413PayloadTooLarge);

        var byteArrayContent = new ByteArrayContent(bytes);
        byteArrayContent.Headers.Add("Content-Type", "image/png");
        
        var result = await xrpcClient.RawCall<UploadBlobResponse>(
            "com.atproto.repo.uploadBlob",
            byteArrayContent);

        if (result is null)
            return BadRequest();

        var oekaki = new Oekaki
        {
            CreatedAt = DateTimeOffset.UtcNow
                .ToUnixTimeMilliseconds()
                .ToString(),
            Image = new Image
            {
                Blob = result.Blob,
                ImageLink = new Image.ImageLinkObject
                {
                    // By default we'll put it at the getBlob xrpc call to the pds for decentralization.
                    // PinkSea will be able to retrieve its own version.
                    FullSize = $"{pds}/xrpc/com.atproto.sync.getBlob?did={did}&cid={result.Blob.Reference.Link}",
                    Alt = request.AltText
                }
            },
            Tags = request.Tags?
                .Where(t => t.Length <= 640)
                .ToArray()
        };

        var response = await xrpcClient.Procedure<PutRecordResponse>(
            "com.atproto.repo.putRecord",
            new PutRecordRequest
            {
                Repo = did,
                Collection = "com.shinolabs.pinksea.oekaki",
                RecordKey = Tid.NewTid().ToString(),
                Record = oekaki
            });

        if (response is null)
            return StatusCode(StatusCodes.Status500InternalServerError);
        
        return Ok(response.Uri);
    }
}