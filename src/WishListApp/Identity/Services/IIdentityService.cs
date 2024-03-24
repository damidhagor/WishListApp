namespace WishListApp.Identity.Services;

public interface IIdentityService
{
    void Login(string? redirectUrl = null);

    void Logout(string? redirectUrl = null);

    void EditProfile(string? redirectUrl = null);
}
