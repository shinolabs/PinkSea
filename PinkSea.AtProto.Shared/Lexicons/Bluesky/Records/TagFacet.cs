using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons.Bluesky.Records;

/// <summary>
/// A tag facet.
/// </summary>
public class TagFacet : BaseLexiconObject
{
    /// <summary>
    /// Constructs a tag facet.
    /// </summary>
    public TagFacet()
        : base("app.bsky.richtext.facet#tag")
    {
    }
    
    /// <summary>
    /// The tag.
    /// </summary>
    [JsonPropertyName("tag")]
    public required string Tag { get; set; }
}