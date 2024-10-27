namespace PinkSea.AtProto.Providers;

public interface IJwtSigningProvider
{
    string GetToken(string did, string audience);
}