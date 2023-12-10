namespace WishlistApp.Services;

public interface IUserService
{
    Task<WishlistUserDto?> GetLoggedInWishlistUser();
}
