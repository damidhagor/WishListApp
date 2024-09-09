using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using WishListApp.Identity.Services;

namespace WishListApp.Identity.Components;

public partial class IdentityHeader(IIdentityService accountService)
{
    private readonly IIdentityService _accountService = accountService;

    [Parameter]
    public string? Class { get; set; }

    private static string GetUsername(AuthenticationState authenticationState)
    {
        var user = authenticationState.User;

        var nameClaim = user.Claims.FirstOrDefault(c => c.Type == "name");
        return nameClaim?.Value ?? user.Identity?.Name ?? "";
    }
}
