using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using PinkSea.AtProto.Lexicons.AtProto;
using PinkSea.AtProto.Lexicons.Bluesky.Records;
using PinkSea.AtProto.Lexicons.Types;
using PinkSea.AtProto.Providers.Storage;
using PinkSea.AtProto.Xrpc.Client;
using PinkSea.Lexicons.Records;
using PinkSea.Models;
using Image = PinkSea.AtProto.Lexicons.Bluesky.Records.Image;

namespace PinkSea.Services.Integration;

/// <summary>
/// Services for integrating with the Bluesky AppView.
/// </summary>
public partial class BlueskyIntegrationService(
    IXrpcClientFactory xrpcClientFactory,
    IOAuthStateStorageProvider oAuthStateStorageProvider,
    IOptions<FrontendConfig> frontendOptions)
{
    /// <summary>
    /// Crossposts the image to Bluesky.
    /// </summary>
    /// <param name="oekaki">The oekaki record.</param>
    /// <param name="stateId">The state id.</param>
    /// <param name="oekakiRecordId">The oekaki record id.</param>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    public async Task CrosspostToBluesky(
        Oekaki oekaki,
        string stateId,
        string oekakiRecordId,
        int width,
        int height)
    {
        var config = frontendOptions.Value;
        if (config.FrontendUrl is null)
            return;
        
        using var xrpcClient = await xrpcClientFactory.GetForOAuthStateId(stateId);
        var oauthState = await oAuthStateStorageProvider.GetForStateId(stateId);

        var postBuilder = new StringBuilder();
        
        postBuilder.Append($"{config.FrontendUrl}/{oauthState!.Did}/oekaki/{oekakiRecordId}\n\n#pinksea");

        // Build the tag array.
        if (oekaki.Tags is not null && oekaki.Tags.Length > 0)
        {
            foreach (var tag in oekaki.Tags)
            {
                // The 2 is the length of " #"
                var newLength = postBuilder.Length + 2 + tag.Length;
                if (newLength > 300)
                    break;

                postBuilder.Append(" #");
                postBuilder.Append(tag);
            }
        }

        var text = postBuilder.ToString();
        
        var record = new Post
        {
            Text = text,
            CreatedAt = DateTimeOffset.UtcNow,
            Embed = new ImageEmbed
            {
                Images =
                [
                    new Image
                    {
                        Alt = oekaki.Image.ImageLink.Alt,
                        Blob = oekaki.Image.Blob,
                        AspectRatio = new AspectRatio
                        {
                            Width = width,
                            Height = height
                        }
                    }
                ]
            },
            Facets = ExtractFacets(text)
        };
        
        await xrpcClient!.Procedure<PutRecordResponse>(
            "com.atproto.repo.putRecord",
            new PutRecordRequest
            {
                Repo = oauthState!.Did,
                Collection = "app.bsky.feed.post",
                RecordKey = Tid.NewTid().ToString(),
                Record = record
            });
    }

    /// <summary>
    /// Extracts the facets.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>The facets.</returns>
    private IEnumerable<Facet> ExtractFacets(string text)
    {
        var facets = new List<Facet>();
        
        foreach (Match url in UrlRegex().Matches(text))
        {
            facets.Add(new Facet
            {
                Index = new Facet.FacetIndex
                {
                    ByteStart = url.Index,
                    ByteEnd = url.Index + url.Length
                },
                Features = [
                    new LinkFacet
                    {
                        Uri = url.Value
                    }
                ]
            });
        }

        foreach (Match tag in TagRegex().Matches(text))
        {
            facets.Add(new Facet
            {
                Index = new Facet.FacetIndex
                {
                    ByteStart = tag.Index - 1,
                    ByteEnd = tag.Index + tag.Length
                },
                Features = [
                    new TagFacet
                    {
                        Tag = tag.Value
                    }
                ]
            });
        }

        return facets;
    }

    /// <summary>
    /// The URL regex.
    /// </summary>
    [GeneratedRegex("https?:\\/\\/(?:www\\.)?[a-zA-Z0-9-]+(?:\\.[a-zA-Z]{2,})(?:[\\/\\w\\.\\-:]*)*\\/?")]
    private static partial Regex UrlRegex();

    /// <summary>
    /// The tag regex.
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex("(?<=#)\\p{L}+")]
    private static partial Regex TagRegex();
}