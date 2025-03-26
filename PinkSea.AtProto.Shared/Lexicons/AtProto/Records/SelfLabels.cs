using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons.AtProto.Records;

/// <summary>
/// The self-labels.
/// </summary>
public class SelfLabels : BaseLexiconObject
{
    /// <summary>
    /// Constructs a new self-label
    /// </summary>
    public SelfLabels()
        : base("com.atproto.label.defs#selfLabels")
    {
    }
    
    /// <summary>
    /// The self-label values.
    /// </summary>
    [JsonPropertyName("values")]
    public required IReadOnlyList<SelfLabel> Values { get; set; }
}