using PinkSea.AtProto;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Providers.Storage;
using PinkSea.AtProto.Server.Xrpc;
using PinkSea.AtProto.Streaming;
using PinkSea.AtProto.Streaming.JetStream;
using PinkSea.Database;
using PinkSea.Middleware;
using PinkSea.Models;
using PinkSea.Services;
using PinkSea.Services.Hosting;
using PinkSea.Services.Integration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppViewConfig>(
    builder.Configuration.GetSection("AppViewConfig"));

builder.Services.Configure<PostgresConfig>(
    builder.Configuration.GetSection("PostgresConfig"));

builder.Services.Configure<FrontendConfig>(
    builder.Configuration.GetSection("FrontendConfig"));

// Add services to the container.
builder.Services.AddTransient<StateTokenMiddleware>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddControllers();
builder.Services.AddScoped<IOAuthStateStorageProvider, DatabaseOAuthStateStorageProvider>();
builder.Services.AddTransient<IOAuthClientDataProvider, OAuthClientDataProvider>();
builder.Services.AddSingleton<SigningKeyService>();
builder.Services.AddSingleton<ConfigurationService>();
builder.Services.AddScoped<OekakiService>();
builder.Services.AddScoped<TagsService>();
builder.Services.AddScoped<BlueskyIntegrationService>();
builder.Services.AddScoped<FirstTimeRunAssistantService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddTransient<FeedBuilder>();
builder.Services.AddDbContext<PinkSeaDbContext>();

builder.Services.AddAtProtoClientServices(o =>
{
    o.PlcDirectory = new Uri(builder.Configuration["AppViewConfig:PlcDirectory"]!);
});

builder.Services.AddJetStream(o =>
{
    o.Endpoint = builder.Configuration["AppViewConfig:JetStreamEndpoint"];
    o.CursorFilePath = builder.Configuration["AppViewConfig:CursorFilePath"];
    o.WantedCollections = ["com.shinolabs.pinksea.oekaki", "com.shinolabs.pinksea.profile"];
});

builder.Services.AddScoped<IJetStreamEventHandler, OekakiJetStreamEventHandler>();
builder.Services.AddXrpcHandlers();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "PinkSeaPolicy",
        policy  =>
        {
            policy.WithOrigins("*")
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddHostedService<FirstTimeRunAssistantServiceRunner>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapControllers();

app.UseRouting();
app.UseXrpcHandler();

app.UseAuthorization();

app.UseCors("PinkSeaPolicy");
app.UseMiddleware<StateTokenMiddleware>();

app.MapFallbackToFile("index.html");

app.Run();