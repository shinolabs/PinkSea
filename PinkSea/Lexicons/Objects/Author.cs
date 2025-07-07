using System.Text.Json.Serialization;

namespace PinkSea.Lexicons.Objects;

/// <summary>
/// The "com.shinolabs.pinksea.appViewDefs#author" object.
/// </summary>
public class Author
{
    /// <summary>
    /// The DID of the author.
    /// </summary>
    [JsonPropertyName("did")]
    public required string Did { get; set; }
    
    /// <summary>
    /// The handle of the author.
    /// </summary>
    [JsonPropertyName("handle")]
    public required string Handle { get; set; }
    
    /// <summary>
    /// The nickname of the author.
    /// </summary>
    [JsonPropertyName("nickname"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Nickname { get; set; }
    
    /// <summary>
    /// The avatar of the author.
    /// </summary>
    [JsonPropertyName("avatar"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Avatar { get; set; }
}