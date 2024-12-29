using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Providers.Storage;
using PinkSea.Services;

namespace PinkSea.Controllers;

/// <summary>
/// Controller for the OAuth methods.
/// </summary>
[Route("/oauth")]
[ApiController]
public class OAuthController(
    SigningKeyService signingKeyService,
    IAtProtoOAuthClient oAuthClient,
    IOAuthClientDataProvider clientDataProvider,
    IOAuthStateStorageProvider oAuthStateStorageProvider) : ControllerBase
{
    /// <summary>
    /// The OAuth callback.
    /// </summary>
    /// <param name="iss">The issuer.</param>
    /// <param name="state">The state.</param>
    /// <param name="code">The code.</param>
    [Route("callback")]
    public async Task<IActionResult> Callback(
        [FromQuery] string iss,
        [FromQuery] string state,
        [FromQuery] string code)
    {
        var oauthState = await oAuthStateStorageProvider.GetForStateId(state);

        if (oauthState is null)
            return Unauthorized();
        
        var token = await oAuthClient.CompleteAuthorization(
            state,
            code);

        if (!token)
            return BadRequest();

        if (oauthState.ClientRedirectUrl is not null)
            return Redirect($"{oauthState.ClientRedirectUrl}?code={state}");
        
        return Ok(state);
    }

    /// <summary>
    /// Returns the client metadata.
    /// </summary>
    /// <returns>The client metadata.</returns>
    [Route("client-metadata.json")]
    public ActionResult<ClientMetadata> ClientMetadata()
    {
        return Ok(clientDataProvider.ClientMetadata);
    }

    /// <summary>
    /// Returns the possible JSON Web Keys.
    /// </summary>
    /// <returns>The JSON Web Keys.</returns>
    [Route("jwks.json")]
    public ActionResult<JsonWebKeySet> Jwks()
    {
        return Ok(signingKeyService.GetJsonWebKeySet());
    }
}