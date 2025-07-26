using PinkSea.AtProto.Authorization;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Server.Xrpc;
using PinkSea.AtProto.Shared.Xrpc;
using PinkSea.Lexicons.Procedures;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.shinolabs.pinksea.beginLoginFlow" XRPC procedure. Begins the login flow.
/// </summary>
[Xrpc("com.shinolabs.pinksea.beginLoginFlow")]
public class BeginLoginFlowProcedure(
    IAtProtoOAuthClient oAuthClient,
    IAtProtoAuthorizationService atProtoAuthorizationService,
    ILogger<BeginLoginFlowProcedure> logger)
    : IXrpcProcedure<BeginLoginFlowProcedureRequest, BeginLoginFlowProcedureResponse>
{
    /// <inheritdoc />
    public async Task<XrpcErrorOr<BeginLoginFlowProcedureResponse>> Handle(BeginLoginFlowProcedureRequest request)
    {
        try
        {
            var normalizedHandle = request.Handle
                .Trim()
                .TrimStart('@')
                .ToLower();

            // If we don't have a domain, that is, we don't have a '.' in the name
            // let's just assume '.bsky.social' at the end.
            // Usually people with custom domains don't do that.
            if (!normalizedHandle.Contains('.') && !normalizedHandle.StartsWith("did:"))
                normalizedHandle += ".bsky.social";

            if (request.Password is { Length: > 0 })
            {
                // Log in via a password.
                var authorized =
                    await atProtoAuthorizationService.LoginWithPassword(normalizedHandle, request.Password);

                if (!authorized.IsError)
                {
                    return XrpcErrorOr<BeginLoginFlowProcedureResponse>.Ok(new BeginLoginFlowProcedureResponse
                    {
                        Redirect = $"{request.RedirectUrl}?code={authorized.Value}"
                    });
                }

                return XrpcErrorOr<BeginLoginFlowProcedureResponse>.Fail(
                    "PasswordFailure",
                    authorized.Error);
            }

            var authorizationServer = await oAuthClient.BeginOAuthFlow(
                normalizedHandle,
                request.RedirectUrl);

            if (authorizationServer.IsError)
            {
                return XrpcErrorOr<BeginLoginFlowProcedureResponse>.Fail(
                    "OAuthFailure",
                    authorizationServer.Error);
            }

            return XrpcErrorOr<BeginLoginFlowProcedureResponse>.Ok(new BeginLoginFlowProcedureResponse
            {
                Redirect = authorizationServer.Value
            });
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to log-in for {request.Handle}", e);

            return XrpcErrorOr<BeginLoginFlowProcedureResponse>.Fail(
                "UnknownError",
                $"Server encountered an exception while performing your login: {e.Message}. Are you sure your handle is of the format @name.bsky.social?");
        }
    }
}