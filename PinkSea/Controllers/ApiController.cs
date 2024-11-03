using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PinkSea.AtProto.Models.Did;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.Database;
using PinkSea.Models;
using PinkSea.Models.Dto;
using PinkSea.Services;

namespace PinkSea.Controllers;

/// <summary>
/// A temporary api controller until we finally get XRPC working :pray:.
/// </summary>
[Route("/api")]
public class ApiController(
    OekakiService oekakiService,
    PinkSeaDbContext dbContext,
    IDidResolver didResolver) : Controller
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
        var stateClaim = User.Claims.FirstOrDefault(c => c.Type == "state")?.Value;
        if (stateClaim is null)
            return Unauthorized();

        return await oekakiService.ProcessUploadedOekaki(request, stateClaim) switch
        {
            OekakiUploadResult.NotAPng => BadRequest("The uploaded file is not a PNG file."),
            OekakiUploadResult.UploadTooBig => BadRequest("The uploaded file exceeds the size."),
            OekakiUploadResult.FailedToUploadBlob => BadRequest("Failed to upload the blob to the PDS"),
            OekakiUploadResult.FailedToUploadRecord => BadRequest("Failed to upload the record to the PDS"),
            _ => Accepted()
        };
    }

    /// <summary>
    /// Gets all the oekaki.
    /// </summary>
    /// <returns>The oekaki enumerable</returns>
    [Route("get-all")]
    [HttpGet]
    public async Task<IEnumerable<OekakiDto>> GetAll()
    {
        var oekaki = await dbContext.Oekaki
            .Where(o => o.ParentId == null)
            .Include(o => o.Author)
            .OrderByDescending(o => o.IndexedAt)
            .ToListAsync();

        var dids = oekaki.Select(o => o.AuthorDid)
            .Distinct();

        var map = new Dictionary<string, DidResponse>();
        foreach (var did in dids)
        {
            var document = await didResolver.GetDidResponseForDid(did);
            map[did] = document!;
        }

        return oekaki.Select(o =>
        {
            var handle = map[o.AuthorDid].AlsoKnownAs[0]
                .Replace("at://", "");

            var pds = map[o.AuthorDid].GetPds()!;
            
            return new OekakiDto
            {
                AuthorDid = o.AuthorDid,
                AuthorHandle = handle,
                CreationTime = o.IndexedAt,
                ImageLink =
                    $"{pds}/xrpc/com.atproto.sync.getBlob?did={o.AuthorDid}&cid={o.BlobCid}",
                Tags = [],

                AtProtoLink = $"at://{handle}/com.shinolabs.pinksea.oekaki/{o.OekakiTid}",
                OekakiCid = o.RecordCid
            };
        });
    }
}