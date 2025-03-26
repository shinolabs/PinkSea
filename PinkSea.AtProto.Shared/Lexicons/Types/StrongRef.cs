using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons.Types;

/// <summary>
/// A URI with a content-hash fingerprint.
/// </summary>
public class StrongRef
{
    /// <summary>
    /// The at:// uri.
    /// </summary>
    [JsonPropertyName("uri")]
    public required string Uri { get; set; }
    
    /// <summary>
    /// The content id fingerprint.
    /// </summary>
    [JsonPropertyName("cid")]
    public required string Cid { get; set; }
}