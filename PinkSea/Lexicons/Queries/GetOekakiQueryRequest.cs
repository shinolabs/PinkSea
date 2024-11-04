namespace PinkSea.Lexicons.Queries;

/// <summary>
/// The request for the "com.shinolabs.pinksea.getOekaki" xrpc call.
/// </summary>
public class GetOekakiQueryRequest
{
    /// <summary>
    /// The DID of the author.
    /// </summary>
    public required string Did { get; set; }
    
    /// <summary>
    /// The record key.
    /// </summary>
    public required string RecordKey { get; set; }
}