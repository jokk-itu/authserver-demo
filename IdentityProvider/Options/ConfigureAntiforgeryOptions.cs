using IdentityProvider.Constants;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Options;

namespace IdentityProvider.Options;

public class ConfigureAntiforgeryOptions : IConfigureOptions<AntiforgeryOptions>
{
    public void Configure(AntiforgeryOptions options)
    {
        options.FormFieldName = AntiForgeryConstants.AntiForgeryField;
        options.Cookie = new CookieBuilder
        {
            Name = AntiForgeryConstants.AntiForgeryCookie,
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Strict,
            SecurePolicy = CookieSecurePolicy.Always
        };
    }
}