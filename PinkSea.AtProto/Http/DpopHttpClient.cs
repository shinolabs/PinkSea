using System.Net;
using System.Net.Http.Json;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Providers.OAuth;

namespace PinkSea.AtProto.Http;

/// <summary>
/// A DPoP-capable HTTP client.
/// </summary>
public class DpopHttpClient : IDisposable
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
    /// The authorization header value.
    /// </summary>
    private string? _authorization;

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
        OAuthClientData clientData)
    {
        _client = client;
        _jwtSigningProvider = jwtSigningProvider;
        _clientData = clientData;
    }

    /// <summary>
    /// Sets the authorization header.
    /// </summary>
    /// <param name="authorization">The value.</param>
    public void SetAuthorizationHeader(string authorization)
    {
        _authorization = authorization;
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
            value: value);
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
        return await Send<object>(
            endpoint,
            HttpMethod.Get,
            keyPair);
    }
    
    private async Task<HttpResponseMessage> Send<TValue>(
        string endpoint,
        HttpMethod method,
        DpopKeyPair keyPair,
        string? nonce = null,
        TValue? value = default)
    {
        var dpop = _jwtSigningProvider.GenerateDpopHeader(new DpopSigningData()
        {
            ClientId = _clientData.ClientId,
            Keypair = keyPair,
            Method = method.ToString().ToUpper(),
            Url = endpoint,
            Nonce = nonce
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
            request.Content = JsonContent.Create(value);
        
        var resp = await _client.SendAsync(request);
        if (resp.StatusCode != HttpStatusCode.BadRequest || nonce is not null)
            return resp;
        
        Console.WriteLine($"Failed to fetch with DPoP: {await resp.Content.ReadAsStringAsync()}");
        
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
    
    public void Dispose()
    {
        _client.Dispose();
    }
}