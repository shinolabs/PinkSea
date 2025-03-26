using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons.Types;

/// <summary>
/// A commit to a repo.
/// </summary>
public class Commit
{
    /// <summary>
    /// The CID of the commit.
    /// </summary>
    [JsonPropertyName("cid")]
    public required string Cid { get; set; }
    
    /// <summary>
    /// The revision of the commit.
    /// </summary>
    [JsonPropertyName("rev")]
    public required string Revision { get; set; }
}