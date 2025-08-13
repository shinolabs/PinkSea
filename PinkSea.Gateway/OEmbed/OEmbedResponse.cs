using System.Text.Json.Serialization;

namespace PinkSea.Gateway.OEmbed;

/// <summary>
/// An OEmbed 1.0 response.
/// </summary>
public class OEmbedResponse
{
    /// <summary>
    /// The version, always 1.0
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; } = "1.0";
    
    /// <summary>
    /// The type of the response.
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; set; }
    
    /// <summary>
    /// The URL of the asset.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>
    /// The width of the asset.
    /// </summary>
    [JsonPropertyName("width")]
    public int Width { get; set; }
    
    /// <summary>
    /// The height of the asset.
    /// </summary>
    [JsonPropertyName("height")]
    public int Height { get; set; }
    
    /// <summary>
    /// The title of the asset.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    
    /// <summary>
    /// The name of the author of this asset.
    /// </summary>
    [JsonPropertyName("author_name")]
    public string? AuthorName { get; set; }
    
    /// <summary>
    /// The URL of the author of this asset.
    /// </summary>
    [JsonPropertyName("author_url")]
    public string? AuthorUrl { get; set; }
    
    /// <summary>
    /// The name of the service providing this asset.
    /// </summary>
    [JsonPropertyName("provider_name")]
    public string? ProviderName { get; set; }
    
    /// <summary>
    /// The URL of the service providing this asset.
    /// </summary>
    [JsonPropertyName("provider_url")]
    public string? ProviderUrl { get; set; }
}