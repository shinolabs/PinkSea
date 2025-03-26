namespace PinkSea.AtProto.Shared.Lexicons.AtProto;

/// <summary>
/// The request for the "com.atproto.repo.listRecords" XRPC call.
/// </summary>
public class ListRecordsRequest
{
    /// <summary>
    /// The repository DID.
    /// </summary>
    public required string Repo { get; set; }
    
    /// <summary>
    /// The collection DID.
    /// </summary>
    public required string Collection { get; set; }
    
    /// <summary>
    /// The cursor to paginate from.
    /// </summary>
    public string? Cursor { get; set; }
}