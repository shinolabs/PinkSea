using PinkSea.AtProto.Models.OAuth;

namespace PinkSea.AtProto.Providers.Storage;

/// <summary>
/// An OAuth state provider.
/// </summary>
public interface IOAuthStateStorageProvider
{
    /// <summary>
    /// Sets the OAuth state for a given state id.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="state">The OAuth state.</param>
    Task SetForStateId(string id, OAuthState state);

    /// <summary>
    /// Gets the OAuth state for a given state id.
    /// </summary>
    /// <param name="id">The state id.</param>
    /// <returns>The OAuth state.</returns>
    Task<OAuthState?> GetForStateId(string id);
}