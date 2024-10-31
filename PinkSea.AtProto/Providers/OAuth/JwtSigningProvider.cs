using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace PinkSea.AtProto.Providers.OAuth;

public class JwtSigningProvider : IJwtSigningProvider, IDisposable
{
    /// <summary>
    /// The ECDSA used for this JWT signing provider.
    /// </summary>
    private readonly ECDsa _ecdsa = ECDsa.Create(ECCurve.CreateFromFriendlyName("nistp256"));
    
    /// <inheritdocs />
    public string GenerateClientAssertion(JwtSigningData signingData)
    {
        var descriptor = new SecurityTokenDescriptor()
        {
            Issuer = signingData.ClientId,
            Audience = signingData.Audience,
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            Claims = new Dictionary<string, object>()
            {
                { "sub", signingData.ClientId },
                { "jti", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() }
            },
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = signingData.Key.SigningCredentials
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateJwtSecurityToken(descriptor);

        token.Header.Add("kid", signingData.Key.KeyId);
        return handler.WriteToken(token);
    }

    /// <inheritdocs />
    public string GenerateDpopHeader(DpopSigningData signingData)
    {
        _ecdsa.ImportFromPem(signingData.Keypair.PublicKey);
        _ecdsa.ImportFromPem(signingData.Keypair.PrivateKey);

        var secKey = new ECDsaSecurityKey(_ecdsa);
        var parameters = _ecdsa.ExportParameters(false);
        
        var descriptor = new SecurityTokenDescriptor()
        {
            Issuer = signingData.ClientId,
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            Claims = new Dictionary<string, object>()
            {
                { "jti", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() },
                { "htm", signingData.Method },
                { "htu", signingData.Url }
            },
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = new SigningCredentials(secKey, SecurityAlgorithms.EcdsaSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            }
        };

        if (signingData.Nonce is not null)
            descriptor.Claims.Add("nonce", signingData.Nonce);
        
        if (signingData.AuthenticationCodeHash is not null)
            descriptor.Claims.Add("ath", signingData.AuthenticationCodeHash);
        
        var key = JsonWebKeyConverter.ConvertFromECDsaSecurityKey(secKey);
        key.D = null;
        key.X = Base64UrlEncoder.Encode(parameters.Q.X!);
        key.Y = Base64UrlEncoder.Encode(parameters.Q.Y!);
        key.KeyOps.Add("verify");
        key.KeyId = secKey.KeyId;
        
        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateJwtSecurityToken(descriptor);
        token.Header.Add("jwk", JsonSerializer.SerializeToElement(key));
        token.Header.Remove("typ");
        token.Header.Add("typ", "dpop+jwt");
        var tokenStr = handler.WriteToken(token);

        return tokenStr;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _ecdsa.Dispose();
    }
}