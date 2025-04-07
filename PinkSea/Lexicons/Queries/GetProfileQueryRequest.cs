using System.Text.Json.Serialization;

namespace PinkSea.Lexicons.Queries;

/// <summary>
/// The request for the "com.shinolabs.pinksea.unspecced.getProfile" xrpc call.
/// </summary>
public class GetProfileQueryRequest
{
    /// <summary>
    /// The DID of the user.
    /// </summary>
    [JsonPropertyName("did")]
    public required string Did { get; set; }
}