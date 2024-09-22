using System.Globalization;
using WishListApp.ProductCrawling.Parsers;

namespace WishListApp.Tests.ProductCrawling.Unit;

public sealed class OpenGraphParserTests
{
    [Theory]
    [InlineData("1", "USD", "1.0", "$")]
    [InlineData("1.0", "USD", "1.0", "$")]
    [InlineData("1.23", "USD", "1.23", "$")]
    [InlineData("1", "EUR", "1.0", "€")]
    [InlineData("1,0", "EUR", "1.0", "€")]
    [InlineData("1,23", "EUR", "1.23", "€")]
    public void Parse_PriceAndCurrency_FromOG(string htmlPrice, string htmlCurrency, string expectedPrice, string expectedCurrency)
    {
        var parser = new OpenGraphParser();
        var html =
            $"""
            <!DOCTYPE html>
            <html>
            <head>
                <meta property="og:price:amount" content="{htmlPrice}" />
                <meta property="og:price:currency" content="{htmlCurrency}" />
            </head>
            <body>
            </body>
            </html>
            """;

        var information = parser.Parse(html);

        Assert.Equal(decimal.Parse(expectedPrice, CultureInfo.InvariantCulture), information.Price);
        Assert.Equal(expectedCurrency, information.Currency);
    }

    [Theory]
    [InlineData("1", "USD", "1.0", "$")]
    [InlineData("1.0", "USD", "1.0", "$")]
    [InlineData("1.23", "USD", "1.23", "$")]
    [InlineData("1", "EUR", "1.0", "€")]
    [InlineData("1,0", "EUR", "1.0", "€")]
    [InlineData("1,23", "EUR", "1.23", "€")]
    public void Parse_PriceAndCurrency_FromProduct(string htmlPrice, string htmlCurrency, string expectedPrice, string expectedCurrency)
    {
        var parser = new OpenGraphParser();
        var html =
            $"""
            <!DOCTYPE html>
            <html>
            <head>
                <meta property="product:price:amount" content="{htmlPrice}" />
                <meta property="product:price:currency" content="{htmlCurrency}" />
            </head>
            <body>
            </body>
            </html>
            """;

        var information = parser.Parse(html);

        Assert.Equal(decimal.Parse(expectedPrice, CultureInfo.InvariantCulture), information.Price);
        Assert.Equal(expectedCurrency, information.Currency);
    }
}
