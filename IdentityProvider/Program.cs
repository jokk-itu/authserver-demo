using Application;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Logging;
using Serilog;
using WebApp.Constants;
using WebApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(options => options.AddServerHeader = false);

builder.Host.UseSerilog((hostBuilderContext, serviceProvider, loggerConfiguration) =>
{
  loggerConfiguration
    .Enrich.FromLogContext()
    .MinimumLevel.Warning()
    .Enrich.WithProperty("Application", "AuthorizationServer")
    .MinimumLevel.Override("WebApp", Serilog.Events.LogEventLevel.Information)
    .MinimumLevel.Override("Infrastructure", Serilog.Events.LogEventLevel.Information)
    .WriteTo.Console();
});

builder.WebHost.ConfigureServices(services =>
{
  services.AddControllersWithViews();
  services.AddEndpointsApiExplorer();
  services.AddOpenIdAuthentication();
  services.AddOpenIdAuthorization();
  services.AddCorsPolicy();

  services.AddAntiforgery(antiForgeryOptions =>
  {
    antiForgeryOptions.FormFieldName = AntiForgeryConstants.AntiForgeryField;
    antiForgeryOptions.Cookie = new CookieBuilder
    {
      Name = AntiForgeryConstants.AntiForgeryCookie, 
      HttpOnly = true,
      IsEssential = true,
      SameSite = SameSiteMode.Strict,
      SecurePolicy = CookieSecurePolicy.Always
    };
  });

  builder.Services.Configure<ForwardedHeadersOptions>(options =>
  {
    options.ForwardedHeaders = ForwardedHeaders.All;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
  });
});

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