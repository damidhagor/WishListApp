using Microsoft.AspNetCore.Authentication;

namespace WishlistApp.Authentication;

internal sealed class WishlistShareAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public string? AccessKeyRouteValueKey { get; set; }
}
