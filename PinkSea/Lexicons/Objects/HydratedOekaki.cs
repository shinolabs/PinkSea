using System.Text.Json.Serialization;
using PinkSea.Database.Models;

namespace PinkSea.Lexicons.Objects;

/// <summary>
/// The "com.shinolabs.pinksea.appViewDefs#hydratedOekaki" type.
/// </summary>
public class HydratedOekaki
{
    /// <summary>
    /// The DID of the author.
    /// </summary>
    [JsonPropertyName("did")]
    public required string Did { get; set; }
    
    /// <summary>
    /// The handle of the author.
    /// </summary>
    [JsonPropertyName("handle")]
    public required string Handle { get; set; }
    
    /// <summary>
    /// The image link.
    /// </summary>
    [JsonPropertyName("image")]
    public required string ImageLink { get; set; }
    
    /// <summary>
    /// The AT protocol link.
    /// </summary>
    [JsonPropertyName("at")]
    public required string AtProtoLink { get; set; }
    
    /// <summary>
    /// The oekaki CID.
    /// </summary>
    [JsonPropertyName("cid")]
    public required string Cid { get; set; }
    
    /// <summary>
    /// The creation time.
    /// </summary>
    [JsonPropertyName("creationTime")]
    public required DateTimeOffset CreationTime { get; set; }
    
    /// <summary>
    /// Is this oekaki NSFW?
    /// </summary>
    [JsonPropertyName("nsfw")]
    public required bool Nsfw { get; set; }
    
    /// <summary>
    /// The tags for this oekaki post.
    /// </summary>
    [JsonPropertyName("tags")]
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
    public static HydratedOekaki FromOekakiModel(
        OekakiModel oekakiModel,
        string authorHandle)
    {
        return new HydratedOekaki
        {
            Did = oekakiModel.AuthorDid,
            Handle = authorHandle,
            CreationTime = oekakiModel.IndexedAt,
            ImageLink =
                $"https://cdn.bsky.app/img/feed_fullsize/plain/{oekakiModel.AuthorDid}/{oekakiModel.BlobCid}",
            Tags = oekakiModel.TagOekakiRelations is not null 
                ? oekakiModel.TagOekakiRelations.Select(to => to.TagId).ToArray()
                : [],

            Nsfw = oekakiModel.IsNsfw ?? false,
            Alt = oekakiModel.AltText,
            
            AtProtoLink = $"at://{oekakiModel.AuthorDid}/com.shinolabs.pinksea.oekaki/{oekakiModel.OekakiTid}",
            Cid = oekakiModel.RecordCid
        };
    }
}