using PinkSea.Database.Models;

namespace PinkSea.Models.Dto;

public class OekakiDto
{
    /// <summary>
    /// The oekaki record key.
    /// </summary>
    public required string OekakiRecordKey { get; set; }
    
    /// <summary>
    /// The DID of the author.
    /// </summary>
    public required string AuthorDid { get; set; }
    
    /// <summary>
    /// The handle of the author.
    /// </summary>
    public required string AuthorHandle { get; set; }
    
    /// <summary>
    /// The image link.
    /// </summary>
    public required string ImageLink { get; set; }
    
    /// <summary>
    /// The AT protocol link.
    /// </summary>
    public required string AtProtoLink { get; set; }
    
    /// <summary>
    /// The oekaki CID.
    /// </summary>
    public required string OekakiCid { get; set; }
    
    /// <summary>
    /// The creation time.
    /// </summary>
    public required DateTimeOffset CreationTime { get; set; }
    
    /// <summary>
    /// Is this oekaki NSFW?
    /// </summary>
    public required bool Nsfw { get; set; }
    
    /// <summary>
    /// The tags for this oekaki post.
    /// </summary>
    public string[]? Tags { get; set; }
    
    /// <summary>
    /// The alt text.
    /// </summary>
    public string? Alt { get; set; }
    
    /// <summary>
    /// Constructs an oekaki DTO from an oekaki model and the author's handle.
    /// </summary>
    /// <param name="oekakiModel">The oekaki model.</param>
    /// <param name="authorHandle">The author's handle.</param>
    /// <returns>The oekaki DTO.</returns>
    public static OekakiDto FromOekakiModel(
        OekakiModel oekakiModel,
        string authorHandle)
    {
        return new OekakiDto
        {
            OekakiRecordKey = oekakiModel.OekakiTid,
            AuthorDid = oekakiModel.AuthorDid,
            AuthorHandle = authorHandle,
            CreationTime = oekakiModel.IndexedAt,
            ImageLink =
                $"https://cdn.bsky.app/img/feed_fullsize/plain/{oekakiModel.AuthorDid}/{oekakiModel.BlobCid}",
            Tags = oekakiModel.TagOekakiRelations is not null 
                ? oekakiModel.TagOekakiRelations.Select(to => to.TagId).ToArray()
                : [],

            Nsfw = oekakiModel.IsNsfw ?? false,
            Alt = oekakiModel.AltText,
            
            AtProtoLink = $"at://{authorHandle}/com.shinolabs.pinksea.oekaki/{oekakiModel.OekakiTid}",
            OekakiCid = oekakiModel.RecordCid
        };
    }
}