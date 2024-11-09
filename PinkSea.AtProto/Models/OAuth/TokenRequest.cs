using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Models.OAuth;

/// <summary>
/// The OAuth token request.
/// </summary>
public class TokenRequest
{
    /// <summary>
    /// Identifies the client software.
    /// </summary>
    [JsonPropertyName("client_id")]
    public required string ClientId { get; init; }

    /// <summary>
    /// The grant type.
    /// </summary>
    [JsonPropertyName("grant_type")]
    public required string GrantType { get; init; }

    /// <summary>
    /// The redirect uri.
    /// </summary>
    [JsonPropertyName("redirect_uri")]
    public required string? RedirectUri { get; init; }
    
    /// <summary>
    /// The PKCE challenge value.
    /// </summary>
    [JsonPropertyName("code_verifier")]
    public required string CodeVerifier { get; init; }
    
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
    /// The code.
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; init; }
    
    /// <summary>
    /// The refresh token.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; init; }
}