using System.Text.Json.Serialization;

namespace PinkSea.Lexicons.Procedures;

/// <summary>
/// Response for the "com.shinolabs.pinksea.putOekaki" procedure.
/// </summary>
public class PutOekakiProcedureResponse
{
    /// <summary>
    /// The at:// uri.
    /// </summary>
    [JsonPropertyName("uri")]
    public required string AtLink { get; set; }
    
    /// <summary>
    /// The record key.
    /// </summary>
    [JsonPropertyName("rkey")]
    public required string RecordKey { get; set; }
}