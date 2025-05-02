using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons.AtProto;

/// <summary>
/// A response for the "com.atproto.sync.getRepoStatus" XRPC call.
/// </summary>
public class GetRepoStatusResponse
{
    /// <summary>
    /// The DID for this repo.
    /// </summary>
    [JsonPropertyName("did")]
    public required string Did { get; set; }
    
    /// <summary>
    /// Whether this repo is active.
    /// </summary>
    [JsonPropertyName("active")]
    public required bool Active { get; set; }
    
    /// <summary>
    /// The status of the repo.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }
}