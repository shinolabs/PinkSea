namespace PinkSea.Gateway.Models.Oekaki;

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
}