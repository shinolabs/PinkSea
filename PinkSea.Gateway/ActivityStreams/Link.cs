using System.Text.Json.Serialization;

namespace PinkSea.Gateway.ActivityStreams;

/// <summary>
/// An ActivityStreams image.
/// </summary>
public class Image : IActivityStreamsObject
{
    /// <inheritdoc />
    [JsonPropertyName("type")]
    public string Type => "Image";
    
    /// <summary>
    /// The url of the image.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}