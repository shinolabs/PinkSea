using Microsoft.AspNetCore.Mvc;
using PinkSea.Gateway.Services;

namespace PinkSea.Gateway.Endpoints;

/// <summary>
/// Maps the ActivityPub-specific endpoints for PinkSea.
/// </summary>
public static class ActivityPubEndpointsMapper
{
    /// <summary>
    /// Maps the ActivityPub-specific endpoints for PinkSea.
    /// </summary>
    public static void MapActivityPubEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/ap/note.json",
            async ([FromQuery] string did, [FromQuery] string rkey, [FromServices] ActivityPubRenderer activityPubRenderer) =>
            {
                var response = await activityPubRenderer.RenderNoteForOekaki(did, rkey);
                if (response is null)
                {
                    return Results.NotFound();
                }

                return Results.Json(response, contentType: "application/activity+json");
            });

        routeBuilder.MapGet("/ap/actor.json",
            async ([FromQuery] string did, [FromServices] ActivityPubRenderer activityPubRenderer) =>
            {
                var response = await activityPubRenderer.RenderActorForProfile(did);
                if (response is null)
                {
                    return Results.NotFound();
                }

                return Results.Json(response, contentType: "application/activity+json");
            });
    }
}