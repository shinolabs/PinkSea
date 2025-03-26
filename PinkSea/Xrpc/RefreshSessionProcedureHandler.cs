using PinkSea.AtProto.Authorization;
using PinkSea.AtProto.Models.Authorization;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Providers.Storage;
using PinkSea.AtProto.Server.Xrpc;
using PinkSea.AtProto.Shared.Xrpc;
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
    public async Task<XrpcErrorOr<Empty>> Handle(Empty request)
    {
        var stateId = httpContextAccessor.HttpContext?.GetStateToken();
        if (stateId is null)
            return XrpcErrorOr<Empty>.Fail("NoAuthToken", "Missing authorization token.");

        var state = await oauthStateStorageProvider.GetForStateId(stateId);
        if (state is null)
            return XrpcErrorOr<Empty>.Fail("InvalidToken", "Invalid token.");

        if (state.AuthorizationType == AuthorizationType.PdsSession)
        {
            if (!await atProtoAuthorizationService.RefreshSession(stateId))
                return XrpcErrorOr<Empty>.Fail("SessionExpired", "Your session has expired, log in again.");
        }
        else
        {
            if (!await oauthClient.Refresh(stateId))
                return XrpcErrorOr<Empty>.Fail("SessionExpired", "Your session has expired, log in again.");
        }
        
        return XrpcErrorOr<Empty>.Ok(new Empty());
    }
}