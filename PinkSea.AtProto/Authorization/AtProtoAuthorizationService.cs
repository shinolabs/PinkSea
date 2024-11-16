using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using PinkSea.AtProto.Helpers;
using PinkSea.AtProto.Lexicons.AtProto;
using PinkSea.AtProto.Models;
using PinkSea.AtProto.Models.Authorization;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.Providers.Storage;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Resolvers.Domain;
using PinkSea.AtProto.Xrpc.Client;

namespace PinkSea.AtProto.Authorization;

/// <summary>
/// The AT Protocol authorization service.
/// </summary>
public class AtProtoAuthorizationService(
    IDidResolver didResolver,
    IDomainDidResolver domainDidResolver,
    IOAuthStateStorageProvider oauthStateStorageProvider,
    IHttpClientFactory httpClientFactory,
    IXrpcClientFactory xrpcClientFactory,
    ILogger<AtProtoAuthorizationService> logger) : IAtProtoAuthorizationService
{
    /// <inheritdoc />
    public async Task<ErrorOr<string>> LoginWithPassword(string handle, string password)
    {
        const string endpoint = "/xrpc/com.atproto.server.createSession";
        
        var identifier = handle.StartsWith("did")
            ? handle
            : await domainDidResolver.GetDidForDomainHandle(handle);

        if (identifier is null)
            return ErrorOr<string>.Fail($"Could not resolve the DID for {handle}.");

        var didDocument = await didResolver.GetDidResponseForDid(identifier);
        if (didDocument is null)
            return ErrorOr<string>.Fail($"Could not fetch the DID document for {identifier}.");

        var pds = didDocument.GetPds()!;
        using var httpClient = httpClientFactory.CreateClient();

        var resp = await httpClient.PostAsJsonAsync($"{pds}{endpoint}", new
        {
            identifier,
            password
        });

        if (!resp.IsSuccessStatusCode)
        {
            var reason = await resp.Content.ReadAsStringAsync();
            logger.LogError($"Failed login for {handle} with reason {reason}");
            
            return ErrorOr<string>.Fail($"Got a non-OK response from your PDS {reason}.");
        }

        var tokenResponse = await resp.Content.ReadFromJsonAsync<CreateSessionResponse>();
        if (tokenResponse is null || !tokenResponse.Active)
            return ErrorOr<string>.Fail($"The password token is not active.");

        var jwt = new JwtSecurityTokenHandler();
        var jwtToken = jwt.ReadJwtToken(tokenResponse.AccessToken);

        var expiry = new DateTimeOffset(jwtToken.ValidTo)
            .UtcDateTime;
        
        var oauthState = new OAuthState
        {
            AuthorizationType = AuthorizationType.PdsSession,
            AuthorizationCode = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            Did = identifier,
            Pds = pds,
            PkceString = "",
            Issuer = "",
            TokenEndpoint = "",
            KeyPair = new DpopKeyPair()
            {
                PrivateKey = "",
                PublicKey = ""
            },
            ExpiresAt = expiry
        };

        var stateId = StateHelper.GenerateRandomState();
        await oauthStateStorageProvider.SetForStateId(stateId, oauthState);
        
        return ErrorOr<string>.Ok(stateId);
    }

    /// <inheritdoc />
    public async Task<bool> RefreshSession(string stateId)
    {
        using var xrpcClient = await xrpcClientFactory.GetForOAuthStateId(stateId);
        if (xrpcClient is null)
            return false;

        var resp = await xrpcClient.Procedure<CreateSessionResponse>("com.atproto.server.refreshSession");
        if (resp is null)
            return false;

        var jwt = new JwtSecurityTokenHandler();
        var jwtToken = jwt.ReadJwtToken(resp.AccessToken);

        var expiry = new DateTimeOffset(jwtToken.ValidTo)
            .UtcDateTime;
        
        var oauthState = await oauthStateStorageProvider.GetForStateId(stateId);
        oauthState!.AuthorizationCode = resp.AccessToken;
        oauthState.RefreshToken = resp.RefreshToken;
        oauthState.ExpiresAt = expiry;

        await oauthStateStorageProvider.SetForStateId(stateId, oauthState);

        return true;
    }
}