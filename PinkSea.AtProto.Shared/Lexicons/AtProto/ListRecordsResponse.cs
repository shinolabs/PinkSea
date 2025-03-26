using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons.AtProto;

/// <summary>
/// Response for the "com.atproto.repo.listRecords" XRPC call.
/// </summary>
/// <typeparam name="TRecord"></typeparam>
public class ListRecordsResponse<TRecord>
{
    /// <summary>
    /// A singular record.
    /// </summary>
    public class Record
    {
        /// <summary>
        /// The AT uri.
        /// </summary>
        [JsonPropertyName("uri")]
        public required string AtUri { get; set; }
        
        /// <summary>
        /// The CID of the record.
        /// </summary>
        [JsonPropertyName("cid")]
        public required string Cid { get; set; }
        
        /// <summary>
        /// The record.
        /// </summary>
        [JsonPropertyName("value")]
        public required TRecord Value { get; set; }
    }
    
    /// <summary>
    /// The records.
    /// </summary>
    [JsonPropertyName("records")]
    public required IReadOnlyList<Record> Records { get; set; }
    
    /// <summary>
    /// The cursor to paginate from.
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }
}