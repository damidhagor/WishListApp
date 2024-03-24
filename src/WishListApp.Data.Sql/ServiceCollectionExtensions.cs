using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WishListApp.Data.Sql;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWishListSqlData(this IServiceCollection services)
    {
        services.AddDbContext<WishListDbContext>((serviceProvider, options) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("SqlServer");
            options.UseNpgsql(connectionString);
        });

        return services;
    }
}
