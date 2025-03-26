namespace PinkSea.AtProto.Models.OAuth;

/// <summary>
/// A DPoP keypair.
/// </summary>
public class DpopKeyPair
{
    /// <summary>
    /// The public key.
    /// </summary>
    public required string PublicKey { get; init; }
    
    /// <summary>
    /// The private key.
    /// </summary>
    public required string PrivateKey { get; init; }
}