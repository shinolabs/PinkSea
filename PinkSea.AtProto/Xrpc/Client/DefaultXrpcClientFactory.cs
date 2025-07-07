using Microsoft.Extensions.Logging;
using PinkSea.AtProto.Http;
using PinkSea.AtProto.Models.Authorization;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Providers.OAuth;
using PinkSea.AtProto.Providers.Storage;

namespace PinkSea.AtProto.Xrpc.Client;

/// <summary>
/// The default XRPC client factory implementation.
/// </summary>
public class DefaultXrpcClientFactory(
    IOAuthStateStorageProvider stateStorageProvider,
    IHttpClientFactory httpClientFactory,
    IJwtSigningProvider jwtSigningProvider,
    IOAuthClientDataProvider clientDataProvider,
    ILoggerFactory loggerFactory) : IXrpcClientFactory
{
    /// <inheritdoc />
    public async Task<IXrpcClient?> GetForOAuthStateId(string stateId)
    {
        var oauthState = await stateStorageProvider.GetForStateId(stateId);
        if (oauthState?.AuthorizationCode is null)
            return null;

        return await GetForOAuthState(oauthState);
    }

    /// <inheritdoc />
    public async Task<IXrpcClient?> GetForOAuthState(OAuthState oauthState)
    {
        var xrpcLogger = loggerFactory.CreateLogger<IXrpcClient>();
        var httpClient = httpClientFactory.CreateClient("xrpc-client");

        if (oauthState.AuthorizationType == AuthorizationType.OAuth2)
        {
            var dpopClientLogger = loggerFactory.CreateLogger<DpopHttpClient>();
            var dpopClient = new DpopHttpClient(
                httpClient,
                jwtSigningProvider,
                clientDataProvider.ClientData,
                dpopClientLogger);
            
            dpopClient.SetAuthorizationCode(oauthState.AuthorizationCode!);
            return new DPopXrpcClient(dpopClient, oauthState, xrpcLogger);
        }
        
        return new SessionXrpcClient(httpClient, oauthState, xrpcLogger);
    }

    /// <inheritdoc />
    public Task<IXrpcClient> GetWithoutAuthentication(string host)
    {
        var xrpcLogger = loggerFactory.CreateLogger<IXrpcClient>();
        var httpClient = httpClientFactory.CreateClient("xrpc-client");
        return Task.FromResult<IXrpcClient>(new BasicXrpcClient(httpClient, host, xrpcLogger));
    }
}