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
    /// The raw HTTP client.
    /// </summary>
    public HttpClient RawClient => _client;
    
    /// <summary>
    /// Constructs a new DPoP HTTP Client.
    /// </summary>
    public DpopHttpClient(
        HttpClient client,
        IJwtSigningProvider jwtSigningProvider)
    {
        _client = client;
        _jwtSigningProvider = jwtSigningProvider;
    }
    
    /// <summary>
    /// Sends a payload with DPoP enabled.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="value">The value to send.</param>
    /// <param name="clientData">The client data.</param>
    /// <param name="keyPair">The keypair.</param>
    /// <typeparam name="TValue">The value of the type.</typeparam>
    /// <returns>The response.</returns>
    public async Task<HttpResponseMessage> Post<TValue>(
        string endpoint,
        TValue value,
        OAuthClientData clientData,
        DpopKeyPair keyPair)
    {
        return await Send(
            endpoint,
            HttpMethod.Post,
            clientData,
            keyPair,
            value: value);
    }
    
    private async Task<HttpResponseMessage> Send<TValue>(
        string endpoint,
        HttpMethod method,
        OAuthClientData clientData,
        DpopKeyPair keyPair,
        string? nonce = null,
        TValue? value = default)
    {
        var dpop = _jwtSigningProvider.GenerateDpopHeader(new DpopSigningData()
        {
            ClientId = clientData.ClientId,
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

        if (method != HttpMethod.Get)
            request.Content = JsonContent.Create(value);
        
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

        return await Send(
            endpoint,
            method,
            clientData,
            keyPair,
            dpopNonce,
            value);
    }
    
    public void Dispose()
    {
        _client.Dispose();
    }
}