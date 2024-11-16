using System.Text.Json.Serialization;

namespace PinkSea.Lexicons.Procedures;

/// <summary>
/// Begins the login flow.
/// </summary>
public class BeginLoginFlowProcedureRequest
{
    /// <summary>
    /// The handle.
    /// </summary>
    [JsonPropertyName("handle")]
    public required string Handle { get; set; }
    
    /// <summary>
    /// The redirect url for the code.
    /// </summary>
    [JsonPropertyName("redirectUrl")]
    public required string RedirectUrl { get; set; }
    
    /// <summary>
    /// The password.
    /// </summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; }
}