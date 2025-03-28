using PinkSea.AtProto.Models.Authorization;

namespace PinkSea.AtProto.Models.OAuth;

/// <summary>
/// The OAuth state.
/// </summary>
public class OAuthState
{
    /// <summary>
    /// The current authorization type.
    /// </summary>
    public required AuthorizationType AuthorizationType { get; set; }
    
    /// <summary>
    /// The issuer.
    /// </summary>
    public required string Issuer { get; set; }
    
    /// <summary>
    /// The user DID.
    /// </summary>
    public required string Did { get; set; }
    
    /// <summary>
    /// The OAuth DPoP keypair.
    /// </summary>
    public required DpopKeyPair KeyPair { get; set; }
    
    /// <summary>
    /// The PKCE string.
    /// </summary>
    public required string PkceString { get; set; }
    
    /// <summary>
    /// The token endpoint.
    /// </summary>
    public required string TokenEndpoint { get; set; }
    
    /// <summary>
    /// The token revocation endpoint.
    /// </summary>
    public required string RevocationEndpoint { get; set; }
    
    /// <summary>
    /// The personal data server of the user.
    /// </summary>
    public required string Pds { get; set; }
    
    /// <summary>
    /// The authorization code.
    /// </summary>
    public string? AuthorizationCode { get; set; }
    
    /// <summary>
    /// The refresh token.
    /// </summary>
    public string? RefreshToken { get; set; }
    
    /// <summary>
    /// When does the token bound to the state expire?
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }
    
    /// <summary>
    /// The custom client-defined redirect url.
    /// </summary>
    public string? ClientRedirectUrl { get; set; }

    /// <summary>
    /// Check if this token has expired.
    /// </summary>
    /// <returns>Whether this token has expired.</returns>
    public bool HasExpired()
    {
        return ExpiresAt <= DateTimeOffset.UtcNow;
    }
}