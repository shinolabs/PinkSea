using PinkSea.AtProto;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Providers.Storage;
using PinkSea.Models;
using PinkSea.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppViewConfig>(
    builder.Configuration.GetSection("AppViewConfig"));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IOAuthStateStorageProvider, MemoryOAuthStateStorageProvider>();
builder.Services.AddTransient<IOAuthClientDataProvider, OAuthClientDataProvider>();
builder.Services.AddSingleton<SigningKeyService>();
builder.Services.AddAtProtoClientServices();

builder.Services.AddAuthentication("PinkSea")
    .AddCookie("PinkSea", options =>
    {
        options.LoginPath = "/";
        options.LogoutPath = "/oauth/invalidate";
        options.AccessDeniedPath = "/";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();