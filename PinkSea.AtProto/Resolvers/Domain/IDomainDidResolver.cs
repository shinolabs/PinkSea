namespace PinkSea.AtProto.Resolvers.Domain;

public interface IDomainDidResolver
{
    public Task<string?> GetDidForDomainHandle(string handle);
}