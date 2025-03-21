namespace PinkSea.AtProto.Xrpc.Client;

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

    /// <summary>
    /// Gets an XRPC client without any kind of authentication.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <returns>The XRPC client.</returns>
    Task<IXrpcClient> GetWithoutAuthentication(string host);
}