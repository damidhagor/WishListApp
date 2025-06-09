using Microsoft.Extensions.Hosting;

namespace WishListApp.Data;

public static class ServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddWishListData(this IHostApplicationBuilder builder, IConfiguration configuration, Action<MongoClientSettings>? configure = null)
    {
        builder.AddMongoDBClient("mongodb", configureClientSettings: configure);

        builder.Services.AddTransient<IAccessKeyGenerator, AccessKeyGenerator>();

        builder.Services.AddScoped<IWishListRepository>(
            serviceProvider =>
            {
                var databaseName = serviceProvider
                    .GetRequiredService<IConfiguration>()
                    .GetValue<string>("MongoDBDatabaseName");

                var database = serviceProvider
                    .GetRequiredService<IMongoClient>()
                    .GetDatabase(databaseName);

                return new WishListRepository(database);
            });

        builder.Services.AddScoped<IWishListItemRepository>(
            serviceProvider =>
            {
                var databaseName = serviceProvider
                    .GetRequiredService<IConfiguration>()
                    .GetValue<string>("MongoDBDatabaseName");

                var database = serviceProvider
                    .GetRequiredService<IMongoClient>()
                    .GetDatabase(databaseName);

                return new WishListItemRepository(database);
            });

        builder.Services.AddScoped<IWishListShareRepository>(
            serviceProvider =>
            {
                var databaseName = serviceProvider
                    .GetRequiredService<IConfiguration>()
                    .GetValue<string>("MongoDBDatabaseName");

                var database = serviceProvider
                    .GetRequiredService<IMongoClient>()
                    .GetDatabase(databaseName);

                return new WishListShareRepository(database);
            });

        return builder;
    }
}
