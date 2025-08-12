using System.Text.Json.Serialization;

namespace PinkSea.Gateway.ActivityStreams;

public class Note : IActivityStreamsObject
{
    /// <summary>
    /// The type of the object
    /// </summary>
    [JsonPropertyName("type")]
    public string Type => "Note";
    
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

    /// <summary>
    /// Who is this note attributed to?
    /// </summary>
    [JsonPropertyName("attributedTo")]
    public string? AttributedTo { get; set; }

    /// <summary>
    /// The url of the note.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
    
    /// <summary>
    /// The content of the note.
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
    
    /// <summary>
    /// What this note is replying to.
    /// </summary>
    [JsonPropertyName("inReplyTo")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? InReplyTo { get; set; }
    
    /// <summary>
    /// Is this post sensitive?
    /// </summary>
    [JsonPropertyName("sensitive")]
    public bool? Sensitive { get; set; }
    
    /// <summary>
    /// The list of tags this note has.
    /// </summary>
    [JsonPropertyName("tag")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<Hashtag>? Tags { get; set; }
    
    /// <summary>
    /// The list of attachments this note has.
    /// </summary>
    [JsonPropertyName("attachment")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<Document>? Attachments { get; set; }
}