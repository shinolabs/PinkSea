using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Streaming.JetStream.Events;

/// <summary>
/// The JetStream identity event kind.
/// </summary>
public class AtProtoIdentity
{
    /// <summary>
    /// The DID of the identity.
    /// </summary>
    [JsonPropertyName("did")]
    public required string Did { get; set; }
    
    /// <summary>
    /// The sequence number.
    /// </summary>
    [JsonPropertyName("seq")]
    public required ulong SequenceNumber { get; set; }
    
    /// <summary>
    /// The new handle of the DID.
    /// </summary>
    [JsonPropertyName("handle")]
    public string? Handle { get; set; }
    
    /// <summary>
    /// The timestamp.
    /// </summary>
    [JsonPropertyName("time")]
    public required DateTimeOffset Timestamp { get; set; }
}