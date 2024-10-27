using PinkSea.AtProto.Providers.OAuth;

namespace PinkSea.AtProto.OAuth;

/// <summary>
/// The OAuth client data.
/// </summary>
public class OAuthClientData
{
    /// <summary>
    /// The client id.
    /// </summary>
    public required string ClientId { get; set; }
    
    /// <summary>
    /// The redirect URI.
    /// </summary>
    public required string RedirectUri { get; set; }
    
    /// <summary>
    /// The key used for signing JWTs.
    /// </summary>
    public required JwtKey Key { get; set; }

    /// <summary>
    /// The scope to authenticate with.
    /// </summary>
    public string Scope { get; set; } = "atproto transition:generic";
}