using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons.AtProto;

/// <summary>
/// The "com.atproto.repo.putRecord" request.
/// </summary>
public class PutRecordRequest
{
    /// <summary>
    /// The handle or DID of the repo (aka, current account).
    /// </summary>
    [JsonPropertyName("repo")]
    public required string Repo { get; set; }
    
    /// <summary>
    /// The NSID of the record collection.
    /// </summary>
    [JsonPropertyName("collection")]
    public required string Collection { get; set; }
    
    /// <summary>
    /// The Record Key.
    /// </summary>
    [JsonPropertyName("rkey")]
    public required string RecordKey { get; set; }
    
    /// <summary>
    /// The record to put.
    /// </summary>
    [JsonPropertyName("record")]
    public required object Record { get; set; }
}