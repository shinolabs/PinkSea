using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using PinkSea.AtProto.Providers;

namespace PinkSea.Services;

public class SigningKeyService : IJwtSigningProvider
{
    /// <summary>
    /// The current elliptic curve used.
    /// </summary>
    public ECDsa Curve { get; private set; } = null!;
    
    /// <summary>
    /// The current key parameters for the curve.
    /// </summary>
    public ECParameters KeyParameters { get; private set; }
    
    /// <summary>
    /// The current security key.
    /// </summary>
    public ECDsaSecurityKey SecurityKey { get; private set; } = null!;
    
    public SigningKeyService()
    {
        GenerateKeyPair();
    }

    /// <summary>
    /// Generates an ECDSA keypair.
    /// </summary>
    private void GenerateKeyPair()
    {
        Curve = ECDsa.Create(ECCurve.CreateFromFriendlyName("nistp256"));
        SecurityKey = new ECDsaSecurityKey(Curve);
        KeyParameters = Curve.ExportParameters(true);
    }

    /// <inheritdocs />
    public string GetToken(
        string did,
        string audience)
    {
        var descriptor = new SecurityTokenDescriptor()
        {
            Issuer = "https://012ce02769236b.lhr.life/oauth/client-metadata.json",
            Audience = audience,
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.EcdsaSha256)
        };

        var handler = new JsonWebTokenHandler
        {
            SetDefaultTimesOnTokenCreation = false
        };
        return handler.CreateToken(descriptor);
    }
}