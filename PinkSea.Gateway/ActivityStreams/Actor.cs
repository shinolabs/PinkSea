using System.Text.Json.Serialization;

namespace PinkSea.Gateway.ActivityStreams;

/// <summary>
/// An ActivityStreams actor.
/// </summary>
public class Actor : IActivityStreamsObject
{
    /// <summary>
    /// The ID of the object.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    
    /// <summary>
    /// When was this object published at?
    /// </summary>
    [JsonPropertyName("published")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? PublishedAt { get; set; }
    
    /// <inheritdoc />
    [JsonPropertyName("type")]
    public string Type => "Person";
    
    /// <summary>
    /// The name of the actor.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    /// <summary>
    /// The display name.
    /// </summary>
    [JsonPropertyName("preferredUsername")]
    public string? PreferredUsername { get; set; }
    
    /// <summary>
    /// The bio of this actor.
    /// </summary>
    [JsonPropertyName("summary")]
    public string? Bio { get; set; }
    
    /// <summary>
    /// The inbox.
    /// </summary>
    [JsonPropertyName("inbox")]
    public string? Inbox { get; set; }
    
    /// <summary>
    /// The outbox.
    /// </summary>
    [JsonPropertyName("outbox")]
    public string? Outbox { get; set; }
    
    /// <summary>
    /// The icon.
    /// </summary>
    [JsonPropertyName("icon")]
    public Image? Icon { get; set; }
    
    /// <summary>
    /// The url of the actor.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}