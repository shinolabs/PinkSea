using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PinkSea.AtProto.Shared.Models.Did;

namespace PinkSea.AtProto.Resolvers.Did;

/// <summary>
/// A generic DID resolver.
/// </summary>
public class DidResolver(
    IHttpClientFactory clientFactory,
    IMemoryCache memoryCache,
    ILogger<DidResolver> logger) : IDidResolver
{
    /// <inheritdoc />
    public async Task<DidDocument?> GetDocumentForDid(string did)
    {
        return await memoryCache.GetOrCreateAsync(
            $"did:{did}",
            async e =>
            {
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return await ResolveDid(did);
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
                var didDocument = await GetDocumentForDid(did);
                return didDocument?.GetHandle();
            });
    }

    /// <summary>
    /// Resolves a DID.
    /// </summary>
    /// <param name="did">The DID.</param>
    /// <returns>The response, if it exists.</returns>
    private async Task<DidDocument?> ResolveDid(
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

        var segments = uri.Segments[0].Split(':');
        
        return segments[0] switch
        {
            "plc" => await ResolveDidViaPlcDirectory(did),
            "web" => await ResolveDidViaWeb(segments[1]),
            _ => null
        };
    }

    /// <summary>
    /// Resolves the DID using the did:web method.
    /// </summary>
    /// <param name="domain">The domain.</param>
    /// <returns>The did response.</returns>
    private async Task<DidDocument?> ResolveDidViaWeb(string domain)
    {
        const string wellKnownUri = $"/.well-known/did.json";

        try
        {
            using var client = clientFactory.CreateClient();
            return await client.GetFromJsonAsync<DidDocument>($"https://{domain}{wellKnownUri}");
        }
        catch (HttpRequestException e)
        {
            logger.LogError(e, $"Encountered an error while resolving did:web:{domain}.");
            return null;
        }
    }

    /// <summary>
    /// Resolves a DID using the did:plc method.
    /// </summary>
    /// <param name="did">The DID.</param>
    /// <returns>The did response.</returns>
    private async Task<DidDocument?> ResolveDidViaPlcDirectory(string did)
    {
        try
        {
            using var client = clientFactory.CreateClient("did-resolver");
            return await client.GetFromJsonAsync<DidDocument>($"/{did}");
        }
        catch (HttpRequestException e)
        {
            logger.LogError(e, $"Encountered an error while resolving {did}.");
            return null;
        }
    }
}