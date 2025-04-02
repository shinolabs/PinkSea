using Microsoft.Extensions.Caching.Memory;
using PinkSea.AtProto.Models.OAuth;

namespace PinkSea.AtProto.Providers.Storage;

/// <summary>
/// A dummy OAuth 
/// </summary>
/// <param name="memoryCache"></param>
public class InMemoryOAuthStateStorageProvider(
    IMemoryCache memoryCache) : IOAuthStateStorageProvider
{
    /// <inheritdoc />
    public Task SetForStateId(string id, OAuthState state)
    {
        memoryCache.Set(id, state);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<OAuthState?> GetForStateId(string id)
    {
        return Task.FromResult(memoryCache.Get<OAuthState>(id));
    }

    /// <inheritdoc />
    public Task DeleteForStateId(string id)
    {
        memoryCache.Remove(id);
        return Task.CompletedTask;
    }
}