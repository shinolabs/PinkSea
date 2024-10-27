namespace PinkSea.AtProto.Providers.OAuth;

public interface IDpopStorageProvider
{
    Task SetForDid(string did);
}