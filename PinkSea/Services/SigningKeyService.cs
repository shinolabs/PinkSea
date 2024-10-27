using System.IdentityModel.Tokens.Jwt;
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

    /// <summary>
    /// The key id of this key.
    /// </summary>
    public string KeyId
    {
        get
        {
            return Base64UrlEncoder.Encode(SecurityKey.ComputeJwkThumbprint());
        }
    }

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
            Issuer = "https://237bb8170e6e72.lhr.life/oauth/client-metadata.json",
            Audience = audience,
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            Claims = new Dictionary<string, object>()
            {
                { "sub", "https://237bb8170e6e72.lhr.life/oauth/client-metadata.json" },
                { "jti", DateTime.Now.ToString() }
            },
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.EcdsaSha256)
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateJwtSecurityToken(descriptor);

        token.Header.Add("kid", KeyId);
        return handler.WriteToken(token);
    }
}