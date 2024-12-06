using PinkSea.Gateway;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles();
app.MapGet("/{did}/oekaki/{rid}", async (string did, string rid) =>
{
    var httpClient = new HttpClient();
    var resp = await httpClient.GetFromJsonAsync<OekakiResponse>($"https://api.pinksea.art/xrpc/com.shinolabs.pinksea.getOekaki?did={did}&rkey={rid}");

    var oembed = $@"
<meta name=""application-name"" content=""PinkSea"">
<meta name=""generator"" content=""PinkSea.Gateway"">
<meta property=""og:site_name"" content=""PinkSea"" />
<meta property=""og:title"" content=""{did}'s oekaki"" />
<meta property=""og:type"" content=""website"" />
<meta property=""og:url"" content=""https://pinksea.art/{did}/oekaki/{rid}"" />
<meta property=""og:image"" content=""{resp!.Parent.ImageLink}"" />
<meta property=""og:description"" content=""*{resp!.Parent.Alt}*"" />
<meta name=""theme-color"" content=""#ffe5ea"">
<meta name=""twitter:card"" content=""summary_large_image"">
";

    var file = File.ReadAllText($"./wwwroot/index.html")
        .Replace("<!-- META -->", oembed);

    return Results.Text(file, contentType: "text/html");
});
app.MapFallbackToFile("index.html");

app.Run();