using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons.AtProto.Records;

/// <summary>
/// A self label.
/// </summary>
public class SelfLabel
{
    /// <summary>
    /// The value of the label.
    /// </summary>
    [JsonPropertyName("val")]
    public required string Value { get; set; }
}