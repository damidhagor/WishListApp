namespace WishListApp.ProductCrawling.ProductCrawlers;

internal interface IProductInformationParser
{
    string Host { get; }

    ProductInformation ParseProductInformation(string html);
}
