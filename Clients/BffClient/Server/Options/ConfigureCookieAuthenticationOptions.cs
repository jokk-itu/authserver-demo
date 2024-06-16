using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Server.Options;

public class ConfigureCookieAuthenticationOptions : IConfigureOptions<CookieAuthenticationOptions>
{
    private readonly ILogger<ConfigureCookieAuthenticationOptions> _logger;
    private IHttpClientFactory _httpClientFactory;
    private readonly IOptionsSnapshot<OpenIdConnectOptions> _openIdConnectOptions;

    public ConfigureCookieAuthenticationOptions(
        ILogger<ConfigureCookieAuthenticationOptions> logger,
        IHttpClientFactory httpClientFactory,
        IOptionsSnapshot<OpenIdConnectOptions> openIdConnectOptions)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _openIdConnectOptions = openIdConnectOptions;
    }

    public void Configure(CookieAuthenticationOptions options)
    {
        options.LoginPath = "/api/user/login";
        options.LogoutPath = "/api/user/logout";
        options.ReturnUrlParameter = "/";
        options.Cookie.Name = "IdentityCookie-BffClient";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = GetRefreshTokenIfExceededExpiration
        };
    }

    private async Task GetRefreshTokenIfExceededExpiration(CookieValidatePrincipalContext context)
    {
        if (context.Principal?.Identity?.IsAuthenticated == false)
        {
            _logger.LogDebug("User is not authenticated");
            return;
        }

        var tokens = context.Properties.GetTokens().ToList();
        var expiresIn = tokens.Find(t => t.Name == "expires_at")?.Value ?? "0";
        var expiration = DateTime.Parse(expiresIn).ToUniversalTime();
        if (expiration > DateTime.UtcNow)
        {
            _logger.LogDebug("Token has not expired {expiresAt}", expiration);
            return;
        }

        var configuration =
            await _openIdConnectOptions.Value.ConfigurationManager!.GetConfigurationAsync(CancellationToken.None);

        var tokenClientOptions = new TokenClientOptions
        {
            ClientCredentialStyle = ClientCredentialStyle.PostBody,
            ClientId = _openIdConnectOptions.Value.ClientId!,
            ClientSecret = _openIdConnectOptions.Value.ClientSecret!,
            Address = configuration.TokenEndpoint
        };

        var httpClient = _httpClientFactory.CreateClient("IdentityProvider");
        var tokenClient = new TokenClient(httpClient, tokenClientOptions);
        var tokenResponse = await tokenClient.RequestRefreshTokenAsync(OpenIdConnectGrantTypes.RefreshToken);
        if (tokenResponse.IsError)
        {
            _logger.LogError(tokenResponse.Exception,
                "Error occurred during refresh token request. Error {ErrorCode}. ErrorDescription {ErrorDescription}. StatusCode {StatusCode}",
                tokenResponse.Error,
                tokenResponse.ErrorDescription,
                tokenResponse.HttpStatusCode);

            context.RejectPrincipal();
            return;
        }

        var newTokens = new List<AuthenticationToken>();
        if (!string.IsNullOrEmpty(tokenResponse.AccessToken))
        {
            newTokens.Add(new AuthenticationToken
            {
                Name = "access_token",
                Value = tokenResponse.AccessToken
            });
            newTokens.Add(new AuthenticationToken
            {
                Name = "expires_in",
                Value = tokenResponse.ExpiresIn.ToString()
            });
        }

        if (!string.IsNullOrEmpty(tokenResponse.IdentityToken))
        {
            newTokens.Add(new AuthenticationToken
            {
                Name = "id_token",
                Value = tokenResponse.IdentityToken
            });
        }

        if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
        {
            newTokens.Add(new AuthenticationToken
            {
                Name = "refresh_token",
                Value = tokenResponse.RefreshToken
            });
        }

        context.Properties.StoreTokens(newTokens);
    }
}