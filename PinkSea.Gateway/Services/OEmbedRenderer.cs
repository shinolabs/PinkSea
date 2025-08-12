using Microsoft.Extensions.Options;
using PinkSea.Gateway.Models;
using PinkSea.Gateway.OEmbed;

namespace PinkSea.Gateway.Services;

public class OEmbedRenderer(
    PinkSeaQuery query,
    IOptions<GatewaySettings> options)
{
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