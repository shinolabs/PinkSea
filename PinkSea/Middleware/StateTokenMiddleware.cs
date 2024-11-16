using PinkSea.AtProto.Authorization;
using PinkSea.AtProto.Models.Authorization;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Providers.Storage;
using PinkSea.Extensions;

namespace PinkSea.Middleware;

/// <summary>
/// A middleware for setting the state token when we receive a request.
/// </summary>
public class StateTokenMiddleware(
    IOAuthStateStorageProvider oAuthStateStorageProvider,
    IAtProtoOAuthClient oAuthClient,
    IAtProtoAuthorizationService atProtoAuthorizationService,
    ILogger<StateTokenMiddleware> logger) : IMiddleware
{
    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var token = FetchTokenFromHeader(context);
        if (token is not null)
        {
            await TryRefreshTokenIfRequired(token);
            context.SetStateToken(token);
        }
        
        await next(context);
    }

    /// <summary>
    /// Tries to refresh the token before usage, if it is required.
    /// </summary>
    /// <param name="token">The token.</param>
    private async Task TryRefreshTokenIfRequired(string token)
    {
        var state = await oAuthStateStorageProvider.GetForStateId(token);
        if (state is null)
            return;

        if (DateTimeOffset.UtcNow < state.ExpiresAt)
            return;
        
        logger.LogInformation($"Refreshing OAuth token for state {token}");

        if (state.AuthorizationType == AuthorizationType.PdsSession)
        {
            await atProtoAuthorizationService.RefreshSession(token);
        }
        else
        {
            await oAuthClient.Refresh(token);
        }
    }

    /// <summary>
    /// Fetches the token from the header.
    /// </summary>
    /// <param name="ctx">The context.</param>
    /// <returns>The token, if it exists.</returns>
    private static string? FetchTokenFromHeader(HttpContext ctx)
    {
        var header = ctx.Request
            .Headers
            .Authorization
            .ToString();

        if (string.IsNullOrEmpty(header))
            return null;

        var code = header.Split(' ');

        return code.First() != "Bearer"
            ? null
            : code.Last();
    }
}