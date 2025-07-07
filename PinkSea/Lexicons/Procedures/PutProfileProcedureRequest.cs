using System.Text.Json.Serialization;
using PinkSea.Lexicons.Records;

namespace PinkSea.Lexicons.Procedures;

/// <summary>
/// The request for the "com.shinolabs.pinksea.putProfile" xrpc call.
/// </summary>
public class PutProfileProcedureRequest
{
    /// <summary>
    /// The updated profile.
    /// </summary>
    [JsonPropertyName("profile")]
    public required Profile Profile { get; set; }
}