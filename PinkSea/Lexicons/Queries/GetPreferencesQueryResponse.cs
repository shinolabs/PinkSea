using System.Text.Json.Serialization;
using PinkSea.Lexicons.Objects;

namespace PinkSea.Lexicons.Queries;

/// <summary>
/// The response for the "com.shinolabs.pinksea.getPreferences" xrpc query.
/// </summary>
public class GetPreferencesQueryResponse
{
    /// <summary>
    /// The preferences.
    /// </summary>
    [JsonPropertyName("preferences")]
    public required IReadOnlyList<Preference> Preferences { get; set; }
}