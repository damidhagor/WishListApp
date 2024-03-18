using WishlistApp.Identity.Endpoints;

namespace WishlistApp.Identity;

internal static class WebApplicationExtensions
{
    public static WebApplication UseIdentity(this WebApplication app)
    {
        app.MapIdentityEndpoints();
        return app;
    }
}