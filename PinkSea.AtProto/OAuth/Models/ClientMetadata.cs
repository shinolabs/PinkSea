using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Models.OAuth;

public class ClientMetadata
{
    [JsonPropertyName("client_id")]
    public required string ClientId { get; init; }
    
    [JsonPropertyName("client_name")]
    public required string ClientName { get; init; }

    [JsonPropertyName("grant_types")]
    public required IReadOnlyList<string> GrantTypes { get; init; } = ["authorization_code"];

    [JsonPropertyName("scope")]
    public required string Scope { get; init; } = "atproto";
    
    [JsonPropertyName("redirect_uris")]
    public required IReadOnlyList<string> RedirectUris { get; init; }

    [JsonPropertyName("dpop_bound_access_tokens")]
    public required bool DpopBoundAccessTokens { get; init; } = true;
    
    [JsonPropertyName("application_type")]
    public string? ApplicationType { get; init; } = "native";
    
    [JsonPropertyName("token_endpoint_auth_method")]
    public string? TokenEndpointAuthMethod { get; init; }
    
    [JsonPropertyName("token_endpoint_auth_signing_alg")]
    public string? TokenEndpointAuthSigningAlgorithm { get; init; }
    
    [JsonPropertyName("jwks_uri")]
    public string? JwksUri { get; init; }
}