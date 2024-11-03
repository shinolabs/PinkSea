using System.Text.Json;
using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Streaming.JetStream.Events;

/// <summary>
/// The JetStream commit event kind.
/// </summary>
public class AtProtoCommit
{
    /// <summary>
    /// The revision of the commit.
    /// </summary>
    [JsonPropertyName("rev")]
    public required string Revision { get; set; }
    
    /// <summary>
    /// The operation.
    /// </summary>
    [JsonPropertyName("operation")]
    public required string Operation { get; set; }
    
    /// <summary>
    /// The collection this operation is directed for.
    /// </summary>
    [JsonPropertyName("collection")]
    public required string Collection { get; set; }
    
    /// <summary>
    /// The record key.
    /// </summary>
    [JsonPropertyName("rkey")]
    public required string RecordKey { get; set; }
    
    /// <summary>
    /// The CID.
    /// </summary>
    [JsonPropertyName("cid")]
    public string? Cid { get; set; }
    
    /// <summary>
    /// The record.
    /// </summary>
    [JsonPropertyName("record")]
    public JsonElement? Record { get; set; }
}