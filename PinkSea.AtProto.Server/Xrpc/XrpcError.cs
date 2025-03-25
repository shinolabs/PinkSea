using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Server.Xrpc;

/// <summary>
/// Simple model describing an XRPC error.
/// </summary>
public class XrpcError
{
    /// <summary>
    /// The error type.
    /// </summary>
    [JsonPropertyName("error")]
    public required string Error { get; set; }
    
    /// <summary>
    /// A human readable message.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}