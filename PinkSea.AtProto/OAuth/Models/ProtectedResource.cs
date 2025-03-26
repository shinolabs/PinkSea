using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Models.OAuth;

public class ProtectedResource
{
    [JsonPropertyName("resource")]
    public required string Resource { get; init; }
    
    [JsonPropertyName("authorization_servers")]
    public required IReadOnlyList<string> AuthorizationServers { get; init; }
    
    [JsonPropertyName("bearer_methods_supported")]
    public required IReadOnlyList<string> BearerMethodsSupported { get; init; }

    public string? GetAuthorizationServer() => AuthorizationServers.FirstOrDefault();

    // TODO: scopes_supported, resource_documentation
}