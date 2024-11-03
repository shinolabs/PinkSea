namespace PinkSea.AtProto.Streaming.JetStream;

/// <summary>
/// Options for configuring jetstream.
/// </summary>
public class JetStreamOptions
{
    /// <summary>
    /// The endpoint to use.
    /// </summary>
    public string? Endpoint { get; set; }
    
    /// <summary>
    /// The wanted collections
    /// </summary>
    public string[]? WantedCollections { get; set; }
}