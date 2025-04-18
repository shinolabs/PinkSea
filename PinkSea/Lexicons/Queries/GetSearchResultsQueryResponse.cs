using System.Text.Json.Serialization;
using PinkSea.Lexicons.Objects;

namespace PinkSea.Lexicons.Queries;

/// <summary>
/// The response for the "com.shinolabs.pinksea.getSearchResults" XRPC call.
/// </summary>
public class GetSearchResultsQueryResponse
{
    /// <summary>
    /// The list of oekaki.
    /// </summary>
    [JsonPropertyName("oekaki")]
    public IReadOnlyList<HydratedOekaki>? Oekaki { get; set; } = [];
    
    /// <summary>
    /// The list of tags.
    /// </summary>
    [JsonPropertyName("tags")]
    public IReadOnlyList<TagSearchResult>? Tags { get; set; } = [];
    
    /// <summary>
    /// The list of profiles.
    /// </summary>
    [JsonPropertyName("profiles")]
    public IReadOnlyList<Author>? Profiles { get; set; } = [];
}