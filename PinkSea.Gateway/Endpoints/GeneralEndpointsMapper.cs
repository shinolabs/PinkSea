using Microsoft.AspNetCore.Mvc;
using PinkSea.Gateway.Services;

namespace PinkSea.Gateway.Endpoints;

/// <summary>
/// Mapper for the general endpoints.
/// </summary>
public static class GeneralEndpointsMapper
{
    /// <summary>
    /// Maps the general endpoints for PinkSea.
    /// </summary>
    public static void MapGeneralEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet(
            "/{did}/oekaki/{rkey}", 
            async ([FromRoute] string did, [FromRoute] string rkey, [FromServices] MetaGeneratorService metaGenerator) =>
            {
                var file = await File.ReadAllTextAsync($"./wwwroot/index.html");
                file = file.Replace("<!-- META -->", await metaGenerator.GetOekakiMetaFor(did, rkey));
    
                return Results.Text(file, contentType: "text/html");
            });

        // The regex ensures we don't accidentally match the favicon...
        routeBuilder.MapGet(
            "/{did:regex(^(?!favicon\\.ico$).*$)}",
            async ([FromRoute] string did, [FromServices] MetaGeneratorService metaGenerator) =>
            {
                var file = await File.ReadAllTextAsync($"./wwwroot/index.html");
                file = file.Replace("<!-- META -->", await metaGenerator.GetProfileMetaFor(did));

                return Results.Text(file, contentType: "text/html");
            });
        
        routeBuilder.MapGet(
            "/xrpc/_health",
            () => new
            {
                version = "PinkSea.Gateway"
            });
    }
}