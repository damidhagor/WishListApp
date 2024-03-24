using Microsoft.AspNetCore.Components;
using WishListApp.Identity.Endpoints;

namespace WishListApp.Identity.Services;

public sealed class IdentityService(NavigationManager navigationManager) : IIdentityService
{
    private readonly NavigationManager _navigationManager = navigationManager;

    public void Login(string? redirectUrl = null)
    {
        var url = redirectUrl is null
            ? IdentityEndpoints.LoginEndpoint
            : $"{IdentityEndpoints.LoginEndpoint}?redirectUrl={redirectUrl}";

        _navigationManager.NavigateTo(url, true);
    }

    public void Logout(string? redirectUrl = null)
    {
        var url = redirectUrl is null
            ? IdentityEndpoints.LogoutEndpoint
            : $"{IdentityEndpoints.LogoutEndpoint}?redirectUrl={redirectUrl}";

        _navigationManager.NavigateTo(url, true);
    }

    public void EditProfile(string? redirectUrl = null)
    {
        var url = redirectUrl is null
            ? IdentityEndpoints.EditProfileEndpoint
            : $"{IdentityEndpoints.EditProfileEndpoint}?redirectUrl={redirectUrl}";

        _navigationManager.NavigateTo(url, true);
    }
}
