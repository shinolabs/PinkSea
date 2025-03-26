using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using PinkSea.AtProto.Shared.Xrpc;

namespace PinkSea.AtProto.Server.Xrpc;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the XRPC server handler into the service collection
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddXrpcHandlers(this IServiceCollection serviceCollection)
    {
        var handlers = Assembly.GetEntryAssembly()!
            .GetTypes()
            .Where(t => t is { IsInterface: false, IsAbstract: false }
                        && t.IsAssignableTo(typeof(IXrpcRequestHandler)));

        foreach (var handler in handlers)
        {
            serviceCollection.AddScoped(handler);
            XrpcTypeResolver.Register(handler);            
        }

        serviceCollection.AddScoped<IXrpcHandler, DefaultXrpcHandler>();
        return serviceCollection;
    }

    /// <summary>
    /// Maps the XRPC server handler into the route builder.
    /// </summary>
    /// <param name="routeBuilder">The route builder.</param>
    /// <returns>The route builder.</returns>
    public static IEndpointRouteBuilder UseXrpcHandler(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet(
            "/xrpc/{nsid}", 
            HandleXrpc);

        routeBuilder.MapPost(
            "/xrpc/{nsid}",
            HandleXrpc);

        return routeBuilder;
    }

    private static async Task<IResult> HandleXrpc(
        HttpContext ctx,
        string nsid,
        [FromServices] IServiceProvider serviceProvider)
    {
        var xrpcHandler = serviceProvider.GetRequiredService<IXrpcHandler>();
        var result = await xrpcHandler.HandleXrpc(nsid, ctx);

        return result is XrpcError
            ? Results.BadRequest(result)
            : Results.Ok(result);
    }
}