using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.Providers;
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

    public async Task<string?> GetOAuthRequestUriForHandle(string handle, string redirectUrl)
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
        var assertion = jwtSigningProvider.GetToken(
            did,
            authorizationServer!.Issuer);
        var body = new AuthorizationRequest()
        {
            ClientId = "https://237bb8170e6e72.lhr.life/oauth/client-metadata.json",
            ResponseType = "code",
            Scope = "atproto transition:generic",
            RedirectUrl = "https://237bb8170e6e72.lhr.life/oauth/callback",
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