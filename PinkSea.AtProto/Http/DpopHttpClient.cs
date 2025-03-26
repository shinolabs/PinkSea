using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Providers.OAuth;

namespace PinkSea.AtProto.Http;

/// <summary>
/// A DPoP-capable HTTP client.
/// </summary>
public sealed class DpopHttpClient : IDisposable
{
    /// <summary>
    /// The HTTP client.
    /// </summary>
    private readonly HttpClient _client;

    /// <summary>
    /// A JWT signing provider.
    /// </summary>
    private readonly IJwtSigningProvider _jwtSigningProvider;

    /// <summary>
    /// The OAuth client data.
    /// </summary>
    private readonly OAuthClientData _clientData;

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger? _logger;

    /// <summary>
    /// The authorization header value.
    /// </summary>
    private string? _authorization;

    /// <summary>
    /// The authorization code.
    /// </summary>
    private string? _authorizationCode;

    /// <summary>
    /// The raw HTTP client.
    /// </summary>
    public HttpClient RawClient => _client;
    
    /// <summary>
    /// Constructs a new DPoP HTTP Client.
    /// </summary>
    public DpopHttpClient(
        HttpClient client,
        IJwtSigningProvider jwtSigningProvider,
        OAuthClientData clientData,
        ILogger? logger = null)
    {
        _client = client;
        _jwtSigningProvider = jwtSigningProvider;
        _clientData = clientData;
        _logger = logger;
    }

    /// <summary>
    /// Sets the authorization code.
    /// </summary>
    /// <param name="authorizationCode">The value.</param>
    public void SetAuthorizationCode(string authorizationCode)
    {
        var hash = SHA256.HashData(Encoding.ASCII.GetBytes(authorizationCode));
        _authorizationCode = Base64UrlEncoder.Encode(hash);
        _authorization = authorizationCode;
    }
    
    /// <summary>
    /// Sends a payload with DPoP enabled.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="value">The value to send.</param>
    /// <param name="keyPair">The keypair.</param>
    /// <typeparam name="TValue">The value of the type.</typeparam>
    /// <returns>The response.</returns>
    public async Task<HttpResponseMessage> Post<TValue>(
        string endpoint,
        TValue value,
        DpopKeyPair keyPair)
    {
        return await Send(
            endpoint,
            HttpMethod.Post,
            keyPair,
            value: JsonContent.Create(value));
    }
    
    /// <summary>
    /// Gets with DPoP enabled.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="keyPair">The keypair.</param>
    /// <returns>The response.</returns>
    public async Task<HttpResponseMessage> Get(
        string endpoint,
        DpopKeyPair keyPair)
    {
        return await Send(
            endpoint,
            HttpMethod.Get,
            keyPair);
    }
    
    /// <summary>
    /// Performs a raw send.
    /// </summary>
    /// <param name="endpoint">The endpoint we're sending to.</param>
    /// <param name="method">The method to send via.</param>
    /// <param name="keyPair">The DPoP keypair.</param>
    /// <param name="nonce">The DPoP nonce.</param>
    /// <param name="value">The value to add in the body.</param>
    /// <returns>The response from the server.</returns>
    public async Task<HttpResponseMessage> Send(
        string endpoint,
        HttpMethod method,
        DpopKeyPair keyPair,
        string? nonce = null,
        HttpContent? value = default)
    {
        var dpop = _jwtSigningProvider.GenerateDpopHeader(new DpopSigningData()
        {
            ClientId = _clientData.ClientId,
            Keypair = keyPair,
            Method = method.ToString().ToUpper(),
            Url = endpoint,
            Nonce = nonce,
            AuthenticationCodeHash = _authorizationCode
        });
        
        var request = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new Uri(endpoint),
            Headers =
            {
                { "DPoP", dpop }
            }
        };
        
        if (_authorization is not null)
            request.Headers.Add("Authorization", $"DPoP {_authorization}");

        if (method != HttpMethod.Get)
            request.Content = value;
        
        var resp = await _client.SendAsync(request);
        if ((resp.StatusCode != HttpStatusCode.BadRequest && resp.StatusCode != HttpStatusCode.Unauthorized) || nonce is not null)
            return resp;
        
        _logger?.LogWarning("Failed to fetch with DPoP: {Reason}",
            await resp.Content.ReadAsStringAsync());
        
        // Failed to send, maybe requires DPoP nonce?
        // Retry sending with the nonce.
        var dpopNonce = resp.Headers.GetValues("DPoP-Nonce")?
            .FirstOrDefault();
            
        // We don't have the nonce, we can quit.
        if (dpopNonce is null)
            return resp;

        return await Send(
            endpoint,
            method,
            keyPair,
            dpopNonce,
            value);
    }
    
    /// <inheritdoc />
    public void Dispose()
    {
        _client.Dispose();
    }
}