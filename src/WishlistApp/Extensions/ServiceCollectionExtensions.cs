using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using WishlistApp.Authentication;
using WishlistApp.Components.Account;
using WishlistApp.Data;
using Constants = WishlistApp.Authentication.WishlistShareAuthenticationConstants;

namespace WishlistApp.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWishlistIdentity(this IServiceCollection services)
    {
        services.AddCascadingAuthenticationState();
        services.AddScoped<IdentityUserAccessor>();
        services.AddScoped<IdentityRedirectManager>();
        services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
        services.AddSingleton<IEmailSender<WishlistUser>, IdentityNoOpEmailSender>();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
            .AddIdentityCookies();

        services.AddIdentityCore<WishlistUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<WishlistRole>()
            .AddEntityFrameworkStores<WishlistDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddWishlistShareAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(Constants.AuthenticationSchemeName)
            .AddScheme<WishlistShareAuthenticationSchemeOptions, WishlistShareAuthenticationSchemeHandler>(
                Constants.AuthenticationSchemeName,
                null);

        services.AddAuthorizationBuilder()
            .AddPolicy(
                Constants.PolicyName,
                policy => policy
                    .RequireRole(Constants.WishlistShareRole)
                    .RequireClaim(Constants.WishlistShareIdClaim)
                    .AddAuthenticationSchemes(Constants.AuthenticationSchemeName));

        return services;
    }
}