using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using PinkSea.AtProto.Providers.OAuth;

namespace PinkSea.Services;

public class SigningKeyService
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
}