using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Models.OAuth;

public class PushedAuthorizationRequestResponse
{
    [JsonPropertyName("request_uri")]
    public required string RequestUri { get; init; }
}