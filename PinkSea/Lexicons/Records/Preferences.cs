using System.Text.Json.Serialization;
using PinkSea.Lexicons.Objects;

namespace PinkSea.Lexicons.Records;

/// <summary>
/// The "com.shinolabs.pinksea.preferences" lexicon.
/// </summary>
public class Preferences
{
    /// <summary>
    /// The preferences.
    /// </summary>
    [JsonPropertyName("values"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required IReadOnlyList<Preference> Values { get; set; }
}