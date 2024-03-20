namespace WishlistApp.Data.Services;

internal sealed class AccessKeyGenerator(IConfiguration configuration) : IAccessKeyGenerator
{
    private const string _possibleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    private readonly string _baseUrl = configuration.GetValue<string>("ApplicationUrl") ?? throw new ArgumentNullException();

    public string GenerateAccessKey(int length)
    {
        var random = new Random();

        var chars = random.GetItems<char>(_possibleChars, length);

        return new string(chars);
    }

    public string GenerateWishlistShareUrl(string accessKey)
    {
        return $"{_baseUrl}/w/{accessKey}";
    }
}
