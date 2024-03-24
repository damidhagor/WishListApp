using WishListApp.Identity.Endpoints;

namespace WishListApp.Identity;

internal static class WebApplicationExtensions
{
    public static WebApplication UseIdentity(this WebApplication app)
    {
        app.MapIdentityEndpoints();
        return app;
    }
}