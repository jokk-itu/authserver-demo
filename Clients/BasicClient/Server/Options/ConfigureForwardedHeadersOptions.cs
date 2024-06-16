using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;

namespace Server.Options;

public class ConfigureForwardedHeadersOptions : IConfigureOptions<ForwardedHeadersOptions>
{
    public void Configure(ForwardedHeadersOptions options)
    {
        options.ForwardedHeaders = ForwardedHeaders.All;
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    }
}