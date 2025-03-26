using PinkSea.AtProto.Shared.Xrpc;

namespace PinkSea.AtProto.Server.Xrpc;

/// <summary>
/// A generic base XRPC request handler.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IXrpcRequestHandler<TRequest, TResponse>
    : IXrpcRequestHandler
{
    Task<XrpcErrorOr<TResponse>> Handle(TRequest request);
}

/// <summary>
/// The base XRPC request handler type, only for finding which type inherits from which.
/// </summary>4
public interface IXrpcRequestHandler
{
    
}