namespace PinkSea.AtProto.Server.Xrpc;

/// <summary>
/// An XRPC procedure.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IXrpcProcedure<TRequest, TResponse>
    : IXrpcRequestHandler<TRequest, TResponse>
{
    
}