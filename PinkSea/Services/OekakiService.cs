using Microsoft.EntityFrameworkCore;
using PinkSea.AtProto.Lexicons.AtProto;
using PinkSea.AtProto.Lexicons.Types;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.Providers.Storage;
using PinkSea.AtProto.Xrpc.Client;
using PinkSea.Database;
using PinkSea.Database.Models;
using PinkSea.Helpers;
using PinkSea.Lexicons.Records;
using PinkSea.Models;

namespace PinkSea.Services;

/// <summary>
/// The oekaki processing service.
/// </summary>
public class OekakiService(
    IOAuthStateStorageProvider oauthStateStorageProvider,
    IXrpcClientFactory xrpcClientFactory,
    PinkSeaDbContext dbContext)
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
            return OekakiUploadResult.NotAuthorized;

        using var xrpcClient = await xrpcClientFactory.GetForOAuthStateId(stateId);
        
        var (mime, bytes) = DataUrlHelper.ParseDataUrl(request.Data);
        
        // We'll only deal with PNG files.
        if (!mime.Equals("image/png;base64", StringComparison.CurrentCultureIgnoreCase))
            return OekakiUploadResult.NotAPng;
        
        // As per the lexicon: maxSize=1048576
        if (bytes.Length > 1048576)
            return OekakiUploadResult.UploadTooBig;

        var blob = await UploadOekakiBlobToRepository(
            bytes,
            xrpcClient!);
        
        if (blob is null)
            return OekakiUploadResult.FailedToUploadBlob;

        var tid = Tid.NewTid()
            .ToString();
        
        var oekakiRecord = await PutOekakiInRepository(
            blob,
            request,
            tid,
            oauthState,
            xrpcClient!);
        
        if (oekakiRecord is null)
            return OekakiUploadResult.FailedToUploadRecord;

        await InsertOekakiIntoDatabase(
            oekakiRecord.Value.Item1,
            oekakiRecord.Value.Item2,
            oauthState.Did,
            tid);
        
        return OekakiUploadResult.Ok;
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
    /// <param name="tid">The TID.</param>
    /// <param name="oauthState">The OAuth state for the client.</param>
    /// <param name="xrpcClient">The XRPC client.</param>
    /// <returns>The oekaki record.</returns>
    private async Task<(Oekaki, string)?> PutOekakiInRepository(
        Blob blob,
        UploadOekakiRequest request,
        string tid,
        OAuthState oauthState,
        IXrpcClient xrpcClient)
    {
        var oekaki = new Oekaki
        {
            CreatedAt = DateTimeOffset.UtcNow
                .ToUnixTimeMilliseconds()
                .ToString(),
            
            Image = new Image
            {
                Blob = blob,
                ImageLink = new Image.ImageLinkObject
                {
                    // By default we'll put it at the getBlob xrpc call to the pds for decentralization.
                    // PinkSea will be able to retrieve its own version.
                    FullSize = $"{oauthState.Pds}/xrpc/com.atproto.sync.getBlob?did={oauthState.Did}&cid={blob.Reference.Link}",
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
    /// <param name="oekakiCid">The oekaki CID..</param>
    /// <param name="authorDid">The author's did.</param>
    /// <param name="recordTid">The tid of the record.</param>
    public async Task InsertOekakiIntoDatabase(
        Oekaki record,
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

        var image = new OekakiModel
        {
            Tid = recordTid,
            Author = author,
            AuthorDid = author.Did,
            IndexedAt = DateTimeOffset.UtcNow,
            RecordCid = oekakiCid,
            BlobCid = record.Image.Blob.Reference.Link,
            AltText = record.Image.ImageLink.Alt
        };

        await dbContext.Oekaki.AddAsync(image);
        await dbContext.SaveChangesAsync();
    }
}