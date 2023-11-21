using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using WishlistApp.Components.Account;
using WishlistApp.Data;

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
}