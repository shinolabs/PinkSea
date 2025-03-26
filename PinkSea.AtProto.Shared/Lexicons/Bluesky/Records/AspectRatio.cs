using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons.Bluesky.Records;

/// <summary>
/// The aspect ratio of the image.
/// </summary>
public class AspectRatio
{
    /// <summary>
    /// The width.
    /// </summary>
    [JsonPropertyName("width")]
    public required int Width { get; set; }
    
    /// <summary>
    /// The height.
    /// </summary>
    [JsonPropertyName("height")]
    public required int Height { get; set; }
}