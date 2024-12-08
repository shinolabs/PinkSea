using Microsoft.AspNetCore.Mvc;
using PinkSea.Gateway.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<MetaGeneratorService>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient(
    "pinksea-xrpc",
    client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["GatewaySettings:PinkSeaEndpoint"]!);
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

app.MapFallback(async () =>
{
    var file = await File.ReadAllTextAsync($"./wwwroot/index.html");
    
});

app.MapFallbackToFile("index.html");

app.Run();