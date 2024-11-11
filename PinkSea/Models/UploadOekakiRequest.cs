using System.Text.Json.Serialization;

namespace PinkSea.Models;

/// <summary>
/// An upload oekaki request.
/// </summary>
public class UploadOekakiRequest
{
    /// <summary>
    /// The data: image/png field.
    /// </summary>
    [JsonPropertyName("data")]
    public required string Data { get; set; }
    
    /// <summary>
    /// The tags attached to this oekaki image.
    /// </summary>
    [JsonPropertyName("tags")]
    public string[]? Tags { get; set; }
    
    /// <summary>
    /// The alt text.
    /// </summary>
    [JsonPropertyName("alt")]
    public string? AltText { get; set; }
    
    /// <summary>
    /// The at:// url of the parent.
    /// </summary>
    [JsonPropertyName("parent")]
    public string? ParentAtUrl { get; set; }

    /// <summary>
    /// Is this oekaki NSFW?
    /// </summary>
    [JsonPropertyName("nsfw")]
    public bool? Nsfw { get; set; } = false;

    /// <summary>
    /// Should the image be crossposted to Bluesky?
    /// </summary>
    [JsonPropertyName("bskyCrosspost")]
    public bool? BlueskyCrosspost { get; set; } = false;
}