using PinkSea.Models.Dto;

namespace PinkSea.Lexicons.Queries;

/// <summary>
/// Response for the "com.shinolabs.pinksea.getOekaki" query.
/// </summary>
public class GetOekakiQueryResponse
{
    /// <summary>
    /// The parent post.
    /// </summary>
    public required OekakiDto Parent { get; set; }
    
    /// <summary>
    /// The children of the post, sorted by time.
    /// </summary>
    public required OekakiDto[] Children { get; set; }
}