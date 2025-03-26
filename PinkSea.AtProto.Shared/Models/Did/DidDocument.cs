using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Models.Did;

/// <summary>
/// A DID document.
/// </summary>
public class DidDocument
{
    /// <summary>
    /// The JSON-LD context.
    /// </summary>
    [JsonPropertyName("@context")]
    public IReadOnlyList<string>? Context { get; init; }
    
    /// <summary>
    /// The ID of the document.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    
    /// <summary>
    /// The services operating for this DID.
    /// </summary>
    [JsonPropertyName("service")]
    public required IReadOnlyList<DidService> Services { get; init; }

    /// <summary>
    /// Other names this DID is known as.
    /// </summary>
    [JsonPropertyName("alsoKnownAs")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? AlsoKnownAs { get; init; }
    
    /// <summary>
    /// The verification methods.
    /// </summary>
    [JsonPropertyName("verificationMethods")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<DidVerificationMethod>? VerificationMethods { get; init; }
    
    /// <summary>
    /// Gets the PDS for this Did.
    /// </summary>
    /// <returns>The address of the PDS.</returns>
    public string? GetPds() => Services
        .FirstOrDefault(s => s.Id == "#atproto_pds")?
        .ServiceEndpoint;

    /// <summary>
    /// Gets the handle from the AKA field.
    /// </summary>
    /// <returns>The handle.</returns>
    public string? GetHandle() => AlsoKnownAs?
        .FirstOrDefault(aka => aka.StartsWith("at://"))?
        .Replace("at://", "");
}