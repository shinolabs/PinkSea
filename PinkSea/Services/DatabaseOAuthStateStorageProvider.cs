using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.Providers.Storage;
using PinkSea.Database;
using PinkSea.Database.Models;

namespace PinkSea.Services;

/// <summary>
/// A database backed OAuth state storage provider.
/// </summary>
public class DatabaseOAuthStateStorageProvider(PinkSeaDbContext pinkSeaDbContext)
    : IOAuthStateStorageProvider
{
    /// <inheritdoc />
    public async Task SetForStateId(string id, OAuthState state)
    {
        var stateIdExists = await pinkSeaDbContext
            .OAuthStates
            .AnyAsync(o => o.Id == id);
        
        var serialized = JsonSerializer.Serialize(state);
        
        if (!stateIdExists)
        {
            await pinkSeaDbContext.OAuthStates.AddAsync(new OAuthStateModel()
            {
                Id = id,
                Json = serialized
            });

            await pinkSeaDbContext.SaveChangesAsync();
        }
        else
        {
            await pinkSeaDbContext.OAuthStates
                .Where(o => o.Id == id)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(o => o.Json, serialized));
        }
    }

    /// <inheritdoc />
    public async Task<OAuthState?> GetForStateId(string id)
    {
        var maybeState = await pinkSeaDbContext.OAuthStates
            .Where(o => o.Id == id)
            .FirstOrDefaultAsync();

        return maybeState is null
            ? null
            : JsonSerializer.Deserialize<OAuthState>(maybeState.Json);
    }

    /// <inheritdoc />
    public async Task DeleteForStateId(string id)
    {
        await pinkSeaDbContext.OAuthStates
            .Where(s => s.Id == id)
            .ExecuteDeleteAsync();
    }
}