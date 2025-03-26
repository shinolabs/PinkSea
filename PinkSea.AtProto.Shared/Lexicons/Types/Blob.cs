using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons.Types;

/// <summary>
/// An AT protocol blob.
/// </summary>
public class Blob
{
    /// <summary>
    /// The type of the blob.
    /// </summary>
    [JsonPropertyName("$type")]
    public required string Type { get; set; }
    
    /// <summary>
    /// The reference.
    /// </summary>
    [JsonPropertyName("ref")]
    public required Reference Reference { get; set; }
    
    /// <summary>
    /// The MIME type.
    /// </summary>
    [JsonPropertyName("mimeType")]
    public string? MimeType { get; set; }
    
    /// <summary>
    /// The size of the blob.
    /// </summary>
    [JsonPropertyName("size")]
    public long Size { get; set; }
}