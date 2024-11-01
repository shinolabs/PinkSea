using System.Text.Json.Serialization;
using PinkSea.AtProto.Lexicons.Types;

namespace PinkSea.Lexicons.Records;

/// <summary>
/// The com.shinolabs.pinksea.oekaki record.
/// </summary>
public class Oekaki
{
    /// <summary>
    /// The timestamp of creation.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public required string CreatedAt { get; set; }
    
    /// <summary>
    /// The reference to the image.
    /// </summary>
    [JsonPropertyName("image")]
    public required Image Image { get; set; }
    
    /// <summary>
    /// The tags.
    /// </summary>
    [JsonPropertyName("tags"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? Tags { get; set; }
    
    /// <summary>
    /// What this oekaki post is a response to.
    /// </summary>
    [JsonPropertyName("inResponseTo"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public StrongRef? InResponseTo { get; set; }
}