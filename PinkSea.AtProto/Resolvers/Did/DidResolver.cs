using System.Net.Http.Json;
using PinkSea.AtProto.Models.Did;

namespace PinkSea.AtProto.Resolvers.Did;

/// <summary>
/// A generic DID resolver.
/// </summary>
public class DidResolver(IHttpClientFactory clientFactory) : IDidResolver
{
    /// <inheritdoc />
    public async Task<DidResponse?> GetDidResponseForDid(string did)
    {
        Uri uri;
        try
        {
            uri = new Uri(did);
        }
        catch
        {
            return null;
        }
        
        if (uri.Scheme != "did")
            return null;

        // TODO: Web.
        var segments = uri.Segments[0].Split(':');
        if (segments[0] != "plc")
            return null;
        
        using var client = clientFactory.CreateClient("did-resolver");
        return await client.GetFromJsonAsync<DidResponse>($"/{did}");
    }
}