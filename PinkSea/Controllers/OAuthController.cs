using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
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
public class OAuthController(
    SigningKeyService signingKeyService,
    IAtProtoOAuthClient oAuthClient,
    IOAuthClientDataProvider clientDataProvider,
    IOAuthStateStorageProvider oAuthStateStorageProvider) : Controller
{
    /// <summary>
    /// Begins the OAuth login flow.
    /// </summary>
    /// <param name="handle">The handle of the user wanting to log in.</param>
    /// <returns>A redirect.</returns>
    [Route("begin-login")]
    public async Task<IActionResult> BeginLogin(
        [FromQuery] string handle)
    {
        var authorizationServer = await oAuthClient.BeginOAuthFlow(handle);
        
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
            return BadRequest();
        
        var token = await oAuthClient.CompleteAuthorization(
            state,
            code);

        if (!token)
            return BadRequest();

        var claims = new List<Claim>()
        {
            new("state", state),
            new("did", oauthState.Did),
            new("pds", oauthState.Pds)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "PinkSea");

        await HttpContext.SignInAsync(
            "PinkSea",
            new ClaimsPrincipal(claimsIdentity),
            new AuthenticationProperties());

        return Redirect("/");
    }
    
    /// <summary>
    /// Invalidates the current session.
    /// </summary>
    /// <returns>A redirect.</returns>
    [Route("invalidate")]
    public async Task<IActionResult> Invalidate()
    {
        await HttpContext.SignOutAsync("PinkSea");
        return Redirect("/");
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