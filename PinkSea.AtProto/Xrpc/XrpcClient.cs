using PinkSea.AtProto.Http;
using PinkSea.AtProto.Providers.OAuth;

namespace PinkSea.AtProto.Xrpc;

/// <summary>
/// An Xrpc client.
/// </summary>
public class XrpcClient(
    IHttpClientFactory httpClientFactory,
    IJwtSigningProvider jwtSigningProvider)
    : IXrpcClient, IDisposable
{
    /// <summary>
    /// The http client.
    /// </summary>
    private readonly DpopHttpClient _client = new(
        httpClientFactory.CreateClient("xrpc-client"),
        jwtSigningProvider);
    
    /// <inheritdoc />
    public Task<TResponse?> Query<TResponse>(
        string pds,
        string nsid,
        object? parameters = null,
        string? token = null)
    {
        var actualEndpoint = $"{pds}/xrpc/{nsid}";
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<TResponse?> Procedure<TResponse>(
        string pds,
        string nsid,
        object? parameters = null,
        string? token = null)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _client.Dispose();
    }
}