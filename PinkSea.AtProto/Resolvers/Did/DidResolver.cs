using System.Net.Http.Json;
using PinkSea.AtProto.Models.Did;

namespace PinkSea.AtProto.Resolvers.Did;

public class DidResolver(IHttpClientFactory clientFactory) : IDidResolver
{
    public async Task<DidResponse?> GetDidResponseForDid(string did)
    {
        // TODO: Caching and validation.
        using var client = clientFactory.CreateClient("did-resolver");
        return await client.GetFromJsonAsync<DidResponse>($"/{did}");
    }
}