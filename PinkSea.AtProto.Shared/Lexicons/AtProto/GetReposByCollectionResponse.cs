using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons.AtProto;

/// <summary>
/// A response for the "com.atproto.sync.listReposByCollection" XRPC call.
/// </summary>
public class GetReposByCollectionResponse
{
    /// <summary>
    /// A single repository.
    /// </summary>
    public class Repo
    {
        /// <summary>
        /// The DID of the repo.
        /// </summary>
        [JsonPropertyName("did")]
        public required string Did { get; set; }
    }
    
    /// <summary>
    /// A list of all the repositories.
    /// </summary>
    [JsonPropertyName("repos")]
    public required IReadOnlyList<Repo> Repos { get; set; }
    
    /// <summary>
    /// The cursor.
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }
}