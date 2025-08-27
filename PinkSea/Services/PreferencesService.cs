using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cms;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.Shared.Lexicons.AtProto;
using PinkSea.AtProto.Xrpc.Client;
using PinkSea.Database;
using PinkSea.Database.Models;
using PinkSea.Lexicons.Objects;
using PinkSea.Lexicons.Records;

namespace PinkSea.Services;

public class PreferencesService(
    IXrpcClientFactory xrpcClientFactory,
    ILogger<PreferencesService> logger,
    PinkSeaDbContext dbContext)
{
    public async Task<List<UserPreferenceModel>> GetAllPreferencesForUser(UserModel user)
    {
        var prefs = await dbContext.Preferences
            .Where(u => u.UserDid == user.Did)
            .ToListAsync();

        return prefs;
    }
    
    public async Task SetPreferenceForUser(
        UserModel user,
        string key,
        string value)
    {
        logger.LogInformation("Setting preference {Key} to {Value} for DID {Did}", key, value, user.Did);
        
        var preference = await dbContext.Preferences
            .Where(u => u.UserDid == user.Did && key == u.Key)
            .FirstOrDefaultAsync();

        if (preference == null)
        {
            preference = new UserPreferenceModel
            {
                UserDid = user.Did,
                User = user,
                Key = key,
                Value = value
            };
            
            await dbContext.Preferences.AddAsync(preference);
        }
        else
        {
            preference.Value = value;
            dbContext.Preferences.Update(preference);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task ImportRepoPreferences(
        UserModel user,
        Preferences record)
    {
        logger.LogInformation("Importing preferences for DID {Did}", user.Did);
        
        await DeletePreferencesForUser(user);

        foreach (var preference in record.Values)
        {
            var model = new UserPreferenceModel
            {
                UserDid = user.Did,
                User = user,
                Key = preference.Key,
                Value = preference.Value
            };

            await dbContext.Preferences.AddAsync(model);
        }
        
        await dbContext.SaveChangesAsync();
    }

    public async Task DeletePreferencesForUser(UserModel user)
    {
        logger.LogInformation("Deleting preferences for DID {Did}", user.Did);
        
        await dbContext.Preferences
            .Where(p => p.UserDid == user.Did)
            .ExecuteDeleteAsync();
    }

    public async Task<bool> PublishPreferencesUpdateToRepo(
        OAuthState oauthState)
    {
        var preferences = await dbContext.Preferences
            .Where(p => p.UserDid == oauthState.Did)
            .Select(p => new Preference
            {
                Key = p.Key,
                Value = p.Value
            })
            .ToListAsync();

        var record = new Preferences
        {
            Values = preferences
        };

        using var xrpcClient = await xrpcClientFactory.GetForOAuthState(oauthState);
        if (xrpcClient == null)
        {
            return false;
        }
        
        var result = await xrpcClient.Procedure<PutRecordResponse>(
            "com.atproto.repo.putRecord",
            new PutRecordRequest
            {
                Repo = oauthState.Did,
                Collection = "com.shinolabs.pinksea.preferences",
                RecordKey = "self",
                Record = record
            });

        return result.IsSuccess;
    }
}