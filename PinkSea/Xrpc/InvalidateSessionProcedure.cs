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
/// Handler for the "com.shinolabs.pinksea.invalidateSession" XRPC procedure. Invalidates a given session.
/// </summary>
[Xrpc("com.shinolabs.pinksea.invalidateSession")]
public class InvalidateSessionProcedure(
    IAtProtoAuthorizationService atProtoAuthorizationService,
    IAtProtoOAuthClient oAuthClient,
    IOAuthStateStorageProvider oAuthStateStorageProvider,
    IHttpContextAccessor contextAccessor,
    ILogger<InvalidateSessionProcedure> logger) : IXrpcProcedure<Empty, Empty>
{
    /// <inheritdoc />
    public async Task<XrpcErrorOr<Empty>> Handle(Empty request)
    {
        var stateId = contextAccessor.HttpContext?.GetStateToken();
        if (stateId is null)
            return XrpcErrorOr<Empty>.Ok(new Empty());
        
        var state = await oAuthStateStorageProvider.GetForStateId(stateId);
        if (state is null)
            return XrpcErrorOr<Empty>.Ok(new Empty());
        
        logger.LogInformation("Invalidating session for ID {SessionId}",
            state);

        if (state.AuthorizationType == AuthorizationType.PdsSession)
            await atProtoAuthorizationService.InvalidateSession(stateId);
        else
            await oAuthClient.InvalidateSession(stateId);
        
        return XrpcErrorOr<Empty>.Ok(new Empty());
    }
}