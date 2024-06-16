using AuthServer.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IdentityProvider.Options;

public class ConfigureCookieAuthenticationOptions : IConfigureNamedOptions<CookieAuthenticationOptions>
{
	private readonly IOptionsMonitor<UserInteraction> _userInteractionOptions;

	public ConfigureCookieAuthenticationOptions(
		IOptionsMonitor<UserInteraction> userInteractionOptions)
	{
		_userInteractionOptions = userInteractionOptions;
	}

	public void Configure(string? name, CookieAuthenticationOptions options)
	{
		if (name == IdentityConstants.ApplicationScheme)
		{
			// TODO add SessionStore using Garnet
			
			options.Cookie.HttpOnly = true;
			options.Cookie.SameSite = SameSiteMode.Strict;
			options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
			options.Cookie.IsEssential = true;

			options.SlidingExpiration = true;

			options.LoginPath = _userInteractionOptions.CurrentValue.LoginUri;
			options.LogoutPath = _userInteractionOptions.CurrentValue.EndSessionUri;
		}

		if (name == IdentityConstants.ExternalScheme)
		{
			// TODO add SessionStore using Garnet

			options.Cookie.HttpOnly = true;
			options.Cookie.SameSite = SameSiteMode.Strict;
			options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
			options.Cookie.IsEssential = true;

			options.SlidingExpiration = true;
		}
	}

	public void Configure(CookieAuthenticationOptions options)
	{
		Configure(null, options);
	}
}