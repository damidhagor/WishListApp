using WishListApp.Models;

namespace WishListApp.Services;

public interface IUserService
{
    Task<WishListUser?> GetLoggedInWishListUser();
}
