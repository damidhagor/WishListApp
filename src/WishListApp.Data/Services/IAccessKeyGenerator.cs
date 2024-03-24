namespace WishListApp.Data.Services;

public interface IAccessKeyGenerator
{
    string GenerateAccessKey(int length);

    string GenerateShareUrl(string accessKey);
}
