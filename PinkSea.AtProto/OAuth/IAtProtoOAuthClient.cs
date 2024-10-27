using PinkSea.AtProto.Models.OAuth;

namespace PinkSea.AtProto.OAuth;

public interface IAtProtoOAuthClient
{
    Task<string?> GetOAuthRequestUriForHandle(
        string handle,
        OAuthClientData clientData);
    
    Task<ProtectedResource?> GetOAuthProtectedResourceForPds(string pds);
    
    Task<AuthorizationServer?> GetOAuthAuthorizationServerDataForAuthorizationServer(string authServer);
}