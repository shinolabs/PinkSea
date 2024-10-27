namespace PinkSea.AtProto.Resolvers.Domain;

/// <summary>
/// Resolves a domain handle to a DID.
/// </summary>
public interface IDomainDidResolver
{
    /// <summary>
    /// Gets the DID for a handle.
    /// </summary>
    /// <param name="handle">The handle.</param>
    /// <returns>The did, or nothing.</returns>
    public Task<string?> GetDidForDomainHandle(string handle);
}