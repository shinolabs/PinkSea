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
    public static IServiceCollection AddAtProtoClientServices(this IServiceCollection collection)
    {
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
            client.BaseAddress = new Uri("https://plc.directory");
        });

        return collection;
    }
}