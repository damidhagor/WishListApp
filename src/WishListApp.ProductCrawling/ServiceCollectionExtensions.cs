using Microsoft.Extensions.DependencyInjection;
using WishListApp.ProductCrawling.Parsers;
using WishListApp.ProductCrawling.Services;

namespace WishListApp.ProductCrawling;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProductCrawler(this IServiceCollection services)
    {
        services.AddTransient<IParser, OpenGraphParser>();
        services.AddTransient<IParser, AmazonParser>();
        services.AddSingleton<IProductCrawlerService, ProductCrawlerService>();

        services.AddHttpClient(Constants.ProductCrawlerHttClientName, client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", Constants.CrawlerUserAgent);
        })
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            return new HttpClientHandler()
            {
                AutomaticDecompression = System.Net.DecompressionMethods.All
            };
        });

        return services;
    }
}
