using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PinkSea.AtProto.Streaming.JetStream;

namespace PinkSea.AtProto.Streaming;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the jetstream listener into the service collection.
    /// </summary>
    /// <param name="collection">The service collection.</param>
    /// <param name="configureMethod">The method for configuring jetstream.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddJetStream(
        this IServiceCollection collection,
        Action<JetStreamOptions>? configureMethod = null)
    {
        var opts = new JetStreamOptions();
        configureMethod?.Invoke(opts);

        collection.AddSingleton(Options.Create(opts));
        collection.AddHostedService<JetStreamListener>();

        return collection;
    }
}