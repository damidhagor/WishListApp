using System.Globalization;

namespace WishListApp.ProductCrawling.ProductCrawlers;

internal sealed class DefaultProductInformationParser : BaseProductInformationParser
{
    public override string Host { get; } = "default";

    public override ProductInformation ParseProductInformation(string html)
    {
        var head = GetValue(html, "<head>", "</head>");

        var title = GetValue(head, "<title>", "</title>");
        var titleOG = GetValue(head, "<meta property=\"og:title\" content=\"", "\"");

        var description = GetValue(head, "<meta name=\"description\" content=\"", "\"");
        var descriptionOG = GetValue(head, "<meta property=\"og:description\" content=\"", "\"");

        var imageUrl = GetValue(head, "<meta property=\"og:image\" content=\"", "\"");

        var imageUrlSecure = GetValue(head, "<meta property=\"og:image:secure_url\" content=\"", "\"");

        var price = GetValue(head, "<meta property=\"product:price:amount\" content=\"", "\"");
        var priceOG = GetValue(head, "<meta property=\"og:price:amount\" content=\"", "\"");
        var priceValue = decimal.TryParse(priceOG.Length == 0 ? price.ToString() : priceOG.ToString(), CultureInfo.InvariantCulture, out var parsedPrice)
            ? parsedPrice
            : 0;

        var currency = GetValue(head, "<meta property=\"product:price:currency\" content=\"", "\"");
        var currencyOG = GetValue(head, "<meta property=\"og:price:currency\" content=\"", "\"");

        return new(
            titleOG.Length == 0 ? title.ToString() : titleOG.ToString(),
            descriptionOG.Length == 0 ? description.ToString() : descriptionOG.ToString(),
            imageUrlSecure.Length == 0 ? imageUrl.ToString() : imageUrlSecure.ToString(),
            priceValue,
            currencyOG.Length == 0 ? currency.ToString() : currencyOG.ToString());
    }
}
