using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Models.Did;

public class DidResponse
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    
    [JsonPropertyName("alsoKnownAs")]
    public required IReadOnlyList<string> AlsoKnownAs { get; init; }
    
    [JsonPropertyName("verificationMethods")]
    public IReadOnlyList<DidVerificationMethod>? VerificationMethods { get; init; }
    
    [JsonPropertyName("service")]
    public required IReadOnlyList<DidService> Services { get; init; }

    /// <summary>
    /// Gets the PDS for this Did.
    /// </summary>
    /// <returns>The address of the PDS.</returns>
    public string? GetPds() => Services
        .FirstOrDefault(s => s.Id == "#atproto_pds")?
        .ServiceEndpoint;
}