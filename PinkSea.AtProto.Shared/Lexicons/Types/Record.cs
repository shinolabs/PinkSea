using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons.Types;

/// <summary>
/// An AT Protocol record.
/// </summary>
public class Record<TInner>
{
    /// <summary>
    /// The at:// URI of the record.
    /// </summary>
    [JsonPropertyName("uri")]
    public required string Uri { get; set; }
    
    /// <summary>
    /// The CID of the record.
    /// </summary>
    [JsonPropertyName("cid")]
    public required string Cid { get; set; }
    
    /// <summary>
    /// The value of the record.
    /// </summary>
    [JsonPropertyName("value")]
    public required TInner Value { get; set; }
}