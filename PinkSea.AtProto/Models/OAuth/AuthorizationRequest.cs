using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Models.OAuth;

public class AuthorizationRequest
{
    /// <summary>
    /// Identifies the client software.
    /// </summary>
    [JsonPropertyName("client_id")]
    public required string ClientId { get; init; }
    
    /// <summary>
    /// The response type.
    /// </summary>
    [JsonPropertyName("response_type")]
    public required string ResponseType { get; init; } = "code";
    
    /// <summary>
    /// The PKCE challenge value.
    /// </summary>
    [JsonPropertyName("code_challenge")]
    public required string CodeChallenge { get; init; }
    
    /// <summary>
    /// Which code challenge method is used.
    /// </summary>
    [JsonPropertyName("code_challenge_method")]
    public required string CodeChallengeMethod { get; init; }
    
    /// <summary>
    /// Random token used to verify the authorization request against the response.
    /// </summary>
    [JsonPropertyName("state")]
    public required string State { get; init; }
    
    /// <summary>
    /// Must match against URIs declared in client metadata and have
    /// a format consistent with the application_type declared in the client metadata.
    /// </summary>
    [JsonPropertyName("redirect_uri")]
    public required string RedirectUrl { get; init; }
    
    /// <summary>
    /// Must be a subset of the scopes declared in client metadata.
    /// </summary>
    [JsonPropertyName("scope")]
    public required string Scope { get; init; }
    
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
    /// The login hint.
    /// </summary>
    [JsonPropertyName("login_hint")]
    public string? LoginHint { get; init; }
}