using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PinkSea.AtProto.Lexicons.AtProto;
using PinkSea.AtProto.Lexicons.Types;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.Providers.Storage;
using PinkSea.AtProto.Resolvers.Domain;
using PinkSea.AtProto.Xrpc.Client;
using PinkSea.Database;
using PinkSea.Database.Models;
using PinkSea.Extensions;
using PinkSea.Helpers;
using PinkSea.Lexicons.Records;
using PinkSea.Models;
using PinkSea.Models.Oekaki;
using PinkSea.Services.Integration;

namespace PinkSea.Services;

/// <summary>
/// The oekaki processing service.
/// </summary>
public partial class OekakiService(
    BlueskyIntegrationService blueskyIntegrationService,
    IOAuthStateStorageProvider oauthStateStorageProvider,
    IXrpcClientFactory xrpcClientFactory,
    IDomainDidResolver didResolver,
    PinkSeaDbContext dbContext,
    TagsService tagsService,
    IMemoryCache memoryCache)
{
    /// <summary>
    /// Processes the uploaded oekaki.
    /// </summary>
    /// <param name="request">The upload request.</param>
    /// <param name="stateId">The state id of the user.</param>
    public async Task<OekakiUploadResult> ProcessUploadedOekaki(
        UploadOekakiRequest request,
        string stateId)
    {
        var oauthState = await oauthStateStorageProvider.GetForStateId(stateId);
        if (oauthState is null)
            return new OekakiUploadResult(OekakiUploadState.NotAuthorized);

        using var xrpcClient = await xrpcClientFactory.GetForOAuthStateId(stateId);
        
        var (mime, bytes) = DataUrlHelper.ParseDataUrl(request.Data);
        
        // We'll only deal with PNG files.
        if (!mime.Equals("image/png;base64", StringComparison.CurrentCultureIgnoreCase))
            return new OekakiUploadResult(OekakiUploadState.NotAPng);
        
        // As per the lexicon: maxSize=1048576
        if (bytes.Length > 1048576)
            return new OekakiUploadResult(OekakiUploadState.UploadTooBig);
        
        // A maximum of 800x800.
        if (!PngHeaderHelper.ValidateDimensionsForOekaki(bytes))
            return new OekakiUploadResult(OekakiUploadState.ExceedsDimensions);

        var parent = request.ParentAtUrl is not null
            ? await GetParentForPost(request.ParentAtUrl)
            : null;

        var blob = await UploadOekakiBlobToRepository(
            bytes,
            xrpcClient!);
        
        if (blob is null)
            return new OekakiUploadResult(OekakiUploadState.FailedToUploadBlob);

        var tid = Tid.NewTid()
            .ToString();
        
        var oekakiRecord = await PutOekakiInRepository(
            blob,
            request,
            parent,
            tid,
            oauthState,
            xrpcClient!);
        
        if (oekakiRecord is null)
            return new OekakiUploadResult(OekakiUploadState.FailedToUploadRecord);

        var lockKey = $"lock:{oekakiRecord.Value.Item2}";
        memoryCache.Set(lockKey, true);
        
        var model = await InsertOekakiIntoDatabase(
            oekakiRecord.Value.Item1,
            parent,
            oekakiRecord.Value.Item2,
            oauthState.Did,
            tid);
        
        memoryCache.Remove(lockKey);

        if (request.BlueskyCrosspost == true)
        {
            var (width, height) = PngHeaderHelper.GetPngDimensions(bytes);
            
            await blueskyIntegrationService.CrosspostToBluesky(
                oekakiRecord.Value.Item1,
                stateId, 
                tid,
                width,
                height);
        }
        
        return new OekakiUploadResult(OekakiUploadState.Ok, model);
    }

    /// <summary>
    /// Uploads the oekaki to the repository.
    /// </summary>
    /// <param name="blob">The blob.</param>
    /// <param name="xrpcClient">The XRPC client.</param>
    /// <returns>The blob, if it succeeded.</returns>
    private async Task<Blob?> UploadOekakiBlobToRepository(
        byte[] blob,
        IXrpcClient xrpcClient)
    {
        var byteArrayContent = new ByteArrayContent(blob);
        byteArrayContent.Headers.Add("Content-Type", "image/png");
        
        var result = await xrpcClient.RawCall<UploadBlobResponse>(
            "com.atproto.repo.uploadBlob",
            byteArrayContent);

        return result?.Blob;
    }

    /// <summary>
    /// Uploads the oekaki record to the repository.
    /// </summary>
    /// <param name="blob">The image blob.</param>
    /// <param name="request">The upload request.</param>
    /// <param name="parent">The parent.</param>
    /// <param name="tid">The TID.</param>
    /// <param name="oauthState">The OAuth state for the client.</param>
    /// <param name="xrpcClient">The XRPC client.</param>
    /// <returns>The oekaki record.</returns>
    private async Task<(Oekaki, string)?> PutOekakiInRepository(
        Blob blob,
        UploadOekakiRequest request,
        OekakiModel? parent,
        string tid,
        OAuthState oauthState,
        IXrpcClient xrpcClient)
    {
        var inResponseTo = parent is not null
            ? new StrongRef
            {
                Uri = $"at://{parent.AuthorDid}/com.shinolabs.pinksea.oekaki/{parent.OekakiTid}",
                Cid = parent.RecordCid
            }
            : null;
        
        var oekaki = new Oekaki
        {
            CreatedAt = DateTimeOffset.UtcNow
                .ToString("o"),
            
            Image = new Image
            {
                Blob = blob,
                ImageLink = new Image.ImageLinkObject
                {
                    // TODO: Each PinkSea "instance" should probably proxy the images.
                    FullSize = $"https://cdn.bsky.app/img/feed_fullsize/plain/{oauthState.Did}/{blob.Reference.Link}",
                    Alt = request.AltText
                }
            },
            
            Tags = request.Tags?
                .Where(t => t.Length <= 640)
                .Select(t => t.ToNormalizedTag())
                .Take(10)
                .ToArray(),
            
            InResponseTo = inResponseTo,
            Nsfw = request.Nsfw ?? false
        };

        var response = await xrpcClient.Procedure<PutRecordResponse>(
            "com.atproto.repo.putRecord",
            new PutRecordRequest
            {
                Repo = oauthState.Did,
                Collection = "com.shinolabs.pinksea.oekaki",
                RecordKey = tid,
                Record = oekaki
            });

        return response is not null 
            ? (oekaki, response.Cid)
            : null;
    }

    /// <summary>
    /// Inserts the oekaki record into the local database
    /// </summary>
    /// <param name="record">The record.</param>
    /// <param name="parent">The parent of this oekaki.</param>
    /// <param name="oekakiCid">The oekaki CID..</param>
    /// <param name="authorDid">The author's did.</param>
    /// <param name="recordTid">The tid of the record.</param>
    public async Task<OekakiModel> InsertOekakiIntoDatabase(
        Oekaki record,
        OekakiModel? parent,
        string oekakiCid,
        string authorDid,
        string recordTid)
    {
        // First, see if the author exists.
        var author = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Did == authorDid);

        if (author is null)
        {
            author = new UserModel
            {
                Did = authorDid,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await dbContext.Users.AddAsync(author);
            await dbContext.SaveChangesAsync();
        }

        var model = new OekakiModel
        {
            // We want the key to be separate from the oekaki TID, as we might collide between PDSes.
            Key = Tid.NewTid().ToString(),
            
            OekakiTid = recordTid,
            Author = author,
            AuthorDid = author.Did,
            IndexedAt = DateTimeOffset.UtcNow,
            RecordCid = oekakiCid,
            BlobCid = record.Image.Blob.Reference.Link,
            AltText = record.Image.ImageLink.Alt,
            
            Parent = parent,
            ParentId = parent?.OekakiTid,
            
            IsNsfw = record.Nsfw ?? false,
            Tombstone = false
        };

        await dbContext.Oekaki.AddAsync(model);
        await dbContext.SaveChangesAsync();

        if (record.Tags is not null)
            await tagsService.CreateRelationBetweenOekakiAndTags(model, record.Tags);

        return model;
    }

    /// <summary>
    /// Gets the parent for an AT url.
    /// </summary>
    /// <param name="atUrl">The url.</param>
    /// <returns>The parent.</returns>
    public async Task<OekakiModel?> GetParentForPost(
        string atUrl)
    {
        var atRegex = AtUrlRegex()
            .Match(atUrl);

        var domain = atRegex.Groups["domain"].Value;
        if (!domain.StartsWith("did"))
            domain = await didResolver.GetDidForDomainHandle(domain);

        var id = atRegex.Groups["tid"].Value;

        var parent = await dbContext.Oekaki
            .Where(o => o.AuthorDid == domain && o.OekakiTid == id)
            .FirstOrDefaultAsync();

        if (parent is null)
            return null;

        // Children cannot reply to each other. 
        return parent.ParentId is not null
            ? null
            : parent;
    }

    /// <summary>
    /// Checks if an oekaki record already exists in the DB.
    /// </summary>
    /// <returns>Whether it exists.</returns>
    public async Task<bool> OekakiRecordExists(
        string authorDid,
        string oekakiTid)
    {
        return await dbContext
            .Oekaki
            .AnyAsync(o => o.AuthorDid == authorDid && o.OekakiTid == oekakiTid);
    }

    /// <summary>
    /// Marks an oekaki as deleted.
    /// </summary>
    /// <param name="authorDid">The author's did.</param>
    /// <param name="oekakiRid">The ID of the oekaki.</param>
    public async Task MarkOekakiAsDeleted(
        string authorDid,
        string oekakiRid)
    {
        var oekakiObject = await dbContext
            .Oekaki
            .FirstOrDefaultAsync(o => o.AuthorDid == authorDid && o.OekakiTid == oekakiRid);

        if (oekakiObject is null)
            return;
        
        // First, check if we can remove it outright. This will be for objects that either are a reply
        // or have no children of their own.
        var canBeHardRemoved = !string.IsNullOrEmpty(oekakiObject.ParentId);
        if (!canBeHardRemoved)
        {
            var hasChildren = await dbContext
                .Oekaki
                .AnyAsync(o => o.ParentId == oekakiObject.Key);

            canBeHardRemoved = !hasChildren;
        }

        if (canBeHardRemoved)
        {
            // If we can hard remove it, just remove it.
            dbContext.Oekaki.Remove(oekakiObject);
        }
        else
        {
            // Otherwise, scrap all the data and mark it as a tombstone.
            oekakiObject.AltText = "";
            oekakiObject.BlobCid = "";
            oekakiObject.RecordCid = "";
            oekakiObject.Tombstone = true;
            dbContext.Oekaki.Update(oekakiObject);
        }

        await dbContext.SaveChangesAsync();
    }
    
    /// <summary>
    /// A regex for the at://(handle)/com.shinolabs.pinksea.oekaki/(id) scheme.
    /// </summary>
    /// <returns>The regex matches.</returns>
    [GeneratedRegex("at:\\/\\/(?<domain>[^\\/]+)\\/com\\.shinolabs\\.pinksea\\.oekaki\\/(?<tid>[^\\/]+)")]
    private static partial Regex AtUrlRegex();
}