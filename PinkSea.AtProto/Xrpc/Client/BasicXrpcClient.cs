using System.Net.Http.Json;
using System.Text.Json;
using System.Web;

namespace PinkSea.AtProto.Xrpc.Client;

/// <summary>
/// A basic, authenticationless XRPC client.
/// </summary>
public sealed class BasicXrpcClient(
    HttpClient client,
    string host) : IXrpcClient
{
    /// <inheritdoc />
    public async Task<TResponse?> Query<TResponse>(string nsid, object? parameters = null)
    {
        var actualEndpoint = $"{host}/xrpc/{nsid}";
        if (parameters is not null)
            actualEndpoint += $"?{ObjectToQueryParams(parameters)}";

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(actualEndpoint),
            Method = HttpMethod.Get,
        };
        
        var resp = await client.SendAsync(request);
        if (!resp.IsSuccessStatusCode)
            return default;

        if (typeof(TResponse) == resp.GetType())
            return (TResponse)(object)resp;
        
        var str = await resp.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(str);
    }

    /// <inheritdoc />
    public async Task<TResponse?> Procedure<TResponse>(string nsid, object? parameters = null)
    {
        var actualEndpoint = $"{host}/xrpc/{nsid}";

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(actualEndpoint),
            Method = HttpMethod.Post,
        };

        if (parameters is not null)
            request.Content = JsonContent.Create(parameters);
        
        var resp = await client.SendAsync(request);
        
        var str = await resp.Content.ReadAsStringAsync();
        
        return resp.IsSuccessStatusCode
            ? JsonSerializer.Deserialize<TResponse>(str)
            : default;
    }

    /// <inheritdoc />
    public async Task<TResponse?> RawCall<TResponse>(string nsid, HttpContent bodyContent)
    {
        var actualEndpoint = $"{host}/xrpc/{nsid}";
        
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(actualEndpoint),
            Method = HttpMethod.Post,
            
            Content = bodyContent,
        };
        
        var resp = await client.SendAsync(request);
        var str = await resp.Content.ReadAsStringAsync();
        
        return resp.IsSuccessStatusCode
            ? JsonSerializer.Deserialize<TResponse>(str)
            : default;
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
    private static string ObjectToQueryParams(object obj)
    {
        var props = from p in obj.GetType().GetProperties()
            where p.GetValue(obj, null) != null
            select p.Name.ToLowerInvariant() + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null)!.ToString());

        return string.Join('&', props.ToArray());
    }
}