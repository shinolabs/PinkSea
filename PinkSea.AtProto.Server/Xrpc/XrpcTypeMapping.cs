namespace PinkSea.AtProto.Server.Xrpc;

/// <summary>
/// An XRPC type mapping.
/// </summary>
/// <param name="RequestType">The request type.</param>
/// <param name="ResponseType">The response type.</param>
public record XrpcTypeMapping(
    Type RequestType,
    Type ResponseType,
    Type HandlerType,
    bool IsQuery);
