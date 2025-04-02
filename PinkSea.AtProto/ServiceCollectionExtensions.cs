using DnsClient;
using Microsoft.Extensions.DependencyInjection;
using PinkSea.AtProto.Authorization;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Providers.OAuth;
using PinkSea.AtProto.Providers.Storage;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Resolvers.Domain;
using PinkSea.AtProto.Xrpc.Client;

namespace PinkSea.AtProto;

/// <summary>
/// Extensions for the service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds in dummy authorization providers.
    /// </summary>
    /// <param name="collection">The service collection to add them to.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddDummyAuthorizationProviders(
        this IServiceCollection collection)
    {
        collection.AddScoped<IOAuthStateStorageProvider, InMemoryOAuthStateStorageProvider>();
        return collection;
    }

    /// <summary>
    /// Adds the ATProto OAuth services.
    /// </summary>
    /// <param name="collection">The service collection to add them to.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddAtProtoOAuthServices(
        this IServiceCollection collection)
    {
        collection.AddScoped<IAtProtoOAuthClient, AtProtoOAuthClient>();
        return collection;
    }
    
    /// <summary>
    /// Adds the ATProto client services.
    /// </summary>
    /// <param name="collection">The service collection to add them to.</param>
    /// <param name="config">The config method.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddAtProtoClientServices(
        this IServiceCollection collection,
        Action<AtProtoClientServicesConfig>? config = null)
    {
        const string endpoint = "PinkSea.AtProto/1.0 (component={0}) (+https://github.com/shinolabs/PinkSea)";
        
        var options = new AtProtoClientServicesConfig();
        config?.Invoke(options);
        
        collection.AddMemoryCache();
        collection.AddSingleton<IDnsQuery, LookupClient>();
        collection.AddTransient<IDomainDidResolver, DomainDidResolver>();
        collection.AddTransient<IJwtSigningProvider, JwtSigningProvider>();
        collection.AddScoped<IDidResolver, DidResolver>();
        collection.AddScoped<IXrpcClientFactory, DefaultXrpcClientFactory>();
        collection.AddScoped<IAtProtoAuthorizationService, AtProtoAuthorizationService>();

        collection.AddHttpClient("did-resolver", client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", 
                string.Format(endpoint, "DidResolver"));
            client.BaseAddress = options.PlcDirectory;
        });

        collection.AddHttpClient("domain-did-resolver", client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", 
                string.Format(endpoint, "DomainDidResolver"));
        });
        
        collection.AddHttpClient("xrpc-client", client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", 
                string.Format(endpoint, "XrpcClient"));
        });

        return collection;
    }
}