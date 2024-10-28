using PinkSea.AtProto.Http;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Providers.OAuth;
using PinkSea.AtProto.Providers.Storage;

namespace PinkSea.AtProto.Xrpc;

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
        if (oauthState is null)
            return null;

        var httpClient = httpClientFactory.CreateClient("xrpc-client");
        var dpopClient = new DpopHttpClient(httpClient, jwtSigningProvider, clientDataProvider.ClientData);

        return new XrpcClient(dpopClient, oauthState.KeyPair);
    }
}