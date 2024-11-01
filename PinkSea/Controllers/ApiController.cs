using Microsoft.AspNetCore.Mvc;
using PinkSea.AtProto.Lexicons.AtProto;
using PinkSea.AtProto.Providers.Storage;
using PinkSea.AtProto.Xrpc.Client;
using PinkSea.Helpers;
using PinkSea.Models;

namespace PinkSea.Controllers;

/// <summary>
/// A temporary api controller until we finally get XRPC working :pray:.
/// </summary>
[Route("/api")]
public class ApiController(
    IOAuthStateStorageProvider oauthStateStorageProvider,
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

        using var xrpcClient = await xrpcClientFactory.GetForOAuthStateId(stateClaim.Value);
        if (xrpcClient is null)
            return Unauthorized();
        
        var (mime, bytes) = DataUrlHelper.ParseDataUrl(request.Data);
        
        // We'll only deal with PNG files.
        if (!mime.Equals("image/png", StringComparison.CurrentCultureIgnoreCase))
            return StatusCode(StatusCodes.Status415UnsupportedMediaType);
        
        // As per the lexicon: maxSize=1000000
        if (bytes.Length > 1048576)
            return StatusCode(StatusCodes.Status413PayloadTooLarge);
        
        var result = await xrpcClient.RawCall<UploadBlobResponse>(
            "com.atproto.repo.uploadBlob",
            new ByteArrayContent(bytes));

        if (result is null)
        {
            Console.WriteLine("Result is null.");
            return BadRequest();
        }
        
        return Ok(result.Blob);
    }
}