using WishListApp.ProductCrawling.Helpers;

namespace WishListApp.ProductCrawling.Parsers;

internal sealed class AmazonParser : IParser
{
    public string Host { get; } = "amazon.de";

    public ProductInformation Parse(ReadOnlySpan<char> html)
    {
        var title = html.GetValueAsString("<title>", "</title>");

        var description = html.GetValueAsString("<meta name=\"description\" content=\"", "\"");

        var imageDiv = html.GetValue("<div id=\"imgTagWrapperId\"", "</div>");
        var imageUrl = imageDiv.GetValueAsString("src=\"", "\"");

        var priceWhole = html.GetValue("<span class=\"a-price-whole\">", "<span");
        var priceFraction = html.GetValue("<span class=\"a-price-fraction\">", "</span>");

        var price = priceWhole.Length > 0 || priceFraction.Length > 0
            ? $"{(priceWhole.Length > 0 ? priceWhole : "0")}.{(priceFraction.Length > 0 ? priceFraction : "00")}"
            : "";

        var priceValue = decimal.TryParse(price, out var parsedPrice)
            ? parsedPrice
            : (decimal?)null;

        var currency = html.GetValueAsString("<span class=\"a-price-symbol\">", "</span>");

        return new(
            "Amazon",
            title,
            description,
            imageUrl,
            priceValue,
            currency);
    }
}
