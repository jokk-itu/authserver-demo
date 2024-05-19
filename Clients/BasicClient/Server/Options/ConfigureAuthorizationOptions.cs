using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Server.Options;

public class ConfigureAuthorizationOptions : IConfigureOptions<AuthorizationOptions>
{
    public void Configure(AuthorizationOptions options)
    {
        options.AddPolicy("CookieAuthenticationPolicy", policyBuilder =>
        {
            policyBuilder.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
            policyBuilder.RequireAuthenticatedUser();
        });
    }
}