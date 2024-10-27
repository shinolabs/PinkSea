using DnsClient;

namespace PinkSea.AtProto.Resolvers.Domain;

/// <summary>
/// A generic handle to DID resolver.
/// </summary>
public class DomainDidResolver(
    LookupClient lookupClient,
    IHttpClientFactory httpClientFactory) : IDomainDidResolver
{
    /// <inheritdoc />
    public async Task<string?> GetDidForDomainHandle(string handle)
    {
        // First try to resolve via DNS.
        var dns = await TryResolveDidThroughDnsTxt(handle);
        if (dns is not null)
            return dns;
        
        // Then try to resolve via well-known
        var wellKnown = await TryResolveDidThroughWellKnown(handle);
        return wellKnown;
    }

    /// <summary>
    /// Resolves a handle via the DNS method.
    /// </summary>
    /// <param name="handle">The handle.</param>
    /// <returns>The DID, or nothing.</returns>
    private async Task<string?> TryResolveDidThroughDnsTxt(string handle)
    {
        const string domainPrelude = "_atproto.";
        var domain = $"{domainPrelude}{handle}";

        var query = await lookupClient.QueryAsync(domain, QueryType.TXT);
        var record = query.Answers.TxtRecords().FirstOrDefault();
        if (record is null)
            return null;

        var answer = record.Text.First();
        if (!answer.StartsWith("did="))
            return null;

        return answer.Replace("did=", "");
    }

    /// <summary>
    /// Tries to resolve a handle via the ".well-known/" url scheme.
    /// </summary>
    /// <param name="handle">The handle.</param>
    /// <returns>The DID, or nothing.</returns>
    private async Task<string?> TryResolveDidThroughWellKnown(string handle)
    {
        const string endpoint = "/.well-known/atproto-did";
        
        using var client = httpClientFactory.CreateClient("domain-did-resolver");
        client.BaseAddress = new Uri("https://" + handle);

        var resp = await client.GetAsync(endpoint);
        if (!resp.IsSuccessStatusCode)
            return null;

        return await resp.Content.ReadAsStringAsync();
    }
}