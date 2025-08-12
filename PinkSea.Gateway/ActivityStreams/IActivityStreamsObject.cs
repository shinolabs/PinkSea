namespace PinkSea.Gateway.ActivityStreams;

/// <summary>
/// An interface for an activity streams object.
/// </summary>
public interface IActivityStreamsObject
{
    /// <summary>
    /// The type of the object.
    /// </summary>
    string Type { get; }
}