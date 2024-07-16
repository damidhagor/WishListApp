using WishListApp.ProductCrawling.Helpers;

namespace WishListApp.ProductCrawling.Parsers;

internal sealed class AmazonParser : IParser
{
    public string Host { get; } = "amazon.de";

    public ProductInformation Parse(ReadOnlySpan<char> html)
    {
        var title = html.GetValue("<title>", "</title>");
        var titleOG = html.GetValue("<meta property=\"og:title\" content=\"", "\"");

        var description = html.GetValue("<meta name=\"description\" content=\"", "\"");
        var descriptionOG = html.GetValue("<meta property=\"og:description\" content=\"", "\"");

        var imageDiv = html.GetValue("<div id=\"imgTagWrapperId\"", "</div>");
        var imageUrl = imageDiv.GetValue("src=\"", "\"");

        var priceWhole = html.GetValue("<span class=\"a-price-whole\">", "<span");
        var priceFraction = html.GetValue("<span class=\"a-price-fraction\">", "</span>");
        var price = priceWhole.Length > 0 ? $"{priceWhole}{(priceFraction.Length > 0 ? $",{priceFraction}" : "")}" : "";
        var priceValue = decimal.TryParse(price, out var parsedPrice) ? parsedPrice : 0;

        var currency = html.GetValue("<span class=\"a-price-symbol\">", "</span>");

        return new(
            "Amazon",
            titleOG.Length == 0 ? title.ToString() : titleOG.ToString(),
            descriptionOG.Length == 0 ? description.ToString() : descriptionOG.ToString(),
            imageUrl.ToString(),
            priceValue,
            currency.ToString());
    }
}
