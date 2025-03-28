namespace PinkSea.AtProto.Shared.Xrpc;

/// <summary>
/// Signifies a successful or failed XRPC call invocation.
/// </summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
public class XrpcErrorOr<TResult> : IXrpcErrorOr
{
    /// <summary>
    /// The value of the XRPCErrorOr.
    /// </summary>
    public TResult? Value { get; init; }
    
    /// <inheritdoc />
    public XrpcError? Error { get; init; }
    
    /// <inheritdoc />
    public bool IsSuccess => Error is null;

    /// <inheritdoc />
    public object GetUnderlyingObject()
    {
        return IsSuccess
            ? Value!
            : Error!;
    }
    
    /// <summary>
    /// Returns a successful XRPC call.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The successful XrpcErrorOr.</returns>
    public static XrpcErrorOr<TResult> Ok(TResult value)
    {
        return new XrpcErrorOr<TResult>
        {
            Value = value
        };
    }

    /// <summary>
    /// Returns a failed XRPC call.
    /// </summary>
    /// <param name="error">The error type.</param>
    /// <param name="message">The error message (optional.)</param>
    /// <param name="statusCode">The status code to send back.</param>
    /// <returns>The failed XrpcErrorOr.</returns>
    public static XrpcErrorOr<TResult> Fail(
        string error,
        string? message = null,
        int? statusCode = 400)
    {
        return new XrpcErrorOr<TResult>
        {
            Error = new XrpcError
            {
                Error = error,
                Message = message,
                StatusCode = statusCode
            }
        };
    }
}

/// <summary>
/// Internal specification of the XrpcErrorOr type for the handler.
/// </summary>
public interface IXrpcErrorOr
{
    /// <summary>
    /// Whether this call is successful.
    /// </summary>
    bool IsSuccess { get; }
    
    /// <summary>
    /// The error.
    /// </summary>
    XrpcError? Error { get; }

    /// <summary>
    /// Gets either the error or the value.
    /// </summary>
    /// <returns>The underlying object.</returns>
    object GetUnderlyingObject();
}