namespace WishlistApp.ProductCrawling.ProductCrawlers;

internal sealed class AmazonProductInformationParser : BaseProductInformationParser
{
    public override string Host { get; } = "amazon.de";

    public override ProductInformation ParseProductInformation(string html)
    {
        var title = GetValue(html, "<title>", "</title>");
        var titleOG = GetValue(html, "<meta property=\"og:title\" content=\"", "\"");

        var description = GetValue(html, "<meta name=\"description\" content=\"", "\"");
        var descriptionOG = GetValue(html, "<meta property=\"og:description\" content=\"", "\"");

        var imageDiv = GetValue(html, "<div id=\"imgTagWrapperId\"", "</div>");
        var imageUrl = GetValue(imageDiv, "src=\"", "\"");

        var priceWhole = GetValue(html, "<span class=\"a-price-whole\">", "<span");
        var priceFraction = GetValue(html, "<span class=\"a-price-fraction\">", "</span>");
        var price = priceWhole.Length > 0 ? $"{priceWhole}{(priceFraction.Length > 0 ? $",{priceFraction}" : "")}" : "";

        var currency = GetValue(html, "<span class=\"a-price-symbol\">", "</span>");

        return new(
            titleOG.Length == 0 ? title.ToString() : titleOG.ToString(),
            descriptionOG.Length == 0 ? description.ToString() : descriptionOG.ToString(),
            imageUrl.ToString(),
            price,
            currency.ToString());
    }
}
