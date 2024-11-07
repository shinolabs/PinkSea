using System.Text.Json.Serialization;

namespace PinkSea.Lexicons.Queries;

/// <summary>
/// The request for the "com.shinolabs.pinksea.getHandleFromDid" xrpc call.
/// </summary>
public class GetHandleFromDidQueryRequest
{
    /// <summary>
    /// The DID.
    /// </summary>
    [JsonPropertyName("did")]
    public required string Did { get; set; }
}