using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.Providers.OAuth;

namespace PinkSea.AtProto.Tests.Providers.OAuth;

[TestFixture]
public class JwtSigningProviderTests
{
    private JwtSigningProvider _provider = null!;

    [SetUp]
    public void Setup()
    {
        _provider = new JwtSigningProvider();
    }

    [TearDown]
    public void Teardown()
    {
        _provider.Dispose();
    }

    private static ECDsaSecurityKey CreateEcdsaKey()
    {
        var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var key = new ECDsaSecurityKey(ecdsa)
        {
            KeyId = Guid.NewGuid().ToString("N")
        };
        return key;
    }

    [Test]
    public void GenerateClientAssertion_Produces_Valid_Jwt_With_Expected_Claims()
    {
        var key = CreateEcdsaKey();
        var creds = new SigningCredentials(key, SecurityAlgorithms.EcdsaSha256);
        var signingData = new JwtSigningData
        {
            ClientId = "client123",
            Audience = "https://issuer.example.com/token",
            Key = new JwtKey
            {
                SigningCredentials = creds,
                KeyId = key.KeyId
            }
        };

        var jwt = _provider.GenerateClientAssertion(signingData);

        Assert.That(jwt, Is.Not.Null.And.Not.Empty);
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        Assert.Multiple(() =>
        {
            Assert.That(token.Issuer, Is.EqualTo(signingData.ClientId));
            Assert.That(token.Audiences.First(), Is.EqualTo(signingData.Audience));
            Assert.That(token.Claims.Any(c => c.Type == "sub" && c.Value == "client123"), Is.True);
            Assert.That(token.Header["kid"], Is.EqualTo(key.KeyId));
            Assert.That(token.ValidTo, Is.GreaterThan(DateTime.UtcNow));
        });
    }

    [Test]
    public void GenerateDpopHeader_Produces_Jwt_With_Dpop_Type_And_Jwk()
    {
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var privPem = ExportPrivateKeyPem(ecdsa);
        var pubPem = ExportPublicKeyPem(ecdsa);

        var signingData = new DpopSigningData
        {
            ClientId = "client123",
            Method = "POST",
            Url = "https://api.example.com/token",
            Keypair = new DpopKeyPair
            {
                PrivateKey = privPem,
                PublicKey = pubPem
            },
            Nonce = "abc123",
            AuthenticationCodeHash = "xyz987"
        };

        var tokenStr = _provider.GenerateDpopHeader(signingData);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(tokenStr);
        Assert.Multiple(() =>
        {
            Assert.That(token.Header["typ"], Is.EqualTo("dpop+jwt"));
            Assert.That(token.Claims.Any(c => c.Type == "htm" && c.Value == "POST"));
            Assert.That(token.Claims.Any(c => c.Type == "htu" && c.Value == "https://api.example.com/token"));
            Assert.That(token.Claims.Any(c => c.Type == "nonce" && c.Value == "abc123"));
            Assert.That(token.Claims.Any(c => c.Type == "ath" && c.Value == "xyz987"));
            Assert.That(token.Header.ContainsKey("jwk"), Is.True);
        });

        var jwkJson = token.Header["jwk"]!.ToString();
        using var doc = JsonDocument.Parse(jwkJson);
        var root = doc.RootElement;
        Assert.Multiple(() =>
        {
            Assert.That(root.GetProperty("kty").GetString(), Is.EqualTo("EC"));
            Assert.That(root.TryGetProperty("x", out _), Is.True);
            Assert.That(root.TryGetProperty("y", out _), Is.True);
        });
    }

    [Test]
    public void Dispose_Does_Not_Throw()
    {
        Assert.DoesNotThrow(() => _provider.Dispose());
    }

    private static string ExportPrivateKeyPem(ECDsa key)
    {
        var sb = new StringBuilder();
        sb.AppendLine("-----BEGIN PRIVATE KEY-----");
        sb.AppendLine(Convert.ToBase64String(key.ExportPkcs8PrivateKey(), Base64FormattingOptions.InsertLineBreaks));
        sb.AppendLine("-----END PRIVATE KEY-----");
        return sb.ToString();
    }

    private static string ExportPublicKeyPem(ECDsa key)
    {
        var sb = new StringBuilder();
        sb.AppendLine("-----BEGIN PUBLIC KEY-----");
        sb.AppendLine(Convert.ToBase64String(key.ExportSubjectPublicKeyInfo(), Base64FormattingOptions.InsertLineBreaks));
        sb.AppendLine("-----END PUBLIC KEY-----");
        return sb.ToString();
    }
}