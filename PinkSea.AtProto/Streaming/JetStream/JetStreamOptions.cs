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
    
    /// <summary>
    /// The cursor.
    /// </summary>
    public string? Cursor { get; set; }
    
    /// <summary>
    /// The path to the cursorfile.
    /// </summary>
    public string? CursorFilePath { get; set; }

    /// <summary>
    /// Defines how many events can be processed in parallel.
    /// </summary>
    public int DegreeOfParallelism { get; set; } = 5;
}