using System.Text.Json.Serialization;

namespace PinkSea.Lexicons.Queries;

/// <summary>
/// The response for the "com.shinolabs.pinksea.getHandleFromDid" xrpc call.
/// </summary>
public class GetHandleFromDidQueryResponse
{
    /// <summary>
    /// The handle.
    /// </summary>
    [JsonPropertyName("handle")]
    public required string Handle { get; set; }
}