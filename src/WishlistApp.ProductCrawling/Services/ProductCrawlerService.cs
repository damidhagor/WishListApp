using System.Collections.Frozen;
using WishlistApp.ProductCrawling.ProductCrawlers;

namespace WishlistApp.ProductCrawling.Services;

internal sealed class ProductCrawlerService(
    IHttpClientFactory httpClientFactory,
    IEnumerable<IProductInformationParser> productInformationParsers)
    : IProductCrawlerService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly FrozenDictionary<string, IProductInformationParser> _productInformationCrawlers
        = productInformationParsers.ToFrozenDictionary(p => p.Host);

    public async Task<ProductInformation> CrawlProduct(Uri url, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(Constants.ProductCrawlerHttClientName);

        var html = await httpClient.GetStringAsync(url, cancellationToken);

        var host = url.Host.StartsWith("www.") ? url.Host.AsSpan(4).ToString() : url.Host;

        var result = _productInformationCrawlers.TryGetValue(host, out var parser)
            ? parser.ParseProductInformation(html)
            : _productInformationCrawlers["default"].ParseProductInformation(html);

        return result;
    }
}
