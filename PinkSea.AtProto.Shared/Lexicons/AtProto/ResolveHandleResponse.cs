using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons.AtProto;

/// <summary>
/// The response for the "com.atproto.identity.resolveHandle" request.
/// </summary>
public class ResolveHandleResponse
{
    /// <summary>
    /// The DID of the handle.
    /// </summary>
    [JsonPropertyName("did")]
    public required string Did { get; set; }
}