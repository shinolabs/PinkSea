using PinkSea.AtProto.Authorization;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Server.Xrpc;
using PinkSea.Lexicons.Procedures;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.shinolabs.pinksea.beginLoginFlow" XRPC procedure. Begins the login flow.
/// </summary>
[Xrpc("com.shinolabs.pinksea.beginLoginFlow")]
public class BeginLoginFlowProcedure(
    IAtProtoOAuthClient oAuthClient,
    IAtProtoAuthorizationService atProtoAuthorizationService)
    : IXrpcProcedure<BeginLoginFlowProcedureRequest, BeginLoginFlowProcedureResponse>
{
    /// <inheritdoc />
    public async Task<BeginLoginFlowProcedureResponse?> Handle(BeginLoginFlowProcedureRequest request)
    {
        var normalizedHandle = request.Handle.TrimStart('@')
            .ToLower();

        // If we don't have a domain, that is, we don't have a '.' in the name
        // let's just assume '.bsky.social' at the end.
        // Usually people with custom domains don't do that.
        if (!normalizedHandle.Contains('.'))
            normalizedHandle += ".bsky.social";

        if (request.Password is { Length: > 0 } )
        {
            var authorized = await atProtoAuthorizationService.LoginWithPassword(request.Handle, request.Password);
            return authorized is not null
                ? new BeginLoginFlowProcedureResponse { Redirect = $"{request.RedirectUrl}?code={authorized}" }
                : new BeginLoginFlowProcedureResponse { FailureReason = "Unable to create a password session." };
        }
        
        var authorizationServer = await oAuthClient.BeginOAuthFlow(
            normalizedHandle,
            request.RedirectUrl);
        
        return authorizationServer is null 
            ? new BeginLoginFlowProcedureResponse { FailureReason = "Unable to begin OAuth2 login flow." }
            : new BeginLoginFlowProcedureResponse { Redirect = authorizationServer };
    }
}