using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PinkSea.AtProto.Streaming.JetStream.Events;
using Websocket.Client;

namespace PinkSea.AtProto.Streaming.JetStream;

/// <summary>
/// A listener for the AT Protocol jetstream.
/// </summary>
public class JetStreamListener(
    IOptions<JetStreamOptions> opts,
    IServiceScopeFactory serviceScopeFactory) : IHostedService
{
    /// <summary>
    /// The websocket client.
    /// </summary>
    private WebsocketClient? _client;
    
    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        const string defaultEndpoint = "jetstream1.us-east.bsky.network";
        var url = $"wss://{opts.Value.Endpoint ?? defaultEndpoint}/subscribe";
        
        if (opts.Value.WantedCollections is not null)
        {
            var collections = opts.Value.WantedCollections.Aggregate(
                "?", (acc, s) => $"{acc}wantedCollections={s}&")[..^1];
            
            url += collections;
        }

        _client = new WebsocketClient(new Uri(url));

        _client.MessageReceived
            .Where(msg => msg.Text is not null)
            .Select(msg =>
            {
                return Observable.FromAsync(async () =>
                {
                    await OnWebsocketMessage(msg);
                });
            })
            .Merge(opts.Value.DegreeOfParallelism)
            .Subscribe();
        
        await _client.Start();
    }

    /// <summary>
    /// Called when we receive a websocket message.
    /// </summary>
    /// <param name="message">The message.</param>
    private async Task OnWebsocketMessage(ResponseMessage message)
    {
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var listeners = scope.ServiceProvider.GetServices<IJetStreamEventHandler>();
            var @event = JsonSerializer.Deserialize<JetStreamEvent>(message.Text!)!;
        
            foreach (var listener in listeners)
                await listener.HandleEvent(@event);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Encountered an exception while handling JetStream event: {e}");
        }
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client!.Stop(WebSocketCloseStatus.NormalClosure, "");
    }
}