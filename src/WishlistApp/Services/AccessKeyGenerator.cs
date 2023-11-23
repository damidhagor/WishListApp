namespace WishlistApp.Services;

internal sealed class AccessKeyGenerator : IAccessKeyGenerator
{
    private const string _possibleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public string GenerateAccessKey(int length)
    {
        var random = new Random();

        var chars = random.GetItems<char>(_possibleChars, length);

        return new string(chars);
    }
}
