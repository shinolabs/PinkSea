using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using PinkSea.AtProto.Models.OAuth;

namespace PinkSea.AtProto.Xrpc.Client;

/// <summary>
/// A session-based XRPC client. (Not OAuth2.)
/// </summary>
/// <param name="httpClientFactory"></param>
/// <param name="clientState"></param>
public class SessionXrpcClient(
    HttpClient client,
    OAuthState clientState) : IXrpcClient
{
    /// <inheritdoc />
    public async Task<TResponse?> Query<TResponse>(string nsid, object? parameters = null)
    {
        var actualEndpoint = $"{clientState.Pds}/xrpc/{nsid}";
        if (parameters is not null)
            actualEndpoint += $"?{ObjectToQueryParams(parameters)}";

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(actualEndpoint),
            Method = HttpMethod.Get,

            Headers =
            {
                { "Authorization", $"Bearer {clientState.AuthorizationCode}" }
            }
        };
        
        var resp = await client.SendAsync(request);
        if (resp.IsSuccessStatusCode)
        {
            var str = await resp.Content.ReadAsStringAsync();
            Console.WriteLine($"Got back data from the PDS: {str}");
            return JsonSerializer.Deserialize<TResponse>(str);
        }

        return default;
    }

    /// <inheritdoc />
    public async Task<TResponse?> Procedure<TResponse>(string nsid, object? parameters = null)
    {
        // Hack but as long as it works :3
        const string refreshNsid = "com.atproto.server.refreshSession";
        var authCode = nsid == refreshNsid
            ? clientState.RefreshToken
            : clientState.AuthorizationCode;
        
        var actualEndpoint = $"{clientState.Pds}/xrpc/{nsid}";

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(actualEndpoint),
            Method = HttpMethod.Post,
            Headers =
            {
                { "Authorization", $"Bearer {authCode}" }
            }
        };

        if (parameters is not null)
            request.Content = JsonContent.Create(parameters);
        
        var resp = await client.SendAsync(request);
        
        var str = await resp.Content.ReadAsStringAsync();
        Console.WriteLine($"Got back data from the PDS: {str}");
        
        if (resp.IsSuccessStatusCode)
            return JsonSerializer.Deserialize<TResponse>(str);

        return default;
    }

    /// <inheritdoc />
    public async Task<TResponse?> RawCall<TResponse>(string nsid, HttpContent bodyContent)
    {
        var actualEndpoint = $"{clientState.Pds}/xrpc/{nsid}";
        
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(actualEndpoint),
            Method = HttpMethod.Post,
            
            Content = bodyContent,

            Headers =
            {
                { "Authorization", $"Bearer {clientState.AuthorizationCode}" }
            }
        };
        
        var resp = await client.SendAsync(request);
        
        var str = await resp.Content.ReadAsStringAsync();
        Console.WriteLine($"Got back data from the PDS: {str}");
        
        if (resp.IsSuccessStatusCode)
            return JsonSerializer.Deserialize<TResponse>(str);

        return default;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        client.Dispose();
    }
    
    /// <summary>
    /// Converts an object to a query string.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>The resulting query string.</returns>
    private string ObjectToQueryParams(object obj)
    {
        var props = from p in obj.GetType().GetProperties()
            where p.GetValue(obj, null) != null
            select p.Name.ToLowerInvariant() + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

        return string.Join('&', props.ToArray());
    }
}