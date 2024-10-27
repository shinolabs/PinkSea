using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Lexicons.Bluesky.Records;

/// <summary>
/// The profile record.
/// </summary>
public class Profile
{
    /// <summary>
    /// The DID.
    /// </summary>
    [JsonPropertyName("did")]
    public required string Did { get; set; }
    
    /// <summary>
    /// The handle of the profile.
    /// </summary>
    [JsonPropertyName("handle")]
    public required string Handle { get; set; }

    /// <summary>
    /// The display name.
    /// </summary>
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }
    
    /// <summary>
    /// The description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    /// <summary>
    /// The description.
    /// </summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }
    
    /// <summary>
    /// The description.
    /// </summary>
    [JsonPropertyName("banner")]
    public string? Banner { get; set; }
}