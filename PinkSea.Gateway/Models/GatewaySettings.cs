namespace PinkSea.Gateway.Models;

/// <summary>
/// The PinkSea Gateway settings.
/// </summary>
public class GatewaySettings
{
    /// <summary>
    /// The PinkSea API endpoint.
    /// </summary>
    public required string PinkSeaEndpoint { get; init; }
}