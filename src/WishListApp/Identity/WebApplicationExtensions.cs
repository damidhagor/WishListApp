using WishListApp.Identity.Endpoints;

namespace WishListApp.Identity;

internal static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public WebApplication UseIdentity()
        {
            app.MapIdentityEndpoints();
            return app;
        }
    }
}
