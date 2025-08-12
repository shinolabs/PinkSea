using System.Text.Json.Serialization;

namespace PinkSea.Gateway.ActivityStreams;

/// <summary>
/// An ActivityStreams hashtag.
/// </summary>
public class Hashtag
{
    /// <summary>
    /// The ID of the hashtag.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    /// <summary>
    /// The name of the hashtag.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }
}