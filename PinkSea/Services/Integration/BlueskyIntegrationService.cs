using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using PinkSea.AtProto.Providers.Storage;
using PinkSea.AtProto.Shared.Lexicons.AtProto;
using PinkSea.AtProto.Shared.Lexicons.AtProto.Records;
using PinkSea.AtProto.Shared.Lexicons.Bluesky.Records;
using PinkSea.AtProto.Shared.Lexicons.Types;
using PinkSea.AtProto.Xrpc.Client;
using PinkSea.Lexicons.Records;
using PinkSea.Models;
using Image = PinkSea.AtProto.Shared.Lexicons.Bluesky.Records.Image;

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
    /// <returns>The TID of the created record.</returns>
    public async Task<string?> CrosspostToBluesky(
        Oekaki oekaki,
        string stateId,
        string oekakiRecordId,
        int width,
        int height)
    {
        var config = frontendOptions.Value;
        if (config.FrontendUrl is null)
            return null;
        
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

        var labels = oekaki.Nsfw == true
            ? new SelfLabels
            {
                Values = [
                    new SelfLabel
                    {
                        Value = "sexual"
                    }
                ]
            }
            : null;
        
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
            SelfLabel = labels,
            Facets = ExtractFacets(text)
        };

        var rkey = Tid.NewTid()
            .ToString();
        
        var resp = await xrpcClient!.Procedure<PutRecordResponse>(
            "com.atproto.repo.putRecord",
            new PutRecordRequest
            {
                Repo = oauthState!.Did,
                Collection = "app.bsky.feed.post",
                RecordKey = rkey,
                Record = record
            });

        return resp is not null
            ? rkey
            : null;
    }

    /// <summary>
    /// Extracts the facets.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>The facets.</returns>
    private static IEnumerable<Facet> ExtractFacets(string text)
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
            var group = tag.Groups[1];
            facets.Add(new Facet
            {
                Index = new Facet.FacetIndex
                {
                    ByteStart = StringToByteIndex(text, group.Index),
                    ByteEnd = StringToByteIndex(text, group.Index + group.Length)
                },
                Features = [
                    new TagFacet
                    {
                        Tag = group.Value[1..]
                    }
                ]
            });
        }

        return facets;
    }

    /// <summary>
    /// Converts a string index to a byte index.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="index">The index.</param>
    /// <returns>The byte index.</returns>
    private static int StringToByteIndex(string str, int index)
    {
        // Not very optimal but it works.
        var subtr = str[..index];
        return Encoding.UTF8.GetByteCount(subtr);
    }

    /// <summary>
    /// The URL regex.
    /// </summary>
    [GeneratedRegex(@"https?:\/\/(?:www\.)?[a-zA-Z0-9-]+(?:\.[a-zA-Z]{2,})(?:[\/\w\.\-:]*)*\/?")]
    private static partial Regex UrlRegex();

    /// <summary>
    /// The tag regex.
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"(?:^|\s)(#[^\d\s]\S*)(?=\s)?")]
    private static partial Regex TagRegex();
}