using PinkSea.AtProto.Models.OAuth;

namespace PinkSea.AtProto.OAuth;

public interface IAtProtoOAuthClient
{
    Task<string?> GetOAuthRequestUriForHandle(
        string handle,
        string redirectUrl);
    
    Task<ProtectedResource?> GetOAuthProtectedResourceForPds(string pds);
    
    Task<AuthorizationServer?> GetOAuthAuthorizationServerDataForAuthorizationServer(string authServer);
}