using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using WishListApp.Identity.Services;

namespace WishListApp.Identity;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        var identityOptions = configuration.GetSection(nameof(IdentityOptions)).Get<IdentityOptions>()
            ?? throw new ArgumentNullException($"No {nameof(IdentityOptions)} were provided.");
        services.Configure<IdentityOptions>(configuration.GetSection(nameof(IdentityOptions)));

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            options.Cookie.SameSite = SameSiteMode.Lax;
        })
        .AddOpenIdConnect(options =>
        {
            options.Authority = identityOptions.Authority;
            options.ClientId = identityOptions.ClientId;
            options.ClientSecret = identityOptions.ClientSecret;
            options.RequireHttpsMetadata = identityOptions.RequireHttpsMetadata;

            foreach (var scope in identityOptions.Scopes)
            {
                options.Scope.Add(scope);
            }

            options.ResponseType = OpenIdConnectResponseType.Code;
            options.SaveTokens = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "preferred_username",
                RoleClaimType = "roles"
            };
            options.GetClaimsFromUserInfoEndpoint = true;

            options.Events.OnRedirectToIdentityProviderForSignOut = context =>
            {
                var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<IdentityOptions>>()?.Value
                    ?? throw new ArgumentNullException($"No {nameof(IdentityOptions)} were provided.");

                if (options.RequireHttpsMetadata)
                {
                    context.ProtocolMessage.PostLogoutRedirectUri = context.ProtocolMessage.PostLogoutRedirectUri.Replace("http", "https");
                }

                return Task.CompletedTask;
            };

            options.Events.OnRedirectToIdentityProvider = context =>
            {
                var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<IdentityOptions>>()?.Value
                    ?? throw new ArgumentNullException($"No {nameof(IdentityOptions)} were provided.");

                if (options.RequireHttpsMetadata)
                {
                    context.ProtocolMessage.RedirectUri = context.ProtocolMessage.RedirectUri.Replace("http", "https");
                }

                return Task.CompletedTask;
            };
        });

        services.AddAuthorization();

        services.AddCascadingAuthenticationState();
        services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

        services.AddTransient<IIdentityService, IdentityService>();

        return services;
    }
}
