using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using PinkSea.AtProto.Helpers;
using PinkSea.AtProto.Lexicons.AtProto;
using PinkSea.AtProto.Models.Authorization;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.Providers.Storage;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Resolvers.Domain;

namespace PinkSea.AtProto.Authorization;

/// <summary>
/// The AT Protocol authorization service.
/// </summary>
public class AtProtoAuthorizationService(
    IDidResolver didResolver,
    IDomainDidResolver domainDidResolver,
    IOAuthStateStorageProvider oauthStateStorageProvider,
    IHttpClientFactory httpClientFactory,
    ILogger<AtProtoAuthorizationService> logger) : IAtProtoAuthorizationService
{
    /// <inheritdoc />
    public async Task<string?> LoginWithPassword(string handle, string password)
    {
        const string endpoint = "/xrpc/com.atproto.server.createSession";
        
        var identifier = handle.StartsWith("did")
            ? handle
            : await domainDidResolver.GetDidForDomainHandle(handle);

        if (identifier is null)
            return null;

        var didDocument = await didResolver.GetDidResponseForDid(identifier);
        if (didDocument is null)
            return null;

        var pds = didDocument.GetPds()!;
        using var httpClient = httpClientFactory.CreateClient();

        var resp = await httpClient.PostAsJsonAsync($"{pds}{endpoint}", new
        {
            identifier,
            password
        });

        if (!resp.IsSuccessStatusCode)
        {
            logger.LogError($"Failed login for {handle} with reason {await resp.Content.ReadAsStringAsync()}");
            return null;
        }

        var tokenResponse = await resp.Content.ReadFromJsonAsync<CreateSessionResponse>();
        if (tokenResponse is null || !tokenResponse.Active)
            return null;

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
            ExpiresAt = DateTimeOffset.UtcNow.AddYears(1)
        };

        var stateId = StateHelper.GenerateRandomState();
        await oauthStateStorageProvider.SetForStateId(stateId, oauthState);
        
        return stateId;
    }
}