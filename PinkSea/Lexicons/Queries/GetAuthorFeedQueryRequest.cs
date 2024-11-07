using System.Text.Json.Serialization;

namespace PinkSea.Lexicons.Queries;

/// <summary>
/// The request for the "com.shinolabs.pinksea.getAuthorFeed" xrpc call.
/// </summary>
public class GetAuthorFeedQueryRequest : GenericTimelineQueryRequest
{
    /// <summary>
    /// The DID of the author.
    /// </summary>
    [JsonPropertyName("did")]
    public required string Did { get; set; }
}