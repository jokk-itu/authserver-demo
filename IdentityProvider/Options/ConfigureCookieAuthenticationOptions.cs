using IdentityProvider.Constants;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using CookieBuilder = Microsoft.AspNetCore.Http.CookieBuilder;

namespace IdentityProvider.Options;

public class ConfigureCookieAuthenticationOptions : IConfigureOptions<CookieAuthenticationOptions>
{
  public void Configure(CookieAuthenticationOptions options)
  {
    options.Cookie = new CookieBuilder
    {
      Name = CookieConstants.IdentityCookie,
      HttpOnly = true,
      SameSite = SameSiteMode.Strict,
      SecurePolicy = CookieSecurePolicy.Always,
      MaxAge = TimeSpan.FromDays(2),
      IsEssential = true
    };
  }
}