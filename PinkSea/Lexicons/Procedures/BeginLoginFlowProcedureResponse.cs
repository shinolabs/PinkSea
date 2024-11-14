using System.Text.Json.Serialization;

namespace PinkSea.Lexicons.Procedures;

/// <summary>
/// Response for the "com.shinolabs.pinksea.beginLoginFlow" procedure.
/// </summary>
public class BeginLoginFlowProcedureResponse
{
    /// <summary>
    /// The redirect url.
    /// </summary>
    [JsonPropertyName("redirect")]
    public string? Redirect { get; set; }
    
    /// <summary>
    /// The failure reason.
    /// </summary>
    [JsonPropertyName("failureReason")]
    public string? FailureReason { get; set; }
}