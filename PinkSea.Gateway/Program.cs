using Microsoft.AspNetCore.Mvc;
using PinkSea.Gateway.Endpoints;
using PinkSea.Gateway.Models;
using PinkSea.Gateway.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<GatewaySettings>(
    builder.Configuration.GetSection("GatewaySettings"));
builder.Services.AddScoped<MetaGeneratorService>();
builder.Services.AddScoped<PinkSeaQuery>();
builder.Services.AddScoped<ActivityPubRenderer>();
builder.Services.AddScoped<OEmbedRenderer>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient(
    "pinksea-xrpc",
    client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["GatewaySettings:AppViewEndpoint"]!);
    });

var app = builder.Build();
app.UseStaticFiles();

app.MapGeneralEndpoints();
app.MapActivityPubEndpoints();
app.MapOEmbedEndpoints();

app.MapFallback(async ([FromServices] MetaGeneratorService metaGenerator) =>
{
    var file = await File.ReadAllTextAsync($"./wwwroot/index.html");
    file = file.Replace("<!-- META -->", metaGenerator.GetRegularMeta());
    
    return Results.Text(file, contentType: "text/html");

});

app.Run();