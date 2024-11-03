using PinkSea.AtProto.Streaming.JetStream.Events;

namespace PinkSea.AtProto.Streaming.JetStream;

/// <summary>
/// A JetStream event handler.
/// </summary>
public interface IJetStreamEventHandler
{
    /// <summary>
    /// Handles a JetStream event.
    /// </summary>
    /// <param name="event">The event.</param>
    Task HandleEvent(JetStreamEvent @event);
}