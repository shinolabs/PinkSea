using System.Text.Json.Serialization;

namespace PinkSea.Gateway.Models.Oekaki;

public class AuthorFeedResponse
{
    [JsonPropertyName("oekaki")]
    public OekakiDto[] Oekaki { get; set; }
}