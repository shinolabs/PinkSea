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
}