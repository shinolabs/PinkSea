namespace PinkSea.AtProto.Models.OAuth;

/// <summary>
/// The OAuth state.
/// </summary>
public class OAuthState
{
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
    /// The personal data server of the user.
    /// </summary>
    public required string Pds { get; set; }
    
    /// <summary>
    /// The authorization code.
    /// </summary>
    public string? AuthorizationCode { get; set; }
    
    /// <summary>
    /// The custom client-defined redirect url.
    /// </summary>
    public string? ClientRedirectUrl { get; set; }
}