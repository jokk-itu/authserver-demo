using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace IdentityProvider.Options;

public class ConfigureAuthenticationOptions : IConfigureOptions<AuthenticationOptions>
{
    public void Configure(AuthenticationOptions options)
    {
    }
}