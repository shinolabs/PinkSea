namespace PinkSea.Gateway.Models;

/// <summary>
/// The PinkSea Gateway settings.
/// </summary>
public class GatewaySettings
{
    /// <summary>
    /// The PinkSea API endpoint.
    /// </summary>
    public required string AppViewEndpoint { get; init; }
    
    /// <summary>
    /// The endpoint for the frontend.
    /// </summary>
    public required string FrontEndEndpoint { get; init; }
}