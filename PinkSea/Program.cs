using PinkSea.AtProto;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Providers.Storage;
using PinkSea.Database;
using PinkSea.Models;
using PinkSea.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppViewConfig>(
    builder.Configuration.GetSection("AppViewConfig"));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IOAuthStateStorageProvider, DatabaseOAuthStateStorageProvider>();
builder.Services.AddTransient<IOAuthClientDataProvider, OAuthClientDataProvider>();
builder.Services.AddSingleton<SigningKeyService>();
builder.Services.AddScoped<OekakiService>();
builder.Services.AddDbContext<PinkSeaDbContext>();
builder.Services.AddAtProtoClientServices();

builder.Services.AddAuthentication("PinkSea")
    .AddCookie("PinkSea", options =>
    {
        options.LoginPath = "/";
        options.LogoutPath = "/oauth/invalidate";
        options.AccessDeniedPath = "/";
    });

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

app.UseAuthentication();
app.UseAuthorization();


app.Run();