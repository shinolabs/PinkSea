using Microsoft.Extensions.Logging;
using PinkSea.AtProto.Http;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.Shared.Xrpc;
using PinkSea.AtProto.Xrpc.Extensions;

namespace PinkSea.AtProto.Xrpc.Client;

/// <summary>
/// An XRPC client.
/// </summary>
public class DPopXrpcClient(
    DpopHttpClient client,
    OAuthState clientState,
    ILogger logger)
    : IXrpcClient
{
    /// <inheritdoc />
    public async Task<XrpcErrorOr<TResponse>> Query<TResponse>(
        string nsid,
        object? parameters = null)
    {
        var actualEndpoint = $"{clientState.Pds}/xrpc/{nsid}";
        if (parameters is not null)
            actualEndpoint += $"?{parameters.ToQueryString()}";

        var resp = await client.Get(actualEndpoint, clientState.KeyPair);
        return await resp.ReadXrpcResponse<TResponse>();
    }

    /// <inheritdoc />
    public async Task<XrpcErrorOr<TResponse>> Procedure<TResponse>(
        string nsid,
        object? parameters = null)
    {
        var actualEndpoint = $"{clientState.Pds}/xrpc/{nsid}";
        var resp = await client.Post(actualEndpoint, parameters, clientState.KeyPair);
        return await resp.ReadXrpcResponse<TResponse>();
    }

    /// <inheritdoc />
    public async Task<XrpcErrorOr<TResponse>> RawCall<TResponse>(string nsid, HttpContent bodyContent)
    {
        var actualEndpoint = $"{clientState.Pds}/xrpc/{nsid}";
        var resp = await client.Send(
            actualEndpoint,
            HttpMethod.Post,
            clientState.KeyPair,
            value: bodyContent);

        return await resp.ReadXrpcResponse<TResponse>(logger);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        client.Dispose();
    }
}