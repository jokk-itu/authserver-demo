using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Net.Http.Headers;
using App.Options;
using Yarp.ReverseProxy.Transforms;
using Serilog;
using Serilog.Events;
using Server.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostBuilderContext, serviceProvider, loggerConfiguration) =>
{
    loggerConfiguration
        .Enrich.FromLogContext()
        .MinimumLevel.Warning()
        .Enrich.WithProperty("Application", "Wasm")
        .MinimumLevel.Override("Serilog.AspNetCore", LogEventLevel.Information)
        .MinimumLevel.Override("AuthServer", LogEventLevel.Information)
        .WriteTo.Console();
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddOptions();
builder.Services.ConfigureOptions<ConfigureForwardedHeadersOptions>();
builder.Services.ConfigureOptions<ConfigureOpenIdConnectOptions>();
builder.Services.ConfigureOptions<ConfigureCookieAuthenticationOptions>();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOpenIdConnect();

builder.Services.AddAuthorization();
builder.Services.AddHttpClient("IdentityProvider",
    httpClient => { httpClient.BaseAddress = new Uri(builder.Configuration.GetSection("Identity")["Authority"]); });

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transformBuilder => transformBuilder.AddRequestTransform(async requestContext =>
    {
        var token = await requestContext.HttpContext.GetTokenAsync("access_token");
        requestContext.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();
app.UseForwardedHeaders();
app.UseHsts();
app.UseSerilogRequestLogging();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllers();
app.MapReverseProxy();
app.MapFallbackToFile("index.html");

app.Run();