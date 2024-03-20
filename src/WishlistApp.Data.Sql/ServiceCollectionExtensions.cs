using WishlistApp.Data.Repositories;
using WishlistApp.Data.Sql.Repositories;

namespace WishlistApp.Data.Sql;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWishlistSqlData(this IServiceCollection services)
    {
        services.AddWishlistData();

        services.AddDbContext<WishlistDbContext>((serviceProvider, options) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("SqlServer");
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IWishlistRepository, WishlistRepository>();
        services.AddScoped<IWishlistItemRepository, WishlistItemRepository>();
        services.AddScoped<IWishlistShareRepository, WishlistShareRepository>();
        services.AddScoped<IWishlistPurchaseRepository, WishlistPurchaseRepository>();

        return services;
    }
}
