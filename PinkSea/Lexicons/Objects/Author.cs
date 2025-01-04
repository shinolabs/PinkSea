using System.Text.Json.Serialization;

namespace PinkSea.Lexicons.Objects;

/// <summary>
/// The "com.shinolabs.pinksea.appViewDefs#author" object.
/// </summary>
public class Author
{
    /// <summary>
    /// The DID of the author.
    /// </summary>
    [JsonPropertyName("did")]
    public required string Did { get; set; }
    
    /// <summary>
    /// The handle of the author.
    /// </summary>
    [JsonPropertyName("handle")]
    public required string Handle { get; set; }
}