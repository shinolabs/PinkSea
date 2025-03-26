using PinkSea.AtProto.Shared.Xrpc;

namespace PinkSea.AtProto.Server.Xrpc;

/// <summary>
/// A server XRPC handler.
/// </summary>
public interface IXrpcHandler
{
    /// <summary>
    /// Handles the XRPC using the http context.
    /// </summary>
    /// <param name="nsid">The namespace ID.</param>
    /// <param name="context">The HTTP context.</param>
    /// <returns>The response from the handler.</returns>
    Task<IXrpcErrorOr?> HandleXrpc(
        string nsid,
        HttpContext context);
}