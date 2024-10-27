namespace PinkSea.AtProto.Providers.OAuth;

/// <summary>
/// Data for signing a JWT.
/// </summary>
public class JwtSigningData
{
    /// <summary>
    /// The client id.
    /// </summary>
    public required string ClientId { get; set; }
    
    /// <summary>
    /// The audience.
    /// </summary>
    public required string Audience { get; set; }
    
    /// <summary>
    /// The signing key.
    /// </summary>
    public required JwtKey Key { get; set; }
}