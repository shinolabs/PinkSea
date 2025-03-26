using System.Text.Json;
using Microsoft.Extensions.Logging;
using PinkSea.AtProto.Shared.Xrpc;

namespace PinkSea.AtProto.Xrpc.Extensions;

/// <summary>
/// Extension class for the HTTP response message class.
/// </summary>
public static class HttpResponseMessageExtensions
{
    /// <summary>
    /// Reads an XRPC response from this HTTP response. 
    /// </summary>
    /// <param name="message">The HTTP response.</param>
    /// <param name="logger">Optional logger for this.</param>
    /// <typeparam name="TResponse">The internal type of the response.</typeparam>
    /// <returns>The XRPC error or a value.</returns>
    public static async Task<XrpcErrorOr<TResponse>> ReadXrpcResponse<TResponse>(
        this HttpResponseMessage message,
        ILogger? logger = null)
    {
        var body = await message.Content.ReadAsStringAsync();
        if (!message.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<XrpcError>(body) ?? new XrpcError
            {
                Error = "UnknownError",
                Message = body
            };
            
            logger?.LogWarning("XRPC call to {Endpoint} returned an error: [{Type}] > {Message}",
                message.RequestMessage?.RequestUri,
                error.Error,
                error.Message);

            return XrpcErrorOr<TResponse>.Fail(error.Error, error.Message);
        }
        
        logger?.LogInformation("XRPC call to {Endpoint} ended with a success.",
            message.RequestMessage?.RequestUri);
        
        if (message is TResponse typedResponse)
            return XrpcErrorOr<TResponse>.Ok(typedResponse);

        var result = JsonSerializer.Deserialize<TResponse>(body);
        return XrpcErrorOr<TResponse>.Ok(result!);
    }
}