using App;
using App.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Serilog;
using Microsoft.IdentityModel.Logging;
using App.Services;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostBuilderContext, serviceProvider, loggingConfiguration) =>
{
    loggingConfiguration
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "WebApp")
        .MinimumLevel.Warning()
        .MinimumLevel.Override("App", LogEventLevel.Information)
        .MinimumLevel.Override("Serilog.AspNetCore", LogEventLevel.Information)
        .WriteTo.Console();
});

builder.Services.AddControllersWithViews();
builder.Services.AddOptions();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<PopulateAccessTokenDelegatingHandler>();

builder.Services.ConfigureOptions<ConfigureForwardedHeadersOptions>();
builder.Services.ConfigureOptions<ConfigureCookieAuthenticationOptions>();
builder.Services.ConfigureOptions<ConfigureOpenIdConnectOptions>();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(configureOptions =>
    {
        configureOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        configureOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOpenIdConnect();

builder.Services.AddHttpClient<WeatherService>(httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration.GetSection("WeatherService")["Url"]);
}).AddHttpMessageHandler<PopulateAccessTokenDelegatingHandler>();

builder.Services.AddHttpClient("IdentityProvider",
    httpClient => { httpClient.BaseAddress = new Uri(builder.Configuration.GetSection("Identity")["Authority"]); });

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
app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();