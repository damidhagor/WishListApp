namespace WishlistApp.ProductCrawling.ProductCrawlers;

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

        var price = GetValue(head, "<meta property=\"og:price:amount\" content=\"", "\"");
        var currency = GetValue(head, "<meta property=\"og:price:currency\" content=\"", "\"");

        return new(
            titleOG.Length == 0 ? title.ToString() : titleOG.ToString(),
            descriptionOG.Length == 0 ? description.ToString() : descriptionOG.ToString(),
            imageUrlSecure.Length == 0 ? imageUrl.ToString() : imageUrlSecure.ToString(),
            price.ToString(),
            currency.ToString());
    }
}

internal abstract class BaseProductInformationParser : IProductInformationParser
{
    public abstract string Host { get; }

    public abstract ProductInformation ParseProductInformation(string html);

    protected ReadOnlySpan<char> GetValue(ReadOnlySpan<char> input, string startToken, string endToken)
    {
        var valueStart = input.IndexOf(startToken);
        if (valueStart == -1)
        {
            return [];
        }

        valueStart += startToken.Length;
        var valueEnd = valueStart + input[valueStart..].IndexOf(endToken);

        return valueEnd == -1
            ? []
            : input[valueStart..valueEnd].Trim();
    }
}