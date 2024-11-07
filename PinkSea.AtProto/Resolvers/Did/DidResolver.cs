using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using PinkSea.AtProto.Models.Did;

namespace PinkSea.AtProto.Resolvers.Did;

/// <summary>
/// A generic DID resolver.
/// </summary>
public class DidResolver(
    IHttpClientFactory clientFactory,
    IMemoryCache memoryCache) : IDidResolver
{
    /// <inheritdoc />
    public async Task<DidResponse?> GetDidResponseForDid(string did)
    {
        return await memoryCache.GetOrCreateAsync(
            $"did:{did}",
            async e =>
            {
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return await ResolveDidViaPlcDirectory(did);
            });
    }

    /// <inheritdoc />
    public async Task<string?> GetHandleFromDid(string did)
    {
        return await memoryCache.GetOrCreateAsync(
            $"did:handle:{did}",
            async e =>
            {
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                var didDocument = await GetDidResponseForDid(did);
                return didDocument?.AlsoKnownAs[0]
                    .Replace("at://", "");
            });
    }

    /// <summary>
    /// Resolves a DID via the PLC directory.
    /// </summary>
    /// <param name="did">The DID.</param>
    /// <returns>The response, if it exists.</returns>
    private async Task<DidResponse?> ResolveDidViaPlcDirectory(
        string did)
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