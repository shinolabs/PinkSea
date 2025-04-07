using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Streaming.JetStream.Events;

/// <summary>
/// The JetStream account event kind.
/// </summary>
public class AtProtoAccount
{
    /// <summary>
    /// Whether this account is active.
    /// </summary>
    [JsonPropertyName("active")]
    public required bool Active { get; set; }
    
    /// <summary>
    /// The DID for this account.
    /// </summary>
    [JsonPropertyName("did")]
    public required string Did { get; set; }
    
    /// <summary>
    /// The sequence number.
    /// </summary>
    [JsonPropertyName("seq")]
    public required ulong SequenceNumber { get; set; }
    
    /// <summary>
    /// The timestamp.
    /// </summary>
    [JsonPropertyName("time")]
    public required DateTimeOffset Timestamp { get; set; }
    
    /// <summary>
    /// The status of the account.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }
}