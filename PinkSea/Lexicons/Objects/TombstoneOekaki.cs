using System.Text.Json.Serialization;
using PinkSea.Database.Models;

namespace PinkSea.Lexicons.Objects;

/// <summary>
/// The "com.shinolabs.pinksea.appViewDefs#oekakiTombstone" object.s
/// </summary>
public class TombstoneOekaki
{
    /// <summary>
    /// The AT uri of the former oekaki.
    /// </summary>
    [JsonPropertyName("formerAt")]
    public required string FormerAt { get; set; }

    /// <summary>
    /// Constructs a tombstone oekaki from an oekaki model.
    /// </summary>
    /// <param name="model">The oekaki model.</param>
    /// <returns>The tombstone oekaki.</returns>
    public static TombstoneOekaki FromOekakiModel(OekakiModel model)
    {
        return new TombstoneOekaki
        {
            FormerAt = $"at://{model.AuthorDid}/com.shinolabs.pinksea.oekaki/{model.OekakiTid}",
        };
    }
}