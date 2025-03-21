using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PinkSea.Gateway.Lexicons;
using PinkSea.Gateway.Models;

namespace PinkSea.Gateway.Services;

/// <summary>
/// The meta tag generator service.
/// </summary>
public class MetaGeneratorService(
    IHttpClientFactory httpClientFactory,
    IMemoryCache memoryCache,
    IOptions<GatewaySettings> options)
{
    /// <summary>
    /// Gets the meta tags for a given did/rkey pair.
    /// </summary>
    /// <param name="did">The DID.</param>
    /// <param name="rkey">The record key of the oekaki.</param>
    /// <returns>The formatted meta tags.</returns>
    public Task<string> GetOekakiMetaFor(string did, string rkey)
    {
        const int cacheExpiry = 30;
        const int cacheExpiryWhenFailed = 1;
        const string endpointTemplate = "/xrpc/com.shinolabs.pinksea.getOekaki?did={0}&rkey={1}";
        
        return memoryCache.GetOrCreateAsync<string>($"{did}:{rkey}",
            async cacheEntry =>
            {
                using var client = httpClientFactory.CreateClient("pinksea-xrpc");
                try
                {
                    var resp = await client.GetFromJsonAsync<GetOekakiResponse>(string.Format(endpointTemplate, did, rkey));
                
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheExpiry);
                
                    return resp is not null
                        ? FormatOekakiResponse(resp)
                        : "";
                }
                catch (HttpRequestException)
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheExpiryWhenFailed);
                    return GetRegularMeta();
                }
            })!;
    }

    /// <summary>
    /// Gets the regular meta tags.
    /// </summary>
    /// <returns>The meta tags.</returns>
    public string GetRegularMeta()
    {
        return $"""
                {GenerateConfig()}
                <meta name="application-name" content="PinkSea">
                <meta name="generator" content="PinkSea.Gateway">
                <meta property="og:site_name" content="PinkSea" />
                <meta property="og:title" content="oekaki BBS" />
                <meta property="og:type" content="website" />
                <meta property="og:image" content="{options.Value.FrontEndEndpoint}/logo.png" />
                <meta property="og:description" content="PinkSea is an Oekaki BBS running as an AT Protocol application. Log in and draw!" />
                <meta name="theme-color" content="#FFB6C1">
                """;
    }

    /// <summary>
    /// Formats an oekaki response.
    /// </summary>
    /// <param name="resp">The response.</param>
    /// <returns>The formatted oekaki response.</returns>
    private string FormatOekakiResponse(GetOekakiResponse resp)
    {
        var rkey = resp.Parent
            .AtProtoLink
            .Split('/')
            .Last();
        
        return $"""
                {GenerateConfig()}
                <link rel="alternate" href="{resp.Parent.AtProtoLink}" />
                <meta name="application-name" content="PinkSea">
                <meta name="generator" content="PinkSea.Gateway">
                <meta property="og:site_name" content="PinkSea" />
                <meta property="og:title" content="{resp!.Parent.Author.Handle}'s oekaki" />
                <meta property="og:type" content="website" />
                <meta property="og:url" content="{options.Value.FrontEndEndpoint}/{resp.Parent.Author.Did}/oekaki/{rkey}" />
                <meta property="og:image" content="{resp!.Parent.ImageLink}" />
                <meta property="og:description" content="{resp!.Parent.Alt}" />
                <meta name="theme-color" content="#FFB6C1">
                <meta name="twitter:card" content="summary_large_image">
                """;
    }

    /// <summary>
    /// Generates the config for the appview endpoint.
    /// </summary>
    /// <returns>The config for the appview endpoint.</returns>
    private string GenerateConfig()
    {
        return $@"
<script>
    window.pinkSeaConfig = {{
        apiUrl: ""{options.Value.AppViewEndpoint}""
    }}
</script>
";
    }
}