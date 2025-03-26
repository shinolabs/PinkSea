using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons.AtProto;

/// <summary>
/// The request for the "com.atproto.repo.deleteRecord" xrpc procedure.
/// </summary>
public class DeleteRecordRequest
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
}