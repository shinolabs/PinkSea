using System.Text.Json.Serialization;

namespace PinkSea.Gateway.Lexicons;

/// <summary>
/// The response for the "com.shinolabs.pinksea.getProfile" XRPC request.
/// </summary>
public class GetProfileResponse
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
}