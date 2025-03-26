using System.Text.Json.Serialization;
using PinkSea.AtProto.Shared.Lexicons.Types;

namespace PinkSea.AtProto.Shared.Lexicons.Bluesky.Records;

/// <summary>
/// The profile record.
/// </summary>
public class Profile
{
    /// <summary>
    /// The display name.
    /// </summary>
    [JsonPropertyName("displayName")]
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
    public Blob? Avatar { get; set; }
    
    /// <summary>
    /// The description.
    /// </summary>
    [JsonPropertyName("banner")]
    public Blob? Banner { get; set; }
}