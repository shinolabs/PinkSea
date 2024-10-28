namespace PinkSea.AtProto.Xrpc;

/// <summary>
/// A factory for <see cref="IXrpcClient"/>
/// </summary>
public interface IXrpcClientFactory
{
    /// <summary>
    /// Gets an XRPC client for a state id.
    /// </summary>
    /// <param name="stateId">The state id.</param>
    /// <returns>The XRPC client.</returns>
    Task<IXrpcClient?> GetForOAuthStateId(string stateId);
}