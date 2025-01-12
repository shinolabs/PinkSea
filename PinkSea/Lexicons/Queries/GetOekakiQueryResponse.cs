using PinkSea.Lexicons.Objects;

namespace PinkSea.Lexicons.Queries;

/// <summary>
/// Response for the "com.shinolabs.pinksea.getOekaki" query.
/// </summary>
public class GetOekakiQueryResponse
{
    /// <summary>
    /// The parent post.
    /// </summary>
    public required object Parent { get; set; }
    
    /// <summary>
    /// The children of the post, sorted by time.
    /// </summary>
    public required HydratedOekaki[] Children { get; set; }
}