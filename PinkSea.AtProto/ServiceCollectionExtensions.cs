using DnsClient;
using Microsoft.Extensions.DependencyInjection;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Resolvers.Domain;

namespace PinkSea.AtProto;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAtProtoServices(this IServiceCollection collection)
    {
        collection.AddSingleton<LookupClient>();
        collection.AddTransient<IDomainDidResolver, DomainDidResolver>();
        collection.AddScoped<IDidResolver, DidResolver>();
        collection.AddScoped<IAtProtoOAuthClient, AtProtoOAuthClient>();

        collection.AddHttpClient("did-resolver", client =>
        {
            client.BaseAddress = new Uri("https://plc.directory");
        });

        return collection;
    }
}