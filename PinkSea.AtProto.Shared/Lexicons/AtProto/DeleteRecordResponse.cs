using System.Text.Json.Serialization;
using PinkSea.AtProto.Shared.Lexicons.Types;

namespace PinkSea.AtProto.Shared.Lexicons.AtProto;

/// <summary>
/// The response produced by "com.atproto.repo.deleteRecord".
/// </summary>
public class DeleteRecordResponse
{
    /// <summary>
    /// The commit of the record.
    /// </summary>
    [JsonPropertyName("commit")]
    public required Commit Commit { get; set; }
}