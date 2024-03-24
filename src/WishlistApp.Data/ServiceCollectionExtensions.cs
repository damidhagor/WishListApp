namespace WishListApp.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWishListData(this IServiceCollection services)
    {
        services.AddTransient<IAccessKeyGenerator, AccessKeyGenerator>();

        return services;
    }
}
