using IdentityProvider.Extensions;
using IdentityProvider.Options;
using Microsoft.IdentityModel.Logging;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(options => options.AddServerHeader = false);

builder.Host.UseSerilog((hostBuilderContext, serviceProvider, loggerConfiguration) =>
{
    loggerConfiguration
        .Enrich.FromLogContext()
        .MinimumLevel.Warning()
        .Enrich.WithProperty("Application", "AuthorizationServer")
        .MinimumLevel.Override("IdentityProvider", LogEventLevel.Information)
        .MinimumLevel.Override("AuthServer", LogEventLevel.Information)
        .WriteTo.Console();
});

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenIdAuthentication();
builder.Services.AddCorsPolicy();
builder.Services.AddAntiforgery();
builder.Services.ConfigureOptions<ConfigureAntiforgeryOptions>();
builder.Services.ConfigureOptions<ConfigureForwardedHeadersOptions>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

if (app.Environment.IsDevelopment())
{
    IdentityModelEventSource.ShowPII = true;
}

app.UseForwardedHeaders();
app.UseHsts();
app.UseSerilogRequestLogging();
app.UseStaticFiles();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();