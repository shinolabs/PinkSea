using PinkSea.AtProto.Shared.Models.Did;

namespace PinkSea.AtProto.Resolvers.Did;

/// <summary>
/// Resolves a DID to a DID document.
/// </summary>
public interface IDidResolver
{
    /// <summary>
    /// Gets the DID document for a given DID.
    /// </summary>
    /// <param name="did">The DID.</param>
    /// <returns>The document, if it was possible to fetch.</returns>
    Task<DidDocument?> GetDocumentForDid(string did);

    /// <summary>
    /// Gets a handle from a DID.
    /// </summary>
    /// <param name="did">The DID.</param>
    /// <returns>The handle, if it was possible to fetch.</returns>
    Task<string?> GetHandleFromDid(string did);
}