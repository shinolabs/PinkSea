using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Lexicons.Bluesky.Records;

/// <summary>
/// A Bluesky post record.
/// </summary>
public class Post : BaseLexiconObject
{
    /// <summary>
    /// The post constructor.
    /// </summary>
    public Post()
        : base("app.bsky.feed.post")
    {
    }
    
    /// <summary>
    /// The post text.
    /// </summary>
    [JsonPropertyName("text")]
    public required string Text { get; set; }
    
    /// <summary>
    /// When was the post created at?.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public required DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// The image embed.
    /// </summary>
    [JsonPropertyName("embed")]
    public ImageEmbed? Embed { get; set; }

    /// <summary>
    /// The facets used by this post.
    /// </summary>
    [JsonPropertyName("facets")]
    public IEnumerable<Facet>? Facets { get; set; }
}