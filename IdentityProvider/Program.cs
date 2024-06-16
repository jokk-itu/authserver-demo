using AuthServer.Cache;
using AuthServer.Core.Abstractions;
using AuthServer.Extensions;
using AuthServer.Introspection;
using IdentityProvider;
using IdentityProvider.Cache;
using IdentityProvider.Options;
using IdentityProvider.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(options => options.AddServerHeader = false);

builder.Host.UseSerilog((_, _, loggerConfiguration) =>
{
    loggerConfiguration
        .Enrich.FromLogContext()
        .MinimumLevel.Information()
        .Enrich.WithProperty("Application", "AuthServer")
        .MinimumLevel.Override("IdentityProvider", LogEventLevel.Information)
        .MinimumLevel.Override("AuthServer", LogEventLevel.Information)
        .WriteTo.Console();
});

builder.Services.AddRazorPages();
builder.Services.AddCors();
builder.Services.AddAntiforgery();
builder.Services.AddAuthentication();
builder.Services.ConfigureOptions<ConfigureAntiforgeryOptions>();
builder.Services.ConfigureOptions<ConfigureForwardedHeadersOptions>();
builder.Services.ConfigureOptions<ConfigureDiscoveryDocumentOptions>();
builder.Services.ConfigureOptions<ConfigureJwksDocumentOptions>();
builder.Services.ConfigureOptions<ConfigureCorsOptions>();
builder.Services.ConfigureOptions<ConfigureAuthenticationOptions>();

builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(x =>
    {
        // TODO post configuration for Cookie options
        // TODO hook into ApplicationCookie with login path
        // TODO what about consent page
        // consent and select_account pages?
        // Should that be separate cookies or properties in Identity cookie? Or is it needed at all to track?
    })
    .AddEntityFrameworkStores<AuthenticationDbContext>()
    .AddDefaultTokenProviders();

builder.Services
    .AddDbContext<AuthenticationDbContext>(dbContextConfigurator =>
    {
        dbContextConfigurator.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
    });

builder.Services
    .AddAuthServer(dbContextConfigurator =>
    {
        dbContextConfigurator.UseSqlServer(
	        builder.Configuration.GetConnectionString("Default"),
	        x => x.MigrationsAssembly("IdentityProvider"));
    });

// TODO Add Garnet for DataProtection
// TODO Add Garnet for DistributedCache for AuthServer
// TODO Add Garnet for SessionStore for AuthenticationCookies

builder.Services.AddSingleton<IDistributedCache, InMemoryCache>();
builder.Services.AddScoped<IUserClaimService, UserClaimService>();
builder.Services.AddScoped<IUsernameResolver, UsernameResolver>();

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
app.MapRazorPages();
app.MapAuthorizeEndpoint();
app.MapPostRegisterEndpoint();
app.MapUserinfoEndpoint();
app.MapTokenEndpoint();
app.MapRevocationEndpoint();
app.MapIntrospectionEndpoint();
app.MapDiscoveryDocumentEndpoint();
app.MapJwksDocumentEndpoint();
app.Run();