using PinkSea.AtProto.Streaming.JetStream;
using PinkSea.AtProto.Streaming.JetStream.Events;

namespace PinkSea.Services;

/// <summary>
/// The oekaki JetStream event handler.
/// </summary>
public class OekakiJetStreamEventHandler : IJetStreamEventHandler
{
    /// <inheritdoc />
    public async Task HandleEvent(JetStreamEvent @event)
    {
        Console.WriteLine($"Got an event of type {@event.Kind} for DID {@event.Did}");
    }
}