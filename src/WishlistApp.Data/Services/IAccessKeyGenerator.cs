namespace WishlistApp.Data.Services;

public interface IAccessKeyGenerator
{
    string GenerateAccessKey(int length);

    string GenerateWishlistShareUrl(string accessKey);
}