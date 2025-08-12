using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PinkSea.Gateway.Lexicons;
using PinkSea.Gateway.Models;

namespace PinkSea.Gateway.Services;

/// <summary>
/// The meta tag generator service.
/// </summary>
public class MetaGeneratorService(
    PinkSeaQuery query,
    IOptions<GatewaySettings> options)
{
    /// <summary>
    /// Gets the meta tags for a given did/rkey pair.
    /// </summary>
    /// <param name="did">The DID.</param>
    /// <param name="rkey">The record key of the oekaki.</param>
    /// <returns>The formatted meta tags.</returns>
    public async Task<string> GetOekakiMetaFor(string did, string rkey)
    {
        var oekakiResponse = await query.GetOekaki(did, rkey);
        return oekakiResponse != null
            ? FormatOekakiResponse(oekakiResponse)
            : GetRegularMeta();
    }
    
    /// <summary>
    /// Gets the meta-tags for a given profile.
    /// </summary>
    /// <param name="did">The DID.</param>
    /// <returns>The formatted meta-tags.</returns>
    public async Task<string> GetProfileMetaFor(string did)
    {
        var profileResponse = await query.GetProfile(did);
        return profileResponse != null
            ? FormatProfileResponse(profileResponse)
            : GetRegularMeta();
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
        
        return $$"""
                 {{GenerateConfig()}}
                 <link rel="alternate" href="{{resp.Parent.AtProtoLink}}" />
                 <link href="{{options.Value.FrontEndEndpoint}}/ap/note.json?did={{resp!.Parent.Author.Did}}&rkey={{rkey}}" rel="alternate" type="application/activity+json" />
                 <meta name="application-name" content="PinkSea">
                 <meta name="generator" content="PinkSea.Gateway">
                 <meta property="og:site_name" content="PinkSea" />
                 <meta property="og:title" content="{{resp!.Parent.Author.Handle}}'s oekaki" />
                 <meta property="profile:username" content="@{{resp!.Parent.Author.Handle}}" />
                 <meta property="og:type" content="website" />
                 <meta property="og:url" content="{{options.Value.FrontEndEndpoint}}/{{resp.Parent.Author.Did}}/oekaki/{{rkey}}" />
                 <meta property="og:image" content="{{resp!.Parent.ImageLink}}" />
                 <meta property="og:description" content="{{resp!.Parent.Alt}}" />
                 <meta name="theme-color" content="#FFB6C1">
                 <meta name="twitter:card" content="summary_large_image">
                 """;
    }
    
    /// <summary>
    /// Formats a profile response.
    /// </summary>
    /// <param name="resp">The response.</param>
    /// <returns>The formatted profile response.</returns>
    private string FormatProfileResponse(GetProfileResponse resp)
    {
        var description = resp.Description ?? "This user has no description.";
        var avatarLink = resp.Avatar ?? $"{options.Value.FrontEndEndpoint}/assets/img/blank_avatar.png";
        return $"""
                {GenerateConfig()}
                <link rel="alternate" href="at://{resp.Did}/com.shinolabs.pinksea.profile/self" />
                <meta name="application-name" content="PinkSea">
                <meta name="generator" content="PinkSea.Gateway">
                <meta property="og:site_name" content="PinkSea" />
                <meta property="og:title" content="{resp.Nickname ?? resp.Handle}'s profile" />
                <meta property="og:type" content="website" />
                <meta property="og:url" content="{options.Value.FrontEndEndpoint}/{resp.Did}" />
                <meta property="og:image" content="{avatarLink}" />
                <meta property="og:description" content="{description}" />
                <meta name="theme-color" content="#FFB6C1">
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