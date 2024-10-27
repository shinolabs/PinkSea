namespace PinkSea.AtProto.Xrpc;

/// <summary>
/// The XRPC client.
/// </summary>
public interface IXrpcClient
{
    /// <summary>
    /// Queries a PDS.
    /// </summary>
    /// <param name="pds">The pds address.</param>
    /// <param name="nsid">The NSID.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="state">The state id.</param>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <returns>The response, if it exists.</returns>
    Task<TResponse?> Query<TResponse>(
        string pds,
        string nsid,
        object? parameters = null,
        string? state = null);

    /// <summary>
    /// Executes a procedure on a PDS.
    /// </summary>
    /// <param name="pds">The pds address.</param>
    /// <param name="nsid">The NSID.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="state">The state id.</param>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <returns>The response, if it exists.</returns>
    Task<TResponse?> Procedure<TResponse>(
        string pds,
        string nsid,
        object? parameters = null,
        string? state = null);
}