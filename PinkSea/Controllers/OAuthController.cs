using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.Services;

namespace PinkSea.Controllers;

[Route("/oauth")]
public class OAuthController(SigningKeyService signingKeyService) : Controller
{
    [Route("client-metadata.json")]
    public async Task<ActionResult<ClientMetadata>> ClientMetadata()
    {
        const string baseUrl = "https://012ce02769236b.lhr.life";
        return Ok(new ClientMetadata
        {
            ClientId = $"{baseUrl}/oauth/client-metadata.json",
            ClientName = "PinkSea",
            ApplicationType = "web",
            DpopBoundAccessTokens = true,
            GrantTypes =
            [
                "authorization_code"
            ],
            RedirectUris =
            [
                $"{baseUrl}/oauth/callback"
            ],
            Scope = "atproto transition:generic",
            TokenEndpointAuthMethod = "private_key_jwt",
            TokenEndpointAuthSigningAlgorithm = "ES256",
            JwksUri = $"{baseUrl}/oauth/jwks.json"
        });
    }

    [Route("jwks.json")]
    public async Task<ActionResult<JsonWebKeySet>> Jwks()
    {
        var set = new JsonWebKeySet();
        var key = JsonWebKeyConverter.ConvertFromECDsaSecurityKey(signingKeyService.SecurityKey);
        key.D = null;
        key.KeyOps.Add("verify");
        key.KeyId = Base64UrlEncoder.Encode(signingKeyService.KeyParameters.Q.Y!.ToString() + signingKeyService.KeyParameters.Q.X!.ToString());
        set.Keys.Add(key);
        
        return Ok(set);
    }
}