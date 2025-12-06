namespace WishListApp.Services;

public interface IUserService
{
    Task<WishListUser?> GetLoggedInWishListUser();
}
