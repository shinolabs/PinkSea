using System.Text.Json.Serialization;
using PinkSea.AtProto.Shared.Lexicons.AtProto.Records;

namespace PinkSea.AtProto.Shared.Lexicons.Bluesky.Records;

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
    [JsonPropertyName("embed"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ImageEmbed? Embed { get; set; }

    /// <summary>
    /// The facets used by this post.
    /// </summary>
    [JsonPropertyName("facets"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<Facet>? Facets { get; set; }
    
    /// <summary>
    /// The self-labels.
    /// </summary>
    [JsonPropertyName("labels"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SelfLabels? SelfLabel { get; set; }
}