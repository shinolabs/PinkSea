using PinkSea.Extensions;

namespace PinkSea.Middleware;

/// <summary>
/// A middleware for setting the state token when we receive a request.
/// </summary>
public class StateTokenMiddleware : IMiddleware
{
    /// <inheritdoc />
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var token = FetchTokenFromHeader(context);
        if (token is not null)
            context.SetStateToken(token);
        
        return next(context);
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