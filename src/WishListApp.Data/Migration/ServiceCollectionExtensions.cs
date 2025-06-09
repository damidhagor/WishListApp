using CommunityToolkit.Mvvm.Messaging;
using WishListApp.Data.Migration.Migrations;
using WishListApp.Data.Migration.Services;

namespace WishListApp.Data.Migration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMigrationService(this IServiceCollection services)
    {
        services.AddWishListMigrations();
        services.AddWishListShareMigrations();

        services.AddSingleton<IMigrationExecutor<WishList>>(
            serviceProvider =>
            {
                var databaseName = serviceProvider
                    .GetRequiredService<IConfiguration>()
                    .GetValue<string>("MongoDBDatabaseName");

                var messenger = serviceProvider.GetRequiredKeyedService<IMessenger>("MigrationMessenger");
                var migrations = serviceProvider.GetServices<IMigration<WishList>>();
                var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();
                var collection = mongoClient.GetDatabase(databaseName)
                    .GetCollection<BsonDocument>(MongoDBConstants.WishListsCollectionName);

                return new MigrationExecutor<WishList>(collection, migrations, messenger);
            });

        services.AddSingleton<IMigrationExecutor<WishListShare>>(
            serviceProvider =>
            {
                var databaseName = serviceProvider
                    .GetRequiredService<IConfiguration>()
                    .GetValue<string>("MongoDBDatabaseName");

                var messenger = serviceProvider.GetRequiredKeyedService<IMessenger>("MigrationMessenger");
                var migrations = serviceProvider.GetServices<IMigration<WishListShare>>();
                var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();
                var collection = mongoClient.GetDatabase(databaseName)
                    .GetCollection<BsonDocument>(MongoDBConstants.SharesCollectionName);

                return new MigrationExecutor<WishListShare>(collection, migrations, messenger);
            });

        return services;
    }

    public static IServiceCollection AddWishListMigrations(this IServiceCollection services)
    {
        services.AddSingleton<IMigration<WishList>, Migrations.WishList.V01_WishListMigration>();
        return services;
    }

    public static IServiceCollection AddWishListShareMigrations(this IServiceCollection services)
    {
        services.AddSingleton<IMigration<WishListShare>, Migrations.WishListShare.V01_WishListShareMigration>();
        return services;
    }
}
