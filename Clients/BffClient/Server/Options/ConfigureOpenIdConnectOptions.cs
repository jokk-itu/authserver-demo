using System.Collections.Specialized;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Server.Options;

public class ConfigureOpenIdConnectOptions : IConfigureOptions<OpenIdConnectOptions>
{
    private readonly ILogger<ConfigureOpenIdConnectOptions> _logger;
    private readonly IConfiguration _configuration;

    public ConfigureOpenIdConnectOptions(
        ILogger<ConfigureOpenIdConnectOptions> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public void Configure(OpenIdConnectOptions options)
    {
        var identitySection = _configuration.GetSection("Identity");

        options.Authority = identitySection.GetValue<string>("Authority");
        options.TokenValidationParameters.ValidIssuer = identitySection.GetValue<string>("Authority");
        options.TokenValidationParameters.ValidAudience = identitySection.GetValue<string>("ClientId");
        options.TokenValidationParameters.NameClaimType = "name";
        options.TokenValidationParameters.RoleClaimType = "role";
        options.GetClaimsFromUserInfoEndpoint = true;
        options.DisableTelemetry = true;
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.SaveTokens = true;

        options.ClaimActions.MapUniqueJsonKey("auth_time", "auth_time");
        options.ClaimActions.MapUniqueJsonKey("grant_id", "grant_id");
        options.ClaimActions.MapUniqueJsonKey("address", "address");
        options.ClaimActions.MapUniqueJsonKey("given_name", "given_name");
        options.ClaimActions.MapUniqueJsonKey("family_name", "family_name");
        options.ClaimActions.MapUniqueJsonKey("birthdate", "birthdate");
        options.ClaimActions.MapUniqueJsonKey("name", "name");
        options.ClaimActions.MapUniqueJsonKey("email", "email");
        options.ClaimActions.MapUniqueJsonKey("phone", "phone");
        options.ClaimActions.MapUniqueJsonKey("locale", "locale");

        options.ClientId = identitySection.GetValue<string>("ClientId");
        options.ClientSecret = identitySection.GetValue<string>("ClientSecret");

        string[] resources = [_configuration.GetSection("WeatherService").GetValue<string>("Url"), identitySection.GetValue<string>("Authority")];
        //TODO set options.Resource (or not)

        string[] scopes = ["openid", "profile", "address", "email", "phone", "weather:read", "authserver:userinfo"];
        foreach (var scope in scopes)
        {
            options.Scope.Add(scope);
        }

        options.ResponseMode = OpenIdConnectResponseMode.FormPost;
        options.MaxAge = TimeSpan.FromSeconds(3600);

        options.NonceCookie.Name = "BffClient-Nonce";
        options.NonceCookie.HttpOnly = true;
        options.NonceCookie.IsEssential = true;
        options.NonceCookie.SameSite = SameSiteMode.Strict;
        options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;

        options.CorrelationCookie.Name = "BffClient-Correlation";
        options.CorrelationCookie.HttpOnly = true;
        options.CorrelationCookie.IsEssential = true;
        options.CorrelationCookie.SameSite = SameSiteMode.Strict;
        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

        options.Events = new OpenIdConnectEvents
        {
            OnRedirectToIdentityProviderForSignOut = context =>
            {
                context.ProtocolMessage.Parameters.Add("client_id", context.Options.ClientId);
                return Task.CompletedTask;
            },
            OnRedirectToIdentityProvider = context =>
            {
                var hasPrompt = context.Properties.Items.TryGetValue("prompt", out var prompt);
                if (hasPrompt)
                {
                    context.ProtocolMessage.Prompt = prompt;
                }

                var hasLoginHint = context.Properties.Items.TryGetValue("login_hint", out var loginHint);
                if (hasLoginHint)
                {
                    context.ProtocolMessage.LoginHint = loginHint;
                }

                _logger.LogInformation("Redirecting to IdP for Authorize {@AuthorizeRequest}", new
                {
                    context.ProtocolMessage.Nonce,
                    context.ProtocolMessage.RedirectUri
                });
                return Task.CompletedTask;
            },
            OnAuthorizationCodeReceived = context =>
            {
                var nonces = context.Request.Cookies
                    .Where(x => x.Key.StartsWith(options.NonceCookie.Name))
                    .Select(x => options.StringDataFormat.Unprotect(x.Key[options.NonceCookie.Name.Length..]))
                    .ToList();

                var request = context.TokenEndpointRequest!;
                _logger.LogInformation("Requesting Token {@Response} {@Internal}", new
                {
                    request.GrantType,
                    request.ClientId,
                    request.ClientSecret,
                    request.Resource,
                    request.RedirectUri
                }, new
                {
                    Nonce = nonces
                });

                var nameValueCollection = new NameValueCollection();
                foreach (var resource in resources)
                {
                    nameValueCollection.Add(OpenIdConnectParameterNames.Resource, resource);
                }

                context.TokenEndpointRequest!.SetParameters(nameValueCollection);

                return Task.CompletedTask;
            },
            OnTokenResponseReceived = context =>
            {
                var nonces = context.Request.Cookies
                    .Where(x => x.Key.StartsWith(options.NonceCookie.Name))
                    .Select(x => options.StringDataFormat.Unprotect(x.Key[options.NonceCookie.Name.Length..]))
                    .ToList();

                _logger.LogInformation("Received Token {@Response} {@Internal}", new
                {
                    context.TokenEndpointResponse.ExpiresIn
                }, new
                {
                    Nonce = nonces
                });
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                _logger.LogInformation("Token {TokenType} Validated {@Internal}",
                    context.SecurityToken.Header.Typ,
                    new
                    {
                        context.Nonce,
                        TokenNonce = context.SecurityToken.Claims.SingleOrDefault(x => x.Type == "nonce")?.Value
                    });

                return Task.CompletedTask;
            }
        };
    }
}