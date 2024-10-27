using System.Net.Http.Json;
using System.Web;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.Providers.OAuth;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Resolvers.Domain;

namespace PinkSea.AtProto.OAuth;

public class AtProtoOAuthClient(
    IHttpClientFactory httpClientFactory,
    IDomainDidResolver domainDidResolver,
    IDidResolver didResolver,
    IJwtSigningProvider jwtSigningProvider) : IAtProtoOAuthClient, IDisposable
{
    private readonly HttpClient _client = httpClientFactory.CreateClient("oauth-client");

    public async Task<string?> GetOAuthRequestUriForHandle(string handle, OAuthClientData clientData)
    {
        var did = await domainDidResolver.GetDidForDomainHandle(handle);
        if (did is null)
            return null;
        
        var resolved = await didResolver.GetDidResponseForDid(did!);
        var pds = resolved?.GetPds();
        if (pds is null)
            return null;
        
        var protectedResource = await GetOAuthProtectedResourceForPds(pds);
        var authServer = protectedResource?.GetAuthorizationServer();
        if (authServer is null)
            return null;

        var authorizationServer = await GetOAuthAuthorizationServerDataForAuthorizationServer(authServer);
        var assertion = jwtSigningProvider.GenerateClientAssertion(new JwtSigningData()
        {
            ClientId = clientData.ClientId,
            Audience = authorizationServer!.Issuer,
            Key = clientData.Key
        });
        
        var body = new AuthorizationRequest()
        {
            ClientId = clientData.ClientId,
            ResponseType = "code",
            Scope = clientData.Scope,
            RedirectUrl = clientData.RedirectUri,
            State = "sdsfsrewr",
            CodeChallenge = "a",
            CodeChallengeMethod = "S256",
            ClientAssertionType = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer",
            ClientAssertion = assertion
        };
        var resp = await _client.PostAsJsonAsync(authorizationServer!.PushedAuthorizationRequestEndpoint!, body);
        if (!resp.IsSuccessStatusCode)
            return null;

        var parResponse = await resp.Content.ReadFromJsonAsync<PushedAuthorizationRequestResponse>();

        var finalUrl = new UriBuilder(authorizationServer!.AuthorizationEndpoint!);
        var query = HttpUtility.ParseQueryString(finalUrl.Query);
        query["client_id"] = body.ClientId;
        query["request_uri"] = parResponse!.RequestUri;
        finalUrl.Query = query.ToString();
        
        return finalUrl.ToString();
    }

    public async Task<ProtectedResource?> GetOAuthProtectedResourceForPds(string pds)
    {
        const string wellKnownUrl = "/.well-known/oauth-protected-resource";
        return await _client.GetFromJsonAsync<ProtectedResource>($"{pds}{wellKnownUrl}");
    }

    public async Task<AuthorizationServer?> GetOAuthAuthorizationServerDataForAuthorizationServer(string authServer)
    {
        const string wellKnownUrl = "/.well-known/oauth-authorization-server";
        return await _client.GetFromJsonAsync<AuthorizationServer>($"{authServer}{wellKnownUrl}");
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}