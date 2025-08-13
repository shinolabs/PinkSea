using Microsoft.Extensions.Options;
using PinkSea.Gateway.Models;
using PinkSea.Gateway.OEmbed;

namespace PinkSea.Gateway.Services;

/// <summary>
/// Renders a PinkSea oekaki as an OEmbed 1.0 document.
/// </summary>
public class OEmbedRenderer(
    PinkSeaQuery query,
    IOptions<GatewaySettings> options)
{
    /// <summary>
    /// Renders an oekaki as an oembed document.
    /// </summary>
    /// <param name="did">The DID of the author.</param>
    /// <param name="rkey">The record key of the oekaki.</param>
    /// <returns>The OEmbed document, if applicable.</returns>
    public async Task<OEmbedResponse?> RenderOEmbedForOekaki(string did, string rkey)
    {
        var oekakiResponse = await query.GetOekaki(did, rkey);
        if (oekakiResponse is null)
            return null;

        return new OEmbedResponse
        {
            Type = "photo",
            Title = oekakiResponse.Parent.Alt,
            Url = oekakiResponse.Parent.ImageLink,
            Width = 400,
            Height = 400,
            AuthorName = oekakiResponse.Parent.Author.Handle,
            AuthorUrl = $"{options.Value.FrontEndEndpoint}/{did}",
            ProviderName = "PinkSea",
            ProviderUrl = options.Value.FrontEndEndpoint
        };
    }
}