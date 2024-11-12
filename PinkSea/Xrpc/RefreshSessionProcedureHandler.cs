using PinkSea.AtProto.OAuth;
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
    IAtProtoOAuthClient oauthClient,
    IHttpContextAccessor httpContextAccessor) : IXrpcProcedure<Empty, Empty>
{
    /// <inheritdoc />
    public async Task<Empty?> Handle(Empty request)
    {
        var state = httpContextAccessor.HttpContext?.GetStateToken();
        if (state is null)
            return null!;

        if (!await oauthClient.Refresh(state))
            return null!;
        
        return new Empty();
    }
}