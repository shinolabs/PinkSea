using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.Providers.Storage;

namespace PinkSea.Services;

/// <summary>
/// An in-memory OAuth state storage provider.
/// </summary>
public class MemoryOAuthStateStorageProvider : IOAuthStateStorageProvider
{
    /// <summary>
    /// The dictionary.
    /// </summary>
    private readonly Dictionary<string, OAuthState> _dict = new Dictionary<string, OAuthState>();
    
    /// <inheritdoc />
    public Task SetForStateId(string id, OAuthState state)
    {
        _dict[id] = state;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<OAuthState?> GetForStateId(string id)
    {
        _dict.TryGetValue(id, out var state);
        return Task.FromResult(state);
    }
}