using AuthServer.Core;
using Microsoft.Extensions.Options;

namespace IdentityProvider.Options;

public class ConfigureUserInteractionOptions : IConfigureOptions<UserInteraction>
{
    private readonly IConfiguration _configuration;

    public ConfigureUserInteractionOptions(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(UserInteraction options)
    {
        var identity = _configuration.GetSection("Identity");
        options.AccountSelectionUri = identity.GetValue<string>("AccountSelectionUri")!;
        options.ConsentUri = identity.GetValue<string>("ConsentUri")!;
        options.LoginUri = identity.GetValue<string>("LoginUri")!;
        options.EndSessionUri = identity.GetValue<string>("EndSessionUri")!;
    }
}