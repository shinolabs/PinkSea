using DnsClient;

namespace PinkSea.AtProto.Resolvers.Domain;

public class DomainDidResolver(LookupClient lookupClient) : IDomainDidResolver
{
    public Task<string?> GetDidForDomainHandle(string handle)
    {
        return TryResolveDidThroughDnsTxt(handle);
    }

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
}