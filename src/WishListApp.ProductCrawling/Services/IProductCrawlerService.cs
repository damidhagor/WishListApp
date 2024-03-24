namespace WishListApp.ProductCrawling.Services;

public interface IProductCrawlerService
{
    Task<ProductInformation> CrawlProduct(Uri url, CancellationToken cancellationToken);
}
