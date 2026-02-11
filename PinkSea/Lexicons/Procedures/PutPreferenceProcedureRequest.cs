using System.Text.Json.Serialization;

namespace PinkSea.Lexicons.Procedures;

/// <summary>
/// The request for the "com.shinolabs.pinksea.putPreference" xrpc call.
/// </summary>
public class PutPreferenceProcedureRequest
{
    /// <summary>
    /// The key of the preference.
    /// </summary>
    [JsonPropertyName("key")]
    public required string Key { get; set; }
    
    /// <summary>
    /// The value of the preference.
    /// </summary>
    [JsonPropertyName("value")]
    public required string Value { get; set; }
}