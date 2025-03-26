using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Models.OAuth;

public class AuthorizationServer
{
    [JsonPropertyName("issuer")]
    public required string Issuer { get; init; }
    
    [JsonPropertyName("authorization_endpoint")]
    public required string AuthorizationEndpoint { get; init; }
    
    [JsonPropertyName("token_endpoint")]
    public required string TokenEndpoint { get; init; }
    
    [JsonPropertyName("pushed_authorization_request_endpoint")]
    public required string PushedAuthorizationRequestEndpoint { get; init; }
    
    // TODO: Rest from https://atproto.com/specs/oauth#summary-of-authorization-flow
}