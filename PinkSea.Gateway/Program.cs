using Microsoft.AspNetCore.Mvc;
using PinkSea.Gateway.Models;
using PinkSea.Gateway.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<GatewaySettings>(
    builder.Configuration.GetSection("GatewaySettings"));
builder.Services.AddScoped<MetaGeneratorService>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient(
    "pinksea-xrpc",
    client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["GatewaySettings:AppViewEndpoint"]!);
    });

var app = builder.Build();

app.UseStaticFiles();
app.MapGet(
    "/{did}/oekaki/{rkey}", 
    async ([FromRoute] string did, [FromRoute] string rkey, [FromServices] MetaGeneratorService metaGenerator) =>
{
    var file = await File.ReadAllTextAsync($"./wwwroot/index.html");
    file = file.Replace("<!-- META -->", await metaGenerator.GetOekakiMetaFor(did, rkey));
    
    return Results.Text(file, contentType: "text/html");
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