namespace PinkSea.AtProto.Server.Xrpc;

/// <summary>
/// An XRPC query.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IXrpcQuery<TRequest, TResponse>
    : IXrpcRequestHandler<TRequest, TResponse>
{
    
}