using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons;

/// <summary>
/// The base lexicon object.
/// </summary>
public class BaseLexiconObject
{
    /// <summary>
    /// The constructor that sets the type of the base lexicon object.
    /// </summary>
    /// <param name="type">The type.</param>
    public BaseLexiconObject(string type)
    {
        Type = type;
    }
    
    /// <summary>
    /// The type of the object.
    /// </summary>
    [JsonPropertyName("$type")]
    public string? Type { get; set; }
}