using PinkSea.AtProto.Authorization;
using PinkSea.AtProto.Models.Authorization;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Providers.Storage;
using PinkSea.AtProto.Server.Xrpc;
using PinkSea.Extensions;
using PinkSea.Lexicons;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.shinolabs.pinksea.refreshSession" procedure.
/// Manually refreshes the OAuth session using the refresh token.
/// </summary>
[Xrpc("com.shinolabs.pinksea.refreshSession")]
public class RefreshSessionProcedureHandler(
    IAtProtoAuthorizationService atProtoAuthorizationService, 
    IAtProtoOAuthClient oauthClient,
    IOAuthStateStorageProvider oauthStateStorageProvider,
    IHttpContextAccessor httpContextAccessor) : IXrpcProcedure<Empty, Empty>
{
    /// <inheritdoc />
    public async Task<Empty?> Handle(Empty request)
    {
        var stateId = httpContextAccessor.HttpContext?.GetStateToken();
        if (stateId is null)
            return null!;

        var state = await oauthStateStorageProvider.GetForStateId(stateId);
        if (state is null)
            return null!;

        if (state.AuthorizationType == AuthorizationType.PdsSession)
        {
            if (!await atProtoAuthorizationService.RefreshSession(stateId))
                return null!;
        }
        else
        {
            if (!await oauthClient.Refresh(stateId))
                return null!;
        }
        
        return new Empty();
    }
}