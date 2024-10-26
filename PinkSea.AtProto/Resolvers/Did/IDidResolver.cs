using PinkSea.AtProto.Models.Did;

namespace PinkSea.AtProto.Resolvers.Did;

public interface IDidResolver
{
    Task<DidResponse?> GetDidResponseForDid(string did);
}