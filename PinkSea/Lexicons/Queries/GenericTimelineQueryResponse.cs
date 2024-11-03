using System.Text.Json.Serialization;
using PinkSea.Models.Dto;

namespace PinkSea.Lexicons.Queries;

/// <summary>
/// The generic timeline query response.
/// </summary>
public class GenericTimelineQueryResponse
{
    /// <summary>
    /// The oekaki that were a result of the query.
    /// </summary>
    [JsonPropertyName("oekaki")]
    public required IEnumerable<OekakiDto> Oekaki { get; set; }
}