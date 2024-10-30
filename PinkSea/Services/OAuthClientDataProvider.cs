using Microsoft.IdentityModel.Tokens;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Providers.OAuth;

namespace PinkSea.Services;

/// <summary>
/// The PinkSea client data provider.
/// </summary>
public class OAuthClientDataProvider(SigningKeyService signingKeyService) 
    : IOAuthClientDataProvider
{
    /// <summary>
    /// The base URL of the service.
    /// </summary>
    private const string BaseUrl = "https://f03d5bef0a83f8.lhr.life";
    
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
        ClientId = $"{BaseUrl}/oauth/client-metadata.json",
        ClientName = "PinkSea",
        ApplicationType = "web",
        DpopBoundAccessTokens = true,
        GrantTypes =
        [
            "authorization_code"
        ],
        RedirectUris =
        [
            $"{BaseUrl}/oauth/callback"
        ],
        Scope = "atproto transition:generic",
        TokenEndpointAuthMethod = "private_key_jwt",
        TokenEndpointAuthSigningAlgorithm = "ES256",
        JwksUri = $"{BaseUrl}/oauth/jwks.json"
    };
}