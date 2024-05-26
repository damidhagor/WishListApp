namespace WishListApp.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWishListData(this IServiceCollection services, IConfiguration configuration, Action<MongoClientSettings>? configure = null)
    {
        var connectionString = configuration.GetConnectionString("mongodb");
        var mongoSettings = MongoClientSettings.FromConnectionString(connectionString);

        configure?.Invoke(mongoSettings);

        var mongoClient = new MongoClient(mongoSettings);

        services.AddSingleton<IMongoClient>(mongoClient);

        services.AddTransient<IAccessKeyGenerator, AccessKeyGenerator>();

        services.AddScoped<IWishListRepository, WishListRepository>();
        services.AddScoped<IWishListItemRepository, WishListItemRepository>();
        services.AddScoped<IWishListShareRepository, WishListShareRepository>();

        return services;
    }
}
