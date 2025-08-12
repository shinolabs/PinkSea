using Microsoft.Extensions.Caching.Memory;
using PinkSea.Gateway.Lexicons;

namespace PinkSea.Gateway.Services;

public class PinkSeaQuery(
    IHttpClientFactory httpClientFactory,
    IMemoryCache memoryCache)
{
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
}