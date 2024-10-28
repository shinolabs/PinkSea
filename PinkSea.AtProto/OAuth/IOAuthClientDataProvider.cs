using PinkSea.AtProto.Models.OAuth;

namespace PinkSea.AtProto.OAuth;

/// <summary>
/// A provider for OAuth client data.
/// </summary>
public interface IOAuthClientDataProvider
{
    /// <summary>
    /// The client data.
    /// </summary>
    OAuthClientData ClientData { get; }
    
    /// <summary>
    /// The client metadata.
    /// </summary>
    ClientMetadata ClientMetadata { get; }
}