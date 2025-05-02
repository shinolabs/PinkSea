using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using PinkSea.Database;
using PinkSea.Database.Models;

namespace PinkSea.Services;

/// <summary>
/// The configuration service for PinkSea.
/// </summary>
public class ConfigurationService(
    IServiceScopeFactory serviceScopeFactory)
{
    /// <summary>
    /// The configuration model.
    /// </summary>
    private ConfigurationModel? _configuration;
    
    /// <summary>
    /// The actual configuration.
    /// </summary>
    public ConfigurationModel Configuration
    {
        get
        {
            _configuration ??= GetConfigurationFromDatabase();
            return _configuration;
        }
    }

    /// <summary>
    /// Edits the config.
    /// </summary>
    public async Task EditConfiguration(Action<ConfigurationModel> method)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PinkSeaDbContext>();

        var config = GetConfigurationFromDatabase();

        method(config);

        _configuration = config;
        dbContext.Configuration
            .Update(config);
        
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Gets the configuration from a database.
    /// </summary>
    /// <returns></returns>
    private ConfigurationModel GetConfigurationFromDatabase()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PinkSeaDbContext>();

        var model = dbContext.Configuration
            .FirstOrDefault();

        if (model is null)
        {
            model = CreateDefaultConfiguration();
            
            dbContext.Configuration.Add(model);
            dbContext.SaveChanges();
        }

        return model;
    }

    /// <summary>
    /// Creates the default configuration.
    /// </summary>
    /// <returns>The default configuration.</returns>
    private ConfigurationModel CreateDefaultConfiguration()
    {
        var curve = ECDsa.Create(ECCurve.CreateFromFriendlyName("nistp256"));
        var securityKey = new ECDsaSecurityKey(curve);

        return new ConfigurationModel
        {
            ClientPrivateKey = curve.ExportECPrivateKeyPem(),
            ClientPublicKey = curve.ExportSubjectPublicKeyInfoPem(),
            KeyId = Base64UrlEncoder.Encode(securityKey.ComputeJwkThumbprint()),
            SynchronizedAccountStates = true
        };
    }
}