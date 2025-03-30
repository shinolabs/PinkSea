using System.Text.Json.Serialization;

namespace PinkSea.AtProto.OAuth.Models;

/// <summary>
/// An OAuth error.
/// </summary>
public class OAuthError
{
    /// <summary>
    /// The error type.
    /// </summary>
    [JsonPropertyName("error")]
    public required string Error { get; set; }
    
    /// <summary>
    /// A human readable message.
    /// </summary>
    [JsonPropertyName("error_description")]
    public string? Message { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{Error}] > {Message}";
    }
}