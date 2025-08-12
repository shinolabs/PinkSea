using Microsoft.AspNetCore.Mvc;
using PinkSea.Gateway.Models;
using PinkSea.Gateway.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpLogging(o => { });
builder.Services.Configure<GatewaySettings>(
    builder.Configuration.GetSection("GatewaySettings"));
builder.Services.AddScoped<MetaGeneratorService>();
builder.Services.AddScoped<PinkSeaQuery>();
builder.Services.AddScoped<ActivityPubRenderer>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient(
    "pinksea-xrpc",
    client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["GatewaySettings:AppViewEndpoint"]!);
    });

var app = builder.Build();
app.UseHttpLogging();
app.UseStaticFiles();
app.MapGet(
    "/{did}/oekaki/{rkey}", 
    async ([FromRoute] string did, [FromRoute] string rkey, [FromServices] MetaGeneratorService metaGenerator) =>
{
    var file = await File.ReadAllTextAsync($"./wwwroot/index.html");
    file = file.Replace("<!-- META -->", await metaGenerator.GetOekakiMetaFor(did, rkey));
    
    return Results.Text(file, contentType: "text/html");
});

// The regex ensures we don't accidentally match the favicon...
app.MapGet(
    "/{did:regex(^(?!favicon\\.ico$).*$)}",
    async ([FromRoute] string did, [FromServices] MetaGeneratorService metaGenerator) =>
    {
        var file = await File.ReadAllTextAsync($"./wwwroot/index.html");
        file = file.Replace("<!-- META -->", await metaGenerator.GetProfileMetaFor(did));

        return Results.Text(file, contentType: "text/html");
    });

app.MapGet("/ap/note.json",
    async ([FromQuery] string did, [FromQuery] string rkey, [FromServices] ActivityPubRenderer activityPubRenderer) =>
    {
        var response = await activityPubRenderer.RenderNoteForOekaki(did, rkey);
        if (response is null)
        {
            return Results.NotFound();
        }

        return Results.Json(response, contentType: "application/activity+json");
    });

app.MapGet("/ap/actor.json",
    async ([FromQuery] string did, [FromServices] ActivityPubRenderer activityPubRenderer) =>
    {
        var response = await activityPubRenderer.RenderActorForProfile(did);
        if (response is null)
        {
            return Results.NotFound();
        }

        return Results.Json(response, contentType: "application/activity+json");
    });

app.MapGet(
    "/xrpc/_health",
    () => new
    {
        version = "PinkSea.Gateway"
    });

app.MapFallback(async ([FromServices] MetaGeneratorService metaGenerator) =>
{
    var file = await File.ReadAllTextAsync($"./wwwroot/index.html");
    file = file.Replace("<!-- META -->", metaGenerator.GetRegularMeta());
    
    return Results.Text(file, contentType: "text/html");

});

app.Run();