using Microsoft.Extensions.Caching.Memory;
using PinkSea.Gateway.Lexicons;

namespace PinkSea.Gateway.Services;

/// <summary>
/// A helper service for querying the PinkSea appview.
/// </summary>
public class PinkSeaQuery(
    IHttpClientFactory httpClientFactory,
    IMemoryCache memoryCache)
{
    /// <summary>
    /// Gets a profile for a given DID.
    /// </summary>
    /// <param name="did">The DID.</param>
    /// <returns>The profile, if found.</returns>
    public async Task<GetProfileResponse?> GetProfile(string did)
    {
        const int cacheExpiry = 30;
        const int cacheExpiryWhenFailed = 1;
        const string endpointTemplate = "/xrpc/com.shinolabs.pinksea.getProfile?did={0}";
        
        return await memoryCache.GetOrCreateAsync<GetProfileResponse?>($"profile:{did}",
            async cacheEntry =>
            {
                using var client = httpClientFactory.CreateClient("pinksea-xrpc");
                try
                {
                    did = await EnsureDid(did);
                    var resp = await client.GetFromJsonAsync<GetProfileResponse>(string.Format(endpointTemplate, did));
                
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheExpiry);

                    return resp;
                }
                catch (HttpRequestException)
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheExpiryWhenFailed);
                    return null;
                }
            })!;
    }

    /// <summary>
    /// Gets a possible parent for a given oekaki.
    /// </summary>
    /// <param name="did">The DID of the author.</param>
    /// <param name="rkey">The record key of the oekaki.</param>
    /// <returns>The possible parent for the oekaki.</returns>
    public async Task<GetParentForReplyResponse?> GetPossibleParentForOekaki(string did, string rkey)
    {
        const int cacheExpiry = 30;
        const int cacheExpiryWhenFailed = 1;
        const string endpointTemplate = "/xrpc/com.shinolabs.pinksea.getParentForReply?did={0}&rkey={1}";
        
        return await memoryCache.GetOrCreateAsync<GetParentForReplyResponse?>($"parent:{did}:{rkey}",
            async cacheEntry =>
            {
                using var client = httpClientFactory.CreateClient("pinksea-xrpc");
                try
                {
                    did = await EnsureDid(did);
                    var resp = await client.GetFromJsonAsync<GetParentForReplyResponse>(string.Format(endpointTemplate, did, rkey));
                
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheExpiry);

                    return resp;
                }
                catch
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheExpiryWhenFailed);
                    return null;
                }
            })!;
    }

    /// <summary>
    /// Gets the oekaki data for a given DID and record key.
    /// </summary>
    /// <param name="did">The DID of the author.</param>
    /// <param name="rkey">The record key of the oekaki.</param>
    /// <returns>The oekaki, if found.</returns>
    public async Task<GetOekakiResponse?> GetOekaki(string did, string rkey)
    {
        const int cacheExpiry = 30;
        const int cacheExpiryWhenFailed = 1;
        const string endpointTemplate = "/xrpc/com.shinolabs.pinksea.getOekaki?did={0}&rkey={1}";
        
        return await memoryCache.GetOrCreateAsync<GetOekakiResponse?>($"{did}:{rkey}",
            async cacheEntry =>
            {
                using var client = httpClientFactory.CreateClient("pinksea-xrpc");
                try
                {
                    did = await EnsureDid(did);
                    var resp = await client.GetFromJsonAsync<GetOekakiResponse>(string.Format(endpointTemplate, did, rkey));
                
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheExpiry);

                    return resp;
                }
                catch (HttpRequestException)
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheExpiryWhenFailed);
                    return null;
                }
            })!;
    }

    /// <summary>
    /// Ensures that the given string is a DID, trying to resolve it as a handle if not.
    /// </summary>
    /// <param name="did">The DID or a handle.</param>
    /// <returns>The resulting DID.</returns>
    private async Task<string> EnsureDid(string did)
    {
        if (did.StartsWith("did:", StringComparison.OrdinalIgnoreCase))
            return did;
        
        const string endpointTemplate = "/xrpc/com.atproto.identity.resolveHandle?handle={0}";

        using var client = httpClientFactory.CreateClient("pinksea-xrpc");
        try
        {
            var resp = await client.GetFromJsonAsync<ResolveHandleResponse>(string.Format(endpointTemplate, did));
            return resp!.Did;
        }
        catch
        {
            return did;
        }
    }
}