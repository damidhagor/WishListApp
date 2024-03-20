using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using WishlistApp.Models;

namespace WishlistApp.Services;

internal sealed class UserService(AuthenticationStateProvider authenticationStateProvider) : IUserService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider = authenticationStateProvider;

    public async Task<WishlistUser?> GetLoggedInWishlistUser()
    {
        var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        if (authenticationState.User.Identity is null
            || !authenticationState.User.Identity.IsAuthenticated)
        {
            return null;
        }

        var identifier = authenticationState.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        var name = authenticationState.User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(identifier)
            || string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        var claims = authenticationState.User.Claims
            .Select(c => (c.Type, c.Value))
            .ToArray();

        var roles = claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToArray();

        return new WishlistUser(identifier, name, roles, claims);
    }
}
