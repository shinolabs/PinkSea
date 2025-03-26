using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons.Bluesky.Records;

/// <summary>
/// An image embed.
/// </summary>
public class ImageEmbed : BaseLexiconObject
{
    /// <summary>
    /// The image embed constructor.
    /// </summary>
    public ImageEmbed()
        : base("app.bsky.embed.images")
    {
    }
    
    /// <summary>
    /// The list of images.
    /// </summary>
    [JsonPropertyName("images")]
    public required IReadOnlyList<Image> Images { get; set; }
}