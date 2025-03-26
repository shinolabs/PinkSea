using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Models.OAuth;

/// <summary>
/// The OAuth2 token response.
/// </summary>
public class TokenResponse
{
    /// <summary>
    /// The access token.
    /// </summary>
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }
    
    /// <summary>
    /// The token type.
    /// </summary>
    [JsonPropertyName("token_type")]
    public required string TokenType { get; set; }
    
    /// <summary>
    /// The scopes.
    /// </summary>
    [JsonPropertyName("scope")]
    public required string Scope { get; set; }
    
    /// <summary>
    /// In how long does the token expire?
    /// </summary>
    [JsonPropertyName("expires_in")]
    public required int ExpiresIn { get; set; }
    
    /// <summary>
    /// The subject DID.
    /// </summary>
    [JsonPropertyName("sub")]
    public required string Subject { get; set; }
    
    /// <summary>
    /// The refresh token.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
}