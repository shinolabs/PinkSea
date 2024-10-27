using PinkSea.AtProto.Models.OAuth;

namespace PinkSea.AtProto.Providers.OAuth;

/// <summary>
/// The DPoP signing data.
/// </summary>
public class DpopSigningData
{
    /// <summary>
    /// The client id.
    /// </summary>
    public required string ClientId { get; set; }
    
    /// <summary>
    /// The DPoP keypair.
    /// </summary>
    public required DpopKeyPair Keypair { get; set; }
    
    /// <summary>
    /// The HTTP method.
    /// </summary>
    public required string Method { get; set; }
    
    /// <summary>
    /// The HTTP url.
    /// </summary>
    public required string Url { get; set; }
    
    /// <summary>
    /// The nonce.
    /// </summary>
    public string? Nonce { get; set; }
}