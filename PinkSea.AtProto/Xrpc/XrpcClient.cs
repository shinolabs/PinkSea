using PinkSea.AtProto.OAuth;

namespace PinkSea.AtProto.Xrpc;

/// <summary>
/// An Xrpc client.
/// </summary>
public class XrpcClient(
    IAtProtoOAuthClient atProtoOAuthClient,
    IHttpClientFactory httpClientFactory)
    : IXrpcClient, IDisposable
{
    /// <summary>
    /// The http client.
    /// </summary>
    private readonly HttpClient _client = httpClientFactory.CreateClient("xrpc-client");
    
    /// <inheritdoc />
    public Task<TResponse?> Query<TResponse>(
        string pds,
        string nsid,
        object? parameters = null,
        string? token = null)
    {
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