using System.Text.Json.Serialization;
using PinkSea.Lexicons.Enums;

namespace PinkSea.Lexicons.Queries;

/// <summary>
/// The request for the "com.shinolabs.pinksea.getSearchResults" xrpc call.
/// </summary>
public class GetSearchResultsQueryRequest
{
    /// <summary>
    /// The query we're looking for.
    /// </summary>
    [JsonPropertyName("query")]
    public required string Query { get; set; }
    
    /// <summary>
    /// The search type.
    /// </summary>
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required SearchType Type { get; set; }

    /// <summary>
    /// Since when should we query.
    /// </summary>
    [JsonPropertyName("since")]
    public DateTimeOffset? Since { get; set; }

    /// <summary>
    /// The limit on values to fetch.
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; } = 50;
}