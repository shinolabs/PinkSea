using System.Text.Json.Serialization;

namespace PinkSea.Gateway.Lexicons;

/// <summary>
/// The response for the "com.shinolabs.pinksea.getParentForReply" xrpc query.
/// </summary>
public class GetParentForReplyResponse
{
    /// <summary>
    /// The DID of the author.
    /// </summary>
    [JsonPropertyName("did")]
    public required string AuthorDid { get; init; }
    
    /// <summary>
    /// The record key.
    /// </summary>
    [JsonPropertyName("rkey")]
    public required string RecordKey { get; init; }
}