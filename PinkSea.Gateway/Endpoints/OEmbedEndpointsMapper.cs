using Microsoft.AspNetCore.Mvc;
using PinkSea.Gateway.Services;

namespace PinkSea.Gateway.Endpoints;

/// <summary>
/// Maps the OEmbed-specific endpoints for PinkSea.
/// </summary>
public static class OEmbedEndpointsMapper
{
    /// <summary>
    /// Maps the OEmbed-specific endpoints for PinkSea.
    /// </summary>
    public static void MapOEmbedEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/oembed.json",
            async ([FromQuery] string url, [FromServices] OEmbedRenderer oEmbedRenderer) =>
            {
                var uri = new Uri(url);
                var split = uri.AbsolutePath.Split("/");
                var response = await oEmbedRenderer.RenderOEmbedForOekaki(split[1], split[3]);
                if (response is null)
                {
                    return Results.NotFound();
                }

                return Results.Json(response, contentType: "application/json+oembed");
            });
    }
}