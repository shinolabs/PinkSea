using System.Text.Json.Serialization;

namespace PinkSea.Gateway.Lexicons;

/// <summary>
/// The "com.shinolabs.pinksea.appViewDefs#hydratedOekaki" type.
/// </summary>
public class OekakiDto
{
    /// <summary>
    /// The author of the oekaki.
    /// </summary>
    [JsonPropertyName("author")]
    public required Author Author { get; set; }
    
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
}