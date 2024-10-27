using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using PinkSea.AtProto.Providers.OAuth;

namespace PinkSea.AtProto.OAuth;

public class JwtSigningProvider : IJwtSigningProvider
{
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
}