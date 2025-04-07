using System.Text.Json.Serialization;
using PinkSea.Lexicons.Objects;

namespace PinkSea.Lexicons.Queries;

/// <summary>
/// The response for the "com.shinolabs.pinksea.unspecced.getProfile" xrpc query.
/// </summary>
public class GetProfileQueryResponse
{
    /// <summary>
    /// The DID of the user.
    /// </summary>
    [JsonPropertyName("did")]
    public required string Did { get; set; }
    
    /// <summary>
    /// The handle of the user.
    /// </summary>
    [JsonPropertyName("handle")]
    public required string Handle { get; set; }
    
    /// <summary>
    /// The nickname selected by the user.
    /// </summary>
    [JsonPropertyName("nick")]
    public string? Nickname { get; set; }
    
    /// <summary>
    /// The URL of the avatar.
    /// </summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }
    
    /// <summary>
    /// The description of this user.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    /// <summary>
    /// The links this profile has.
    /// </summary>
    [JsonPropertyName("links")]
    public IReadOnlyList<Link>? Links { get; set; }
}