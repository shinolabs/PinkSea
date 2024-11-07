using System.Text.Json.Serialization;

namespace PinkSea.Lexicons.Queries;

/// <summary>
/// A generic timeline query request.
/// </summary>
public class GenericTimelineQueryRequest
{
    /// <summary>
    /// Since when should we query.
    /// </summary>
    [JsonPropertyName("since")]
    public DateTimeOffset? Since { get; set; }

    /// <summary>
    /// The limit on posts to fetch.
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; } = 50;
}