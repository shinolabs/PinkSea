using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons.Bluesky.Records;

/// <summary>
/// A link facet.
/// </summary>
public class LinkFacet : BaseLexiconObject
{
    /// <summary>
    /// A link facet.
    /// </summary>
    public LinkFacet()
        : base("app.bsky.richtext.facet#link")
    {
    }
    
    /// <summary>
    /// The URI of the link.
    /// </summary>
    [JsonPropertyName("uri")]
    public required string Uri { get; set; }
}