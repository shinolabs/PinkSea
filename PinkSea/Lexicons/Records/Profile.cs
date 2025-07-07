using System.Text.Json.Serialization;
using PinkSea.AtProto.Shared.Lexicons.Types;

namespace PinkSea.Lexicons.Records;

/// <summary>
/// The com.shinolabs.pinksea.profile record.
/// </summary>
public class Profile
{
    /// <summary>
    /// The nickname of this profile.
    /// </summary>
    [JsonPropertyName("nickname"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Nickname { get; set; }
    
    /// <summary>
    /// The bio of this profile.
    /// </summary>
    [JsonPropertyName("bio"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Bio { get; set; }
    
    /// <summary>
    /// The avatar of this profile.
    /// </summary>
    [JsonPropertyName("avatar"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public StrongRef? Avatar { get; set; }
    
    /// <summary>
    /// The links this profile has.
    /// </summary>
    [JsonPropertyName("links"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<ProfileLink>? Links { get; set; }
}