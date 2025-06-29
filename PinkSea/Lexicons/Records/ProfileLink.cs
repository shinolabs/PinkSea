using System.Text.Json.Serialization;

namespace PinkSea.Lexicons.Records;

/// <summary>
/// A link in a profile.
/// </summary>
public class ProfileLink
{
    /// <summary>
    /// The name of this profile link.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    /// <summary>
    /// The link it is pointing to.
    /// </summary>
    [JsonPropertyName("link")]
    public required string Link { get; set; }
}