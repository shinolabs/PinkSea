using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons.AtProto;

/// <summary>
/// The "com.atproto.identity.resolveHandle" request.
/// Resolves an atproto handle (hostname) to a DID. Does not necessarily bi-directionally verify against the the DID document.
/// </summary>
public class ResolveHandleRequest
{
    /// <summary>
    /// The handle to resolve.
    /// </summary>
    [JsonPropertyName("handle")]
    public required string Handle { get; set; }
}