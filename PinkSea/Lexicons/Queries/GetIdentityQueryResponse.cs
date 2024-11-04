namespace PinkSea.Lexicons.Queries;

/// <summary>
/// The response for the "com.shinolabs.pinksea.getIdentity" xrpc call.
/// </summary>
public class GetIdentityQueryResponse
{
    /// <summary>
    /// The did.
    /// </summary>
    public required string Did { get; set; }
    
    /// <summary>
    /// The handle.
    /// </summary>
    public required string Handle { get; set; }
}