using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Models.Did;

public class DidVerificationMethod
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("controller")]
    public required string Controller { get; init; }

    [JsonPropertyName("publicKeyMultibase")]
    public required string PublicKeyMultibase { get; init; }
}