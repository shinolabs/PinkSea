using PinkSea.AtProto.Shared.Xrpc;

namespace PinkSea.AtProto.Xrpc.Client;

/// <summary>
/// The XRPC client.
/// </summary>
public interface IXrpcClient : IDisposable
{
    /// <summary>
    /// Queries a PDS.
    /// </summary>
    /// <param name="nsid">The NSID.</param>
    /// <param name="parameters">The parameters.</param>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <returns>The response, if it exists.</returns>
    Task<XrpcErrorOr<TResponse>> Query<TResponse>(
        string nsid,
        object? parameters = null);

    /// <summary>
    /// Executes a procedure on a PDS.
    /// </summary>
    /// <param name="nsid">The NSID.</param>
    /// <param name="parameters">The parameters.</param>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <returns>The response, if it exists.</returns>
    Task<XrpcErrorOr<TResponse>> Procedure<TResponse>(
        string nsid,
        object? parameters = null);
    
    /// <summary>
    /// A raw XRPC call, without any serialization.
    /// </summary>
    /// <param name="nsid">The NSID.</param>
    /// <param name="bodyContent">The body content.</param>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <returns>The response, if it exists.</returns>
    Task<XrpcErrorOr<TResponse>> RawCall<TResponse>(
        string nsid,
        HttpContent bodyContent);
}