using System.Text.Json.Serialization;

namespace PinkSea.Lexicons.Objects;

/// <summary>
/// A profile link.
/// </summary>
public class Link
{
    /// <summary>
    /// The name of the link.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    /// <summary>
    /// The url.
    /// </summary>
    [JsonPropertyName("url")]
    public required string Url { get; set; }
}