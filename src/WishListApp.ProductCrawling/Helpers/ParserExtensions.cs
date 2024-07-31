using System.Globalization;

namespace WishListApp.ProductCrawling.Helpers;

internal static class ParserExtensions
{
    public static ReadOnlySpan<char> GetValue(this ReadOnlySpan<char> input, string startToken, string endToken)
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

    public static string? GetValueAsString(this ReadOnlySpan<char> input, string startToken, string endToken)
    {
        var value = input.GetValue(startToken, endToken);
        return value.Length == 0 ? null : value.ToString();
    }

    public static decimal? ToDecimalByCurrency(this string? price, string? currency)
    {
        if (string.IsNullOrEmpty(price))
        {
            return null;
        }

        if (currency is "EUR" || currency is "€")
        {
            price = price.Replace(',', '.');
        }

        return decimal.TryParse(price, CultureInfo.InvariantCulture, out var parsedPrice)
            ? parsedPrice
            : null;
    }

    public static string? ToCurrencySymbol(this string? currency)
        => currency switch
        {
            "AED" => "د.إ.",
            "AFN" => "Af",
            "ALL" => "L",
            "AMD" => "֏",
            "ANG" => "ƒ",
            "AOA" => "Kz",
            "ARS" => "AR$",
            "AUD" => "AU$",
            "AWG" => "ƒ",
            "AZN" => "ман",
            "BAM" => "KM",
            "BBD" => "BBD$",
            "BDT" => "৳",
            "BGN" => "лв.",
            "BHD" => "BD",
            "BIF" => "FBu",
            "BMD" => "$",
            "BND" => "B$",
            "BOB" => "Bs.",
            "BRL" => "R$",
            "BSD" => "$",
            "BTN" => "Nu.",
            "BWP" => "P",
            "BYN" => "Br",
            "BZD" => "BZ$",
            "CAD" => "CA$",
            "CDF" => "FC",
            "CHF" => "Fr.",
            "CKD" => "$",
            "CLP" => "CL$",
            "CNY" => "CN¥",
            "COP" => "CO$",
            "CRC" => "₡",
            "CUC" => "CUC$",
            "CUP" => "$MN",
            "CVE" => "CV$",
            "CZK" => "Kč",
            "DJF" => "Fdj",
            "DKK" => "kr.",
            "DOP" => "RD$",
            "DZD" => "DA",
            "EGP" => "E£",
            "EHP" => "Ptas.",
            "ERN" => "Nkf",
            "ETB" => "Br",
            "EUR" => "€",
            "FJD" => "FJ$",
            "FKP" => "FK£",
            "FOK" => "kr",
            "GBP" => "£",
            "GEL" => "₾",
            "GGP" => "£",
            "GHS" => "GH₵",
            "GIP" => "£",
            "GMD" => "D",
            "GNF" => "FG",
            "GTQ" => "Q",
            "GYD" => "G$",
            "HKD" => "HK$",
            "HNL" => "L",
            "HRK" => "kn",
            "HTG" => "G",
            "HUF" => "Ft",
            "IDR" => "Rp",
            "ILS" => "₪",
            "IMP" => "£",
            "INR" => "Rs.",
            "IQD" => "د.ع.",
            "IRR" => "﷼",
            "ISK" => "kr",
            "JEP" => "£",
            "JMD" => "J$",
            "JOD" => "JD",
            "JPY" => "¥",
            "KES" => "KSh",
            "KGS" => "с",
            "KHR" => "៛",
            "KID" => "$",
            "KMF" => "CF",
            "KPW" => "₩",
            "KRW" => "₩",
            "KWD" => "KD",
            "KYD" => "CI$",
            "KZT" => "₸",
            "LAK" => "₭N",
            "LBP" => "LL.",
            "LKR" => "Rs.",
            "LRD" => "L$",
            "LSL" => "L",
            "LYD" => "LD",
            "MAD" => "DH",
            "MDL" => "L",
            "MGA" => "Ar",
            "MKD" => "den",
            "MMK" => "Ks",
            "MNT" => "₮",
            "MOP" => "MOP$",
            "MRU" => "UM",
            "MUR" => "Rs.",
            "MVR" => "MRf",
            "MWK" => "MK",
            "MXN" => "MX$",
            "MYR" => "RM",
            "MZN" => "MTn",
            "NAD" => "N$",
            "NGN" => "₦",
            "NIO" => "C$",
            "NOK" => "kr",
            "NPR" => "Rs.",
            "NZD" => "NZ$",
            "OMR" => "OR",
            "PAB" => "B/.",
            "PEN" => "S/.",
            "PGK" => "K",
            "PHP" => "₱",
            "PKR" => "Rs.",
            "PLN" => "zł",
            "PND" => "$",
            "PRB" => "р.",
            "PYG" => "₲",
            "QAR" => "QR",
            "RON" => "L",
            "RSD" => "din",
            "RUB" => "₽",
            "RWF" => "FRw",
            "SAR" => "SR",
            "SBD" => "SI$",
            "SCR" => "Rs.",
            "SDG" => "£SD",
            "SEK" => "kr",
            "SGD" => "S$",
            "SHP" => "£",
            "SLL" => "Le",
            "SLS" => "Sl",
            "SOS" => "Sh.So.",
            "SRD" => "Sr$",
            "SSP" => "SS£",
            "STN" => "Db",
            "SVC" => "₡",
            "SYP" => "LS",
            "SZL" => "L",
            "THB" => "฿",
            "TJS" => "SM",
            "TMT" => "m.",
            "TND" => "DT",
            "TOP" => "T$",
            "TRY" => "TL",
            "TTD" => "TT$",
            "TVD" => "$",
            "TWD" => "NT$",
            "TZS" => "TSh",
            "UAH" => "₴",
            "UGX" => "USh",
            "USD" => "$",
            "UYU" => "$U",
            "UZS" => "сум",
            "VED" => "Bs.",
            "VES" => "Bs.F",
            "VND" => "₫",
            "VUV" => "VT",
            "WST" => "T",
            "XAF" => "Fr",
            "XCD" => "$",
            "XOF" => "₣",
            "XPF" => "₣",
            "YER" => "YR",
            "ZAR" => "R",
            "ZMW" => "ZK",
            "ZWB" => "",
            "ZWL" => "Z$",
            "Abkhazia" => "",
            "Artsakh" => "դր.",
            _ => null
        };
}
