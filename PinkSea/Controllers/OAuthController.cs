using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PinkSea.AtProto.Authorization;
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
    IOAuthStateStorageProvider oAuthStateStorageProvider,
    IAtProtoAuthorizationService atProtoAuthorizationService) : ControllerBase
{
    /// <summary>
    /// Begins the OAuth login flow.
    /// </summary>
    /// <param name="handle">The handle of the user wanting to log in.</param>
    /// <param name="redirectUrl">The final redirect url.</param>
    /// <param name="password">The optional password, for the session login flow.</param>
    /// <returns>A redirect.</returns>
    [Route("login")]
    public async Task<IActionResult> BeginLogin(
        [FromQuery] string handle,
        [FromQuery] string redirectUrl,
        [FromQuery] string? password)
    {
        var normalizedHandle = handle.TrimStart('@')
            .ToLower();

        // If we don't have a domain, that is, we don't have a '.' in the name
        // let's just assume '.bsky.social' at the end.
        // Usually people with custom domains don't do that.
        if (!normalizedHandle.Contains('.'))
            normalizedHandle += ".bsky.social";

        if (password is not null)
        {
            var authorized = await atProtoAuthorizationService.LoginWithPassword(handle, password);
            if (authorized is not null)
                return Redirect($"{redirectUrl}?code={authorized}");

            return BadRequest();
        }
        
        var authorizationServer = await oAuthClient.BeginOAuthFlow(
            normalizedHandle,
            redirectUrl);
        
        if (authorizationServer is null)
            return BadRequest();
        
        return Redirect(authorizationServer);
    }

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
    /// Invalidates the current session.
    /// </summary>
    [Route("invalidate")]
    public async Task<IActionResult> Invalidate([FromQuery] string code)
    {
        await oAuthStateStorageProvider.DeleteForStateId(code);
        return NoContent();
    }
    
    /// <summary>
    /// Returns the client metadata.
    /// </summary>
    /// <returns>The client metadata.</returns>
    [Route("client-metadata.json")]
    public async Task<ActionResult<ClientMetadata>> ClientMetadata()
    {
        return Ok(clientDataProvider.ClientMetadata);
    }

    /// <summary>
    /// Returns the possible JSON Web Keys.
    /// </summary>
    /// <returns>The JSON Web Keys.</returns>
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