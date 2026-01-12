using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WishListApp.Identity.Endpoints;

public static class IdentityEndpoints
{
    public const string LoginEndpoint = $"account/login";
    public const string LogoutEndpoint = $"account/logout";
    public const string EditProfileEndpoint = $"account/profile";

    public static WebApplication MapIdentityEndpoints(this WebApplication app)
    {
        app.MapGet(LoginEndpoint, Login)
            .AllowAnonymous();
        app.MapGet(LogoutEndpoint, Logout)
            .RequireAuthorization();
        app.MapGet(EditProfileEndpoint, EditProfile)
            .RequireAuthorization();

        return app;
    }

    private static async Task Login(
        [FromServices] IAuthenticationService authenticationService,
        [FromQuery] string? redirectUrl,
        HttpContext httpContext)
    {
        await authenticationService.ChallengeAsync(httpContext, null, new() { RedirectUri = redirectUrl });
    }

    private static async Task Logout(
        [FromServices] IAuthenticationService authenticationService,
        [FromQuery] string? redirectUrl,
        HttpContext httpContext)
    {
        await authenticationService.SignOutAsync(httpContext, CookieAuthenticationDefaults.AuthenticationScheme, null);
        await authenticationService.SignOutAsync(httpContext, OpenIdConnectDefaults.AuthenticationScheme, new() { RedirectUri = redirectUrl });
    }

    private static void EditProfile(
        [FromServices] IOptions<IdentityOptions> identityOptions,
        HttpContext httpContext)
    {
        Guard.IsNotNull(identityOptions.Value);
        httpContext.Response.Redirect(identityOptions.Value.ProfileUrl);
    }
}
