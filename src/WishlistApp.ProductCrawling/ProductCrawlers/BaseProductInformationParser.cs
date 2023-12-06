namespace WishlistApp.ProductCrawling.ProductCrawlers;

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