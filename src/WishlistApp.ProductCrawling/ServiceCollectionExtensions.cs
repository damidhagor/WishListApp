using Microsoft.Extensions.DependencyInjection;
using WishlistApp.ProductCrawling.ProductCrawlers;
using WishlistApp.ProductCrawling.Services;

namespace WishlistApp.ProductCrawling;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProductCrawler(this IServiceCollection services)
    {
        services.AddTransient<IProductInformationParser, DefaultProductInformationParser>();
        services.AddSingleton<IProductCrawlerService, ProductCrawlerService>();

        services.AddHttpClient(Constants.ProductCrawlerHttClientName, client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", Constants.CrawlerUserAgent);
        });

        return services;
    }
}
