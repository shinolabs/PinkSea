using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons.AtProto;

/// <summary>
/// The request for the "com.atproto.sync.getRepoStatus" XRPC call.
/// </summary>
public class GetRepoStatusRequest
{
    /// <summary>
    /// The DID we're querying.
    /// </summary>
    [JsonPropertyName("did")]
    public required string Did { get; set; }
}