using System.Text.Json.Serialization;

namespace PinkSea.Gateway.ActivityStreams;

/// <summary>
/// An ActivityStreams document.
/// </summary>
public class Document : IActivityStreamsObject
{
    /// <inheritdoc />
    [JsonPropertyName("type")]
    public string Type => "Document";
    
    /// <summary>
    /// The name of the document.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    /// <summary>
    /// The hyperlink.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Href { get; set; }
    
    /// <summary>
    /// The media type of the document.
    /// </summary>
    [JsonPropertyName("mediaType")]
    public string? MediaType { get; set; }
}