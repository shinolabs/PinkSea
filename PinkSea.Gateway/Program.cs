using System.Text;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using PinkSea.Gateway.Models;
using PinkSea.Gateway.Models.Rss;
using PinkSea.Gateway.Services;
using PinkSea.Gateway.Services.Rss;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<GatewaySettings>(
    builder.Configuration.GetSection("GatewaySettings"));
builder.Services.AddScoped<MetaGeneratorService>();
builder.Services.AddScoped<SyndicationBuilderService>();
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

app.MapGet(
    "/{did}/rss", 
    async ([FromRoute] string did, [FromServices] SyndicationBuilderService syndicationBuilderService) =>
    {
        using var ms = new MemoryStream();
        await using var sw = new StreamWriter(ms);
        
        var rss = await syndicationBuilderService.BuildSyndicationFeedFor(did);
        var xmlNamespaces = new XmlSerializerNamespaces();
        xmlNamespaces.Add("atom", "http://www.w3.org/2005/Atom");
        var serializer = new XmlSerializer(typeof(RssRoot));
        serializer.Serialize(sw, rss, xmlNamespaces);
        return Results.Text(Encoding.UTF8.GetString(ms.ToArray()), contentType: "application/rss+xml");
    });

app.MapFallback(async ([FromServices] MetaGeneratorService metaGenerator) =>
{
    var file = await File.ReadAllTextAsync($"./wwwroot/index.html");
    file = file.Replace("<!-- META -->", metaGenerator.GetRegularMeta());
    
    return Results.Text(file, contentType: "text/html");

});

app.Run();