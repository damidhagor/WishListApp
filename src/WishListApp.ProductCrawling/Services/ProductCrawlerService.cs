using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using WishListApp.ProductCrawling.Parsers;

namespace WishListApp.ProductCrawling.Services;

internal sealed class ProductCrawlerService(
    IHttpClientFactory httpClientFactory,
    TimeProvider timeProvider,
    IEnumerable<IParser> productInformationParsers)
    : IProductCrawlerService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly FrozenDictionary<string, IParser> _productInformationCrawlers = productInformationParsers.ToFrozenDictionary(p => p.Host);
    private readonly Dictionary<string, (ProductInformation ProductInformation, DateTimeOffset CreatedAt)> _cache = [];

    public async Task<ProductInformation> CrawlProduct(Uri url, CancellationToken cancellationToken)
    {
        if (TryGetCachedProductInformation(url.ToString(), out var cachedProductInformation))
        {
            return cachedProductInformation;
        }

        var httpClient = _httpClientFactory.CreateClient(Constants.ProductCrawlerHttClientName);

        var html = await httpClient.GetStringAsync(url, cancellationToken);
        html = WebUtility.HtmlDecode(html);

        var host = url.Host.StartsWith("www.") ? url.Host.AsSpan(4).ToString() : url.Host;

        var result = _productInformationCrawlers.TryGetValue(host, out var parser)
            ? parser.Parse(html)
            : _productInformationCrawlers[Constants.DefaultParserHost].Parse(html);


        _cache[url.ToString()] = (result, _timeProvider.GetUtcNow());

        return result;
    }

    private bool TryGetCachedProductInformation(string url, [NotNullWhen(true)] out ProductInformation? productInformation)
    {
        productInformation = null;

        if (!_cache.TryGetValue(url, out var cachedProductInformation))
        {
            return false;
        }

        if (_timeProvider.GetUtcNow() - cachedProductInformation.CreatedAt > TimeSpan.FromDays(1))
        {
            return false;
        }

        productInformation = cachedProductInformation.ProductInformation;
        return true;
    }
}
