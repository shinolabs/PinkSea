namespace PinkSea.AtProto.Lexicons.AtProto;

/// <summary>
/// The request for the "com.atproto.sync.listReposByCollection" XRPC call.
/// </summary>
public class GetReposByCollectionRequest
{
    /// <summary>
    /// The collection name.
    /// </summary>
    public required string Collection { get; set; }
    
    /// <summary>
    /// The limit on DIDs.
    /// </summary>
    public required int Limit { get; set; }
}