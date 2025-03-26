using PinkSea.AtProto.Authorization;
using PinkSea.AtProto.Models.Authorization;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Providers.Storage;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Server.Xrpc;
using PinkSea.AtProto.Shared.Xrpc;
using PinkSea.Extensions;
using PinkSea.Lexicons.Queries;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.shinolabs.pinksea.getIdentity" xrpc query. Returns the identity of the authorized user.
/// </summary>
[Xrpc("com.shinolabs.pinksea.getIdentity")]
public class GetIdentityQueryHandler(IHttpContextAccessor httpContextAccessor,
    IOAuthStateStorageProvider oAuthStateStorageProvider,
    IDidResolver didResolver,
    IAtProtoAuthorizationService atProtoAuthorizationService, 
    IAtProtoOAuthClient oauthClient,
    ILogger<GetIdentityQueryHandler> logger)
    : IXrpcQuery<GetIdentityQueryRequest, GetIdentityQueryResponse>
{
    /// <inheritdoc />
    public async Task<XrpcErrorOr<GetIdentityQueryResponse>> Handle(GetIdentityQueryRequest request)
    {
        var state = httpContextAccessor.HttpContext?.GetStateToken();
        if (state is null)
            return XrpcErrorOr<GetIdentityQueryResponse>.Fail("NoAuthToken", "Missing authorization token.");

        var oauthState = await oAuthStateStorageProvider.GetForStateId(state);
        if (oauthState is null)
            return XrpcErrorOr<GetIdentityQueryResponse>.Fail("InvalidToken", "Invalid token.");

        if (oauthState.HasExpired())
        {
            // Try to renew the session.
            logger.LogInformation("Trying to renew session for {Did} while fetching identity.",
                oauthState.Did);
            
            if (oauthState.AuthorizationType == AuthorizationType.PdsSession)
            {
                if (!await atProtoAuthorizationService.RefreshSession(state))
                    return XrpcErrorOr<GetIdentityQueryResponse>.Fail("SessionExpired", "Your session has expired, log in again.");
            }
            else
            {
                if (!await oauthClient.Refresh(state))
                    return XrpcErrorOr<GetIdentityQueryResponse>.Fail("SessionExpired", "Your session has expired, log in again.");
            }
        }

        var didDocument = await didResolver.GetDocumentForDid(oauthState.Did);
        return XrpcErrorOr<GetIdentityQueryResponse>.Ok(new GetIdentityQueryResponse
        {
            Did = oauthState.Did,
            Handle = didDocument!.GetHandle()!
        });
    }
}