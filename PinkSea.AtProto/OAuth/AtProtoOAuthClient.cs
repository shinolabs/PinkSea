using System.Net.Http.Json;
using System.Text.Json;
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
            authorizationServer!.PushedAuthorizationRequestEndpoint!);
        var body = new AuthorizationRequest()
        {
            ClientId = "https://012ce02769236b.lhr.life/oauth/client-metadata.json",
            ResponseType = "code",
            Scope = "atproto transition:generic",
            RedirectUrl = "https://012ce02769236b.lhr.life/oauth/callback",
            State = "sdsfsrewr",
            CodeChallenge = "a",
            CodeChallengeMethod = "S256",
            ClientAssertionType = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer",
            ClientAssertion = assertion
        };
        var resp = await _client.PostAsJsonAsync(authorizationServer!.PushedAuthorizationRequestEndpoint!, body);
        return resp.ToString() + "\n" + resp.Content.ReadAsStringAsync().Result + "\n" + JsonSerializer.Serialize(body);
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