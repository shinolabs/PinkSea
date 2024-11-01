namespace PinkSea.Models.Dto;

public class OekakiDto
{
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
    /// The creation time.
    /// </summary>
    public required DateTimeOffset CreationTime { get; set; }
    
    /// <summary>
    /// The tags for this oekaki post.
    /// </summary>
    public string[]? Tags { get; set; }
}