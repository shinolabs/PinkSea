using PinkSea.AtProto.Http;
using PinkSea.AtProto.Models.Authorization;
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
    IOAuthClientDataProvider clientDataProvider) : IXrpcClientFactory
{
    /// <inheritdoc />
    public async Task<IXrpcClient?> GetForOAuthStateId(string stateId)
    {
        var oauthState = await stateStorageProvider.GetForStateId(stateId);
        if (oauthState?.AuthorizationCode is null)
            return null;

        var httpClient = httpClientFactory.CreateClient("xrpc-client");

        if (oauthState.AuthorizationType == AuthorizationType.OAuth2)
        {
            var dpopClient = new DpopHttpClient(httpClient, jwtSigningProvider, clientDataProvider.ClientData);
            dpopClient.SetAuthorizationCode(oauthState.AuthorizationCode);
            return new DPopXrpcClient(dpopClient, oauthState);
        }
        
        return new SessionXrpcClient(httpClient, oauthState);
    }
}