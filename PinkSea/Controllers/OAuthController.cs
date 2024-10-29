using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PinkSea.AtProto.Lexicons.Types;
using PinkSea.AtProto.Lexicons.Bluesky.Records;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Xrpc;
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
    IXrpcClientFactory xrpcClientFactory) : Controller
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
        var authorizationServer = await oAuthClient.GetOAuthRequestUriForHandle(handle);
        
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
        var token = await oAuthClient.CompleteAuthorization(
            state,
            code);

        if (!token)
            return BadRequest();

        using var xrpcClient = await xrpcClientFactory.GetForOAuthStateId(state);
        var profile = await xrpcClient!.Query<Record<Profile>>(
            "com.atproto.repo.getRecord",
            new
            {
                Repo = "did:plc:xtzdbt3kmdb6ef3wumhkhktd",
                Collection = "app.bsky.actor.profile",
                Rkey = "self",
            });
        
        return Content($@"
<h1>Hi {profile!.Value.DisplayName}!</h1>
<b>{profile.Value.Description}</b>
<p>This is your avatar: <img src=""https://porcini.us-east.host.bsky.network/xrpc/com.atproto.sync.getBlob?did=did:plc:xtzdbt3kmdb6ef3wumhkhktd&cid={profile.Value.Avatar!.Reference.Link}"" /></p>
", "text/html");
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