using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Models.Did;

public class DidService
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    
    [JsonPropertyName("type")]
    public required string Type { get; init; }
    
    [JsonPropertyName("serviceEndpoint")]
    public required string ServiceEndpoint { get; init; }
}