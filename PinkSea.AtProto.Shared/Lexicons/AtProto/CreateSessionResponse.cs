using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons.AtProto;

/// <summary>
/// The response for the "com.atproto.server.createSession" xrpc call.
/// </summary>
public class CreateSessionResponse
{
    /// <summary>
    /// The access token.
    /// </summary>
    [JsonPropertyName("accessJwt")]
    public required string AccessToken { get; set; }
    
    /// <summary>
    /// The refresh token.
    /// </summary>
    [JsonPropertyName("refreshJwt")]
    public required string RefreshToken { get; set; }
    
    /// <summary>
    /// The handle.
    /// </summary>
    [JsonPropertyName("handle")]
    public required string Handle { get; set; }
    
    /// <summary>
    /// The DID.
    /// </summary>
    [JsonPropertyName("did")]
    public required string Did { get; set; }
    
    /// <summary>
    /// Whether this account is active or not.
    /// </summary>
    [JsonPropertyName("active")]
    public bool Active { get; set; }
}