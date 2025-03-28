using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Xrpc;

/// <summary>
/// Simple model describing an XRPC error.
/// </summary>
public sealed class XrpcError
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
    
    /// <summary>
    /// An optional status code, to override the status code within the XRPC server handler.
    /// </summary>
    [JsonIgnore]
    public int? StatusCode { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{Error}] > {Message}";
    }
}