namespace WishListApp.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWishListData(this IServiceCollection services)
    {
        services.AddTransient<IAccessKeyGenerator, AccessKeyGenerator>();

        services.AddScoped<IWishListRepository, WishListRepository>();
        services.AddScoped<IWishListItemRepository, WishListItemRepository>();
        services.AddScoped<IWishListShareRepository, WishListShareRepository>();

        return services;
    }
}
