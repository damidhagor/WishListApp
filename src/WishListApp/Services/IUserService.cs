using WishlistApp.Models;

namespace WishlistApp.Services;

public interface IUserService
{
    Task<WishlistUser?> GetLoggedInWishlistUser();
}
