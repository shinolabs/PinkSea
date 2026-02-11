using System.Text.Json.Serialization;

namespace PinkSea.Lexicons.Objects;

public class Preference
{
    [JsonPropertyName("key")]
    public required string Key { get; set; }
    
    [JsonPropertyName("value")]
    public required string Value { get; set; }
}