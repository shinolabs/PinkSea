using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.Services;

namespace PinkSea.Controllers;

[Route("/oauth")]
public class OAuthController(SigningKeyService signingKeyService) : Controller
{
    [Route("client-metadata.json")]
    public async Task<ActionResult<ClientMetadata>> ClientMetadata()
    {
        const string baseUrl = "https://237bb8170e6e72.lhr.life";
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
        key.X = Base64UrlEncoder.Encode(signingKeyService.KeyParameters.Q.X!);
        key.Y = Base64UrlEncoder.Encode(signingKeyService.KeyParameters.Q.Y!);
        key.KeyOps.Add("verify");
        key.KeyId = signingKeyService.KeyId;
        set.Keys.Add(key);
        
        return Ok(set);
    }
}