using System.Text.Json.Serialization;

namespace PinkSea.AtProto.OAuth.Models;

public class TokenRevokeRequest
{
    /// <summary>
    /// Identifies the client software.
    /// </summary>
    [JsonPropertyName("client_id")]
    public required string ClientId { get; init; }
    
    /// <summary>
    /// The PKCE challenge value.
    /// </summary>
    [JsonPropertyName("code_verifier")]
    public required string CodeVerifier { get; init; }
    
    /// <summary>
    /// The token to revoke.
    /// </summary>
    [JsonPropertyName("token")]
    public required string Token { get; init; }
    
    /// <summary>
    /// Used by confidential clients to describe the client authentication mechanism. 
    /// </summary>
    [JsonPropertyName("client_assertion_type")]
    public string? ClientAssertionType { get; init; }
    
    /// <summary>
    /// Only used for confidential clients, for client authentication
    /// </summary>
    [JsonPropertyName("client_assertion")]
    public string? ClientAssertion { get; init; }
    
    /// <summary>
    /// The hint as to the type of the token being revoked.
    /// </summary>
    [JsonPropertyName("token_type_hint")]
    public string? TokenTypeHint { get; init; }
}