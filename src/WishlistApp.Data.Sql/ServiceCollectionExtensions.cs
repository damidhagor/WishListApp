using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WishlistApp.Data.Sql;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWishlistSqlData(this IServiceCollection services)
    {
        services.AddDbContext<WishlistDbContext>((serviceProvider, options) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("SqlServer");
            options.UseNpgsql(connectionString);
        });

        return services;
    }
}
