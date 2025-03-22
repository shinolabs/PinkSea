using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PinkSea.AtProto.Streaming.JetStream.Events;
using Websocket.Client;

namespace PinkSea.AtProto.Streaming.JetStream;

/// <summary>
/// A listener for the AT Protocol JetStream.
/// </summary>
public class JetStreamListener(
    IOptions<JetStreamOptions> opts,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<JetStreamListener> logger) : IHostedService
{
    /// <summary>
    /// The websocket client.
    /// </summary>
    private WebsocketClient? _client;

    /// <summary>
    /// Last time in microseconds.
    /// </summary>
    private long LastTimeInMicroseconds { get; set; }
    
    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var uri = BuildJetStreamEndpoint();

        _client = new WebsocketClient(uri);

        _client.DisconnectionHappened
            .Subscribe(OnDisconnectionHappened);

        _client.ReconnectionHappened
            .Subscribe(OmReconnectionHappened);
        
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
        
        logger.LogInformation("Connecting to JetStream @ {JetStreamEndpoint}...", uri);
        
        await _client.Start();
    }
    
    /// <summary>
    /// Called when we disconnect from the Jetstream server.
    /// </summary>
    /// <param name="disconnectionInfo">The disconnection info.</param>
    private void OnDisconnectionHappened(DisconnectionInfo disconnectionInfo)
    {
        const long thirtySecondsInMicroseconds = 30_000_000;
        
        logger.LogWarning(disconnectionInfo.Exception,
            "Lost connection to {JetStreamEndpoint} ({Type})!",
            _client!.Url,
            disconnectionInfo.Type);

        // Replace the URL to ensure consistent playback.
        var cursor = LastTimeInMicroseconds - thirtySecondsInMicroseconds;
        _client.Url = BuildJetStreamEndpoint(cursor.ToString());
    }

    /// <summary>
    /// Called when we reconnect to the Jetstream server.
    /// </summary>
    /// <param name="reconnectionInfo">The reconnection info.</param>
    private void OmReconnectionHappened(ReconnectionInfo reconnectionInfo)
    {
        logger.LogInformation("Connected to JetStream @ {JetStreamEndpoint}", _client!.Url);    
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

            LastTimeInMicroseconds = @event.TimeInMicroseconds;
        
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

    /// <summary>
    /// Builds the JetStream endpoint.
    /// </summary>
    /// <returns>The endpoint.</returns>
    private Uri BuildJetStreamEndpoint(string? cursorOverride = null)
    {
        const string defaultEndpoint = "jetstream1.us-east.bsky.network";
        var url = $"wss://{opts.Value.Endpoint ?? defaultEndpoint}/subscribe";

        var queryString = HttpUtility.ParseQueryString(string.Empty);
        
        if (opts.Value.WantedCollections is not null)
        {
            foreach (var collection in opts.Value.WantedCollections)
            {
                queryString.Add("wantedCollections", collection);
            }
        }

        if (!string.IsNullOrEmpty(cursorOverride) || !string.IsNullOrEmpty(opts.Value.Cursor))
        {
            var cursor = cursorOverride ?? opts.Value.Cursor;
            if (!long.TryParse(cursor, out _))
                throw new InvalidOperationException($"{cursor} is not a valid unix timestamp.");
            
            queryString.Add("cursor", cursor);
        }

        url += $"?{queryString}";
        return new Uri(url);
    }
}