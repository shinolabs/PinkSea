namespace PinkSea.Gateway.Lexicons;

/// <summary>
/// The response for the "com.shinolabs.pinksea.getOekaki" call.
/// </summary>
public class GetOekakiResponse
{
    /// <summary>
    /// The parent post.
    /// </summary>
    public required OekakiDto Parent { get; set; }
}