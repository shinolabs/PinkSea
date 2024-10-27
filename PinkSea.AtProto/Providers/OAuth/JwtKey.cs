using Microsoft.IdentityModel.Tokens;

namespace PinkSea.AtProto.Providers.OAuth;

/// <summary>
/// The key used for signing the jwt.
/// </summary>
public class JwtKey
{
    /// <summary>
    /// The id of this key.
    /// </summary>
    public required string KeyId { get; set; }
    
    /// <summary>
    /// The signing credentials used.
    /// </summary>
    public required SigningCredentials SigningCredentials { get; set; }
}