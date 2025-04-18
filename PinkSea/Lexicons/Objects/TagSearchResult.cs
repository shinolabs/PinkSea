using System.Text.Json.Serialization;

namespace PinkSea.Lexicons.Objects;

/// <summary>
/// A tag search result.
/// </summary>
public class TagSearchResult
{
    /// <summary>
    /// The tag.
    /// </summary>
    [JsonPropertyName("tag")]
    public required string Tag { get; set; }
    
    /// <summary>
    /// The oekaki to represent it.
    /// </summary>
    [JsonPropertyName("oekaki")]
    public required HydratedOekaki Oekaki { get; set; }
    
    /// <summary>
    /// How many posts have this tag?
    /// </summary>
    [JsonPropertyName("count")]
    public required int Count { get; set; }
}