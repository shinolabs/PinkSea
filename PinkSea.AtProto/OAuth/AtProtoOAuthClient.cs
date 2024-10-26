using System.Net.Http.Json;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Resolvers.Domain;

namespace PinkSea.AtProto.OAuth;

public class AtProtoOAuthClient(
    IHttpClientFactory httpClientFactory,
    IDomainDidResolver domainDidResolver,
    IDidResolver didResolver) : IAtProtoOAuthClient, IDisposable
{
    private readonly HttpClient _client = httpClientFactory.CreateClient("oauth-client");

    public async Task<string?> GetOAuthRequestUriForHandle(string handle)
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
        return authorizationServer?.AuthorizationEndpoint;
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