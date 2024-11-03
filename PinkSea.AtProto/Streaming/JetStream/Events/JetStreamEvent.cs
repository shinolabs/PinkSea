using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Streaming.JetStream.Events;

/// <summary>
/// Represents a JetStream event.
/// </summary>
public class JetStreamEvent
{
    /// <summary>
    /// The DID responsible for this event.
    /// </summary>
    [JsonPropertyName("did")]
    public required string Did { get; set; }
    
    /// <summary>
    /// The time in microseconds for when this event has occured.
    /// </summary>
    [JsonPropertyName("time_us")]
    public required long TimeInMicroseconds { get; set; }
    
    /// <summary>
    /// The kind of the event.
    /// </summary>
    [JsonPropertyName("kind")]
    public required string Kind { get; set; }
    
    /// <summary>
    /// The commit data.
    /// </summary>
    [JsonPropertyName("commit")]
    public AtProtoCommit? Commit { get; set; }
    
    /// <summary>
    /// The identity data.
    /// </summary>
    [JsonPropertyName("identity")]
    public AtProtoIdentity? Identity { get; set; }
    
    /// <summary>
    /// The identity data.
    /// </summary>
    [JsonPropertyName("account")]
    public AtProtoAccount? Account { get; set; }
}