using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons.Types;

/// <summary>
/// An AT protocol reference.
/// </summary>
public class Reference
{
    /// <summary>
    /// The link of the object.
    /// </summary>
    [JsonPropertyName("$link")]
    public required string Link { get; set; }
}