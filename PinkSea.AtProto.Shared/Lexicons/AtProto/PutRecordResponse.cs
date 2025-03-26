using System.Text.Json.Serialization;
using PinkSea.AtProto.Shared.Lexicons.Types;

namespace PinkSea.AtProto.Shared.Lexicons.AtProto;

/// <summary>
/// The response produced by "com.atproto.repo.putRecord".
/// </summary>
public class PutRecordResponse
{
    /// <summary>
    /// The at:// URI of the record.
    /// </summary>
    [JsonPropertyName("uri")]
    public required string Uri { get; set; }
    
    /// <summary>
    /// The CID of the record.
    /// </summary>
    [JsonPropertyName("cid")]
    public required string Cid { get; set; }
    
    /// <summary>
    /// The commit of the record.
    /// </summary>
    [JsonPropertyName("commit")]
    public required Commit Commit { get; set; }
    
    /// <summary>
    /// The validation result of the record.
    /// </summary>
    [JsonPropertyName("validationStatus")]
    public required string ValidationStatus { get; set; }
}