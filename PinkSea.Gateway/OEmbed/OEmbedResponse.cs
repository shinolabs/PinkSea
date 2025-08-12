using System.Text.Json.Serialization;

namespace PinkSea.Gateway.OEmbed;

public class OEmbedResponse
{
    [JsonPropertyName("version")]
    public string Version { get; } = "1.0";
    
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonPropertyName("url")]
    public string Url { get; init; }

    [JsonPropertyName("width")]
    public int Width { get; set; }
    
    [JsonPropertyName("height")]
    public int Height { get; set; }
    
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    
    [JsonPropertyName("author_name")]
    public string? AuthorName { get; set; }
    
    [JsonPropertyName("author_url")]
    public string? AuthorUrl { get; set; }
    
    [JsonPropertyName("provider_name")]
    public string? ProviderName { get; set; }
    
    [JsonPropertyName("provider_url")]
    public string? ProviderUrl { get; set; }
}