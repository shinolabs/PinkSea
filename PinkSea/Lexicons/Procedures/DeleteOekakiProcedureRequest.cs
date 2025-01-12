using System.Text.Json.Serialization;

namespace PinkSea.Lexicons.Procedures;

/// <summary>
/// The request for the "com.shinolabs.pinksea.deleteOekaki" xrpc call.
/// </summary>
public class DeleteOekakiProcedureRequest
{
    /// <summary>
    /// The record key of the oekaki.
    /// </summary>
    [JsonPropertyName("rkey")]
    public required string RecordKey { get; set; }
}