using System.Net.WebSockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Websocket.Client;

namespace PinkSea.AtProto.Streaming.JetStream;

/// <summary>
/// A listener for the AT Protocol jetstream.
/// </summary>
public class JetStreamListener(
    IOptions<JetStreamOptions> opts) : IHostedService
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

        _client.MessageReceived.Subscribe(OnWebsocketMessage);
        await _client.Start();
    }

    /// <summary>
    /// Called when we receive a websocket message.
    /// </summary>
    /// <param name="message">The message.</param>
    private void OnWebsocketMessage(ResponseMessage message)
    {
        Console.WriteLine(message.Text!);
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client!.Stop(WebSocketCloseStatus.NormalClosure, "");
    }
}