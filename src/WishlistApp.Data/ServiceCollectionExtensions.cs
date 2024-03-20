using WishlistApp.Data.Services;

namespace WishlistApp.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWishlistData(this IServiceCollection services)
    {
        services.AddTransient<IAccessKeyGenerator, AccessKeyGenerator>();

        return services;
    }
}
