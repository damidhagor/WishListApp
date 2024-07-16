namespace WishListApp.ProductCrawling.Parsers;

internal interface IParser
{
    string Host { get; }

    ProductInformation Parse(ReadOnlySpan<char> html);
}
