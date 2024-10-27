using Microsoft.AspNetCore.Mvc;
using PinkSea.AtProto.Models.OAuth;

namespace PinkSea.Controllers;

[Route("/oauth")]
public class OAuthController : Controller
{
    [Route("client-metadata.json")]
    public async Task<ActionResult<ClientMetadata>> ClientMetadata()
    {
        return Ok(new ClientMetadata
        {
            ClientId = "https://cf749a34717b65.lhr.life/oauth/client-metadata.json",
            ClientName = "PinkSea",
            ApplicationType = "native",
            DpopBoundAccessTokens = true,
            GrantTypes =
            [
                "authorization_code"
            ],
            RedirectUris =
            [
                "https://cf749a34717b65.lhr.life/oauth/callback"
            ],
            Scope = "atproto"
        });
    }
}