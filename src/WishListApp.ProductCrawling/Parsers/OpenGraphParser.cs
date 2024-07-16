using System.Globalization;
using WishListApp.ProductCrawling.Helpers;

namespace WishListApp.ProductCrawling.Parsers;

internal sealed class OpenGraphParser : IParser
{
    public string Host { get; } = Constants.DefaultParserHost;

    public ProductInformation Parse(ReadOnlySpan<char> html)
    {
        var head = html.GetValue("<head>", "</head>");

        var siteName = head.GetValueAsString("<meta property=\"og:site_name\" content=\"", "\"");

        var title = head.GetValueAsString("<meta property=\"og:title\" content=\"", "\"");

        var description = head.GetValueAsString("<meta property=\"og:description\" content=\"", "\"");

        var image = head.GetValueAsString("<meta property=\"og:image\" content=\"", "\"");

        var imageSecure = head.GetValueAsString("<meta property=\"og:image:secure_url\" content=\"", "\"");

        var price = head.GetValueAsString("<meta property=\"og:price:amount\" content=\"", "\"")
            ?? head.GetValueAsString("<meta property=\"product:price:amount\" content=\"", "\"");

        var currency = head.GetValueAsString("<meta property=\"og:price:currency\" content=\"", "\"")
            ?? head.GetValueAsString("<meta property=\"product:price:currency\" content=\"", "\"");

        var parsedPrice = price.ToDecimalByCurrency(currency);
        var currencySymbol = currency.ToCurrencySymbol();

        return new(
            siteName,
            title,
            description,
            imageSecure ?? image,
            parsedPrice,
            currencySymbol);
    }
}
