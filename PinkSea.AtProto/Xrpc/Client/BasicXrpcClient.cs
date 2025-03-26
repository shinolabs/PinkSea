using System.Net.Http.Json;
using System.Web;
using Microsoft.Extensions.Logging;
using PinkSea.AtProto.Shared.Xrpc;
using PinkSea.AtProto.Xrpc.Extensions;

namespace PinkSea.AtProto.Xrpc.Client;

/// <summary>
/// A basic, authenticationless XRPC client.
/// </summary>
public sealed class BasicXrpcClient(
    HttpClient client,
    string host,
    ILogger logger) : IXrpcClient
{
    /// <inheritdoc />
    public async Task<XrpcErrorOr<TResponse>> Query<TResponse>(string nsid, object? parameters = null)
    {
        var actualEndpoint = $"{host}/xrpc/{nsid}";
        if (parameters is not null)
            actualEndpoint += $"?{parameters.ToQueryString()}";

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(actualEndpoint),
            Method = HttpMethod.Get,
        };
        
        var resp = await client.SendAsync(request);
        return await resp.ReadXrpcResponse<TResponse>(logger);
    }

    /// <inheritdoc />
    public async Task<XrpcErrorOr<TResponse>> Procedure<TResponse>(string nsid, object? parameters = null)
    {
        var actualEndpoint = $"{host}/xrpc/{nsid}";

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(actualEndpoint),
            Method = HttpMethod.Post,
        };

        if (parameters is not null)
            request.Content = JsonContent.Create(parameters);
        
        var resp = await client.SendAsync(request);
        return await resp.ReadXrpcResponse<TResponse>(logger);
    }

    /// <inheritdoc />
    public async Task<XrpcErrorOr<TResponse>> RawCall<TResponse>(string nsid, HttpContent bodyContent)
    {
        var actualEndpoint = $"{host}/xrpc/{nsid}";
        
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(actualEndpoint),
            Method = HttpMethod.Post,
            
            Content = bodyContent,
        };
        
        var resp = await client.SendAsync(request);
        return await resp.ReadXrpcResponse<TResponse>(logger);
    }
    
    /// <inheritdoc />
    public void Dispose()
    {
        client.Dispose();
    }
}