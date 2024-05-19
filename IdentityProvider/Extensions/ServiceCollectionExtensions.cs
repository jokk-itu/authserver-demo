using IdentityProvider.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace IdentityProvider.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddOpenIdAuthentication(this IServiceCollection services)
  {
    services.ConfigureOptions<ConfigureCookieAuthenticationOptions>();
    services
      .AddAuthentication(configureOptions =>
      {
        configureOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        configureOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer()
      .AddCookie();
    return services;
  }

  public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
  {
    services.AddCors(corsOptions =>
    {
      corsOptions.AddDefaultPolicy(corsPolicyBuilder =>
      {
        corsPolicyBuilder
          .AllowAnyOrigin()
          .AllowAnyHeader();
      });
    });
    return services;
  }
}