using DnsClient;
using Microsoft.Extensions.DependencyInjection;
using PinkSea.AtProto.Authorization;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Providers.OAuth;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Resolvers.Domain;
using PinkSea.AtProto.Xrpc.Client;

namespace PinkSea.AtProto;

public static class ServiceCollectionExtensions
{
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
        collection.AddScoped<IAtProtoOAuthClient, AtProtoOAuthClient>();
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