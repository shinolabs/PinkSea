using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Web;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.Providers.OAuth;
using PinkSea.AtProto.Providers.Storage;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Resolvers.Domain;

namespace PinkSea.AtProto.OAuth;

/// <summary>
/// The AT Protocol OAuth client.
/// </summary>
public class AtProtoOAuthClient(
    IHttpClientFactory httpClientFactory,
    IDomainDidResolver domainDidResolver,
    IDidResolver didResolver,
    IJwtSigningProvider jwtSigningProvider,
    IOAuthStateStorageProvider oAuthStateStorageProvider) : IAtProtoOAuthClient, IDisposable
{
    /// <summary>
    /// The HTTP client used for the OAuth client.
    /// </summary>
    private readonly HttpClient _client = httpClientFactory.CreateClient("oauth-client");

    /// <inheritdoc />
    public async Task<string?> GetOAuthRequestUriForHandle(string handle, OAuthClientData clientData)
    {
        var did = handle;
        if (!did.StartsWith("did"))
            did = await domainDidResolver.GetDidForDomainHandle(handle);

        if (did is null)
            return null;

        var authServer = await GetOAuthAuthorizationServerDataForDid(did);
        
        var assertion = jwtSigningProvider.GenerateClientAssertion(new JwtSigningData()
        {
            ClientId = clientData.ClientId,
            Audience = authServer!.Issuer,
            Key = clientData.Key
        });

        var keyPair = GenerateDPopKeypair();
        var state = GenerateRandomState();
        
        var body = new AuthorizationRequest()
        {
            ClientId = clientData.ClientId,
            ResponseType = "code",
            Scope = clientData.Scope,
            RedirectUrl = clientData.RedirectUri,
            State = state,
            CodeChallenge = "a",
            CodeChallengeMethod = "S256",
            ClientAssertionType = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer",
            ClientAssertion = assertion
        };
        
        var resp = await SendWithDpop(authServer!.PushedAuthorizationRequestEndpoint!, body, clientData, keyPair);
        if (!resp.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to send a PAR: {await resp.Content.ReadAsStringAsync()}");
            return null;
        }

        var parResponse = await resp.Content.ReadFromJsonAsync<PushedAuthorizationRequestResponse>();

        var finalUrl = new UriBuilder(authServer!.AuthorizationEndpoint!);
        var query = HttpUtility.ParseQueryString(finalUrl.Query);
        query["client_id"] = body.ClientId;
        query["request_uri"] = parResponse!.RequestUri;
        finalUrl.Query = query.ToString();

        await oAuthStateStorageProvider.SetForStateId(state,
            new OAuthState
            {
                Did = did,
                PkceString = "a",
                Issuer = authServer.Issuer,
                KeyPair = keyPair
            });
        
        return finalUrl.ToString();
    }

    /// <inheritdoc />
    public async Task<ProtectedResource?> GetOAuthProtectedResourceForPds(string pds)
    {
        const string wellKnownUrl = "/.well-known/oauth-protected-resource";
        return await _client.GetFromJsonAsync<ProtectedResource>($"{pds}{wellKnownUrl}");
    }

    /// <inheritdoc />
    public async Task<AuthorizationServer?> GetOAuthAuthorizationServerDataForAuthorizationServer(string authServer)
    {
        const string wellKnownUrl = "/.well-known/oauth-authorization-server";
        return await _client.GetFromJsonAsync<AuthorizationServer>($"{authServer}{wellKnownUrl}");
    }

    /// <summary>
    /// Gets an OAuth Authorization Server data for a given DID.
    /// </summary>
    /// <param name="did">The DID.</param>
    /// <returns>The authorization server data.</returns>
    private async Task<AuthorizationServer?> GetOAuthAuthorizationServerDataForDid(string did)
    {
        var resolved = await didResolver.GetDidResponseForDid(did!);
        var pds = resolved?.GetPds();
        if (pds is null)
            return null;
        
        var protectedResource = await GetOAuthProtectedResourceForPds(pds);
        var authServer = protectedResource?.GetAuthorizationServer();
        if (authServer is null)
            return null;

        return await GetOAuthAuthorizationServerDataForAuthorizationServer(authServer);
    }

    /// <summary>
    /// Generates a DPoP keypair.
    /// </summary>
    /// <returns>The DPoP keypair.</returns>
    private DpopKeyPair GenerateDPopKeypair()
    {
        using var ecdsa = ECDsa.Create(ECCurve.CreateFromFriendlyName("nistp256"));
        return new DpopKeyPair
        {
            PublicKey = ecdsa.ExportSubjectPublicKeyInfoPem(),
            PrivateKey = ecdsa.ExportECPrivateKeyPem()
        };
    }

    /// <summary>
    /// Generates a random state string.
    /// </summary>
    /// <returns>The state string.</returns>
    private string GenerateRandomState()
    {
        return RandomNumberGenerator.GetHexString(64, true);
    }

    /// <summary>
    /// Sends a payload with DPoP enabled.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="value">The value to send.</param>
    /// <param name="clientData">The client data.</param>
    /// <param name="keyPair">The keypair.</param>
    /// <param name="nonce">The DPoP nonce.</param>
    /// <typeparam name="TValue">The value of the type.</typeparam>
    /// <returns>The response.</returns>
    private async Task<HttpResponseMessage> SendWithDpop<TValue>(
        string endpoint,
        TValue value,
        OAuthClientData clientData,
        DpopKeyPair keyPair,
        string? nonce = null)
    {
        var dpop = jwtSigningProvider.GenerateDpopHeader(new DpopSigningData()
        {
            ClientId = clientData.ClientId,
            Keypair = keyPair,
            Method = "POST",
            Url = endpoint,
            Nonce = nonce
        });
        
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(endpoint),
            Content = JsonContent.Create(value),
            Headers =
            {
                { "DPoP", dpop }
            }
        };

        var resp = await _client.SendAsync(request);
        if (resp.StatusCode != HttpStatusCode.BadRequest || nonce is not null)
            return resp;
        
        // Failed to send, maybe requires DPoP nonce?
        // Retry sending with the nonce.
        var dpopNonce = resp.Headers.GetValues("DPoP-Nonce")?
            .FirstOrDefault();
            
        // We don't have the nonce, we can quit.
        if (dpopNonce is null)
            return resp;

        return await SendWithDpop(
            endpoint,
            value,
            clientData,
            keyPair,
            dpopNonce);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _client.Dispose();
    }
}