using Microsoft.Extensions.Options;
using PinkSea.Gateway.Models;
using PinkSea.Gateway.Models.Oekaki;
using PinkSea.Gateway.Models.Rss;
using PinkSea.Gateway.Models.Rss.Atom;

namespace PinkSea.Gateway.Services.Rss;

/// <summary>
/// The RSS syndication feed builder.
/// </summary>
public class SyndicationBuilderService(
    IHttpClientFactory httpClientFactory,
    IOptions<GatewaySettings> options)
{
    /// <summary>
    /// Builds a syndication feed for a given did.
    /// </summary>
    /// <param name="did">The DID of the user.</param>
    /// <returns>The RSS root.</returns>
    public async Task<RssRoot> BuildSyndicationFeedFor(string did)
    {
        const string endpointTemplate = "/xrpc/com.shinolabs.pinksea.getAuthorFeed?did={0}";

        using var client = httpClientFactory.CreateClient("pinksea-xrpc");
        var resp = await client.GetFromJsonAsync<AuthorFeedResponse>(string.Format(endpointTemplate, did));

        var items = resp?.Oekaki
            .Select(o => new RssItem
            {
                Guid = $"at://{did}/com.shinolabs.pinksea.oekaki/{o.OekakiRecordKey}",
                Title = $"{did}'s oekaki",
                PublishedDate = o.CreationTime.ToString("R"),
                Description = o.Alt ?? "",
                Link = $"{options.Value.FrontEndEndpoint}/{did}/oekaki/{o.OekakiRecordKey}",
                Enclosure = new RssEnclosure()
                {
                    Url = o.ImageLink,
                    Length = 0,
                    Type = "image/png"
                }
            })
            .ToList() ?? [];
        
        return new RssRoot()
        {
            Channel = new RssChannel()
            {
                Title = $"{did}'s posts feed",
                Link = $"{options.Value.FrontEndEndpoint}/{did}/rss",
                Description = "",
                Atom = new AtomLink
                {
                    Link = $"{options.Value.FrontEndEndpoint}/{did}/rss",
                    Relative = "self",
                    Type = "application/rss+xml"
                },
                Items = items
            }
        };
    }
}