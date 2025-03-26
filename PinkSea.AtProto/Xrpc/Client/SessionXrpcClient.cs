using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Logging;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.Shared.Xrpc;
using PinkSea.AtProto.Xrpc.Extensions;

namespace PinkSea.AtProto.Xrpc.Client;

/// <summary>
/// A session-based XRPC client. (Not OAuth2.)
/// </summary>
/// <param name="httpClientFactory"></param>
/// <param name="clientState"></param>
public class SessionXrpcClient(
    HttpClient client,
    OAuthState clientState,
    ILogger logger) : IXrpcClient
{
    /// <inheritdoc />
    public async Task<XrpcErrorOr<TResponse>> Query<TResponse>(string nsid, object? parameters = null)
    {
        var actualEndpoint = $"{clientState.Pds}/xrpc/{nsid}";
        if (parameters is not null)
            actualEndpoint += $"?{parameters.ToQueryString()}";

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(actualEndpoint),
            Method = HttpMethod.Get,

            Headers =
            {
                { "Authorization", $"Bearer {clientState.AuthorizationCode}" }
            }
        };
        
        var resp = await client.SendAsync(request);
        return await resp.ReadXrpcResponse<TResponse>(logger);
    }

    /// <inheritdoc />
    public async Task<XrpcErrorOr<TResponse>> Procedure<TResponse>(string nsid, object? parameters = null)
    {
        // Hack but as long as it works :3
        const string refreshNsid = "com.atproto.server.refreshSession";
        var authCode = nsid == refreshNsid
            ? clientState.RefreshToken
            : clientState.AuthorizationCode;
        
        var actualEndpoint = $"{clientState.Pds}/xrpc/{nsid}";

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(actualEndpoint),
            Method = HttpMethod.Post,
            Headers =
            {
                { "Authorization", $"Bearer {authCode}" }
            }
        };

        if (parameters is not null)
            request.Content = JsonContent.Create(parameters);
        
        var resp = await client.SendAsync(request);
        return await resp.ReadXrpcResponse<TResponse>(logger);
    }

    /// <inheritdoc />
    public async Task<XrpcErrorOr<TResponse>> RawCall<TResponse>(string nsid, HttpContent bodyContent)
    {
        var actualEndpoint = $"{clientState.Pds}/xrpc/{nsid}";
        
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(actualEndpoint),
            Method = HttpMethod.Post,
            
            Content = bodyContent,

            Headers =
            {
                { "Authorization", $"Bearer {clientState.AuthorizationCode}" }
            }
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