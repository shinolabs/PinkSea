using System.Text.Json.Serialization;

namespace PinkSea.Lexicons.Queries;

/// <summary>
/// The request for the "com.shinolabs.pinksea.getTagFeed" xrpc call.
/// </summary>
public class GetTagFeedQueryRequest : GenericTimelineQueryRequest
{
    /// <summary>
    /// The tag to check.
    /// </summary>
    [JsonPropertyName("tag")]
    public required string Tag { get; set; }
}