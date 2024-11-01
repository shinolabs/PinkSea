using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Providers.OAuth;
using PinkSea.Models;

namespace PinkSea.Services;

/// <summary>
/// The PinkSea client data provider.
/// </summary>
public class OAuthClientDataProvider(
    SigningKeyService signingKeyService,
    IOptions<AppViewConfig> appViewConfig) 
    : IOAuthClientDataProvider
{
    /// <inheritdoc />
    public OAuthClientData ClientData => new()
    {
        ClientId = ClientMetadata.ClientId,
        RedirectUri = ClientMetadata.RedirectUris[0],
        Scope = ClientMetadata.Scope,
        Key = new JwtKey
        {
            SigningCredentials = new SigningCredentials(signingKeyService.SecurityKey, SecurityAlgorithms.EcdsaSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            },
            KeyId = signingKeyService.KeyId
        }
    };
    
    /// <inheritdoc />
    public ClientMetadata ClientMetadata => new()
    {
        ClientId = $"{appViewConfig.Value.AppUrl}/oauth/client-metadata.json",
        ClientName = "PinkSea",
        ApplicationType = "web",
        DpopBoundAccessTokens = true,
        GrantTypes =
        [
            "authorization_code"
        ],
        RedirectUris =
        [
            $"{appViewConfig.Value.AppUrl}/oauth/callback"
        ],
        Scope = "atproto transition:generic",
        TokenEndpointAuthMethod = "private_key_jwt",
        TokenEndpointAuthSigningAlgorithm = "ES256",
        JwksUri = $"{appViewConfig.Value.AppUrl}/oauth/jwks.json"
    };
}