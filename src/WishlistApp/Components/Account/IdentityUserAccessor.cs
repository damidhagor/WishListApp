using Microsoft.AspNetCore.Identity;
using WishlistApp.Data.Models;

namespace WishlistApp.Components.Account;

internal sealed class IdentityUserAccessor(UserManager<WishlistUser> userManager, IdentityRedirectManager redirectManager)
{
    public async Task<WishlistUser> GetRequiredUserAsync(HttpContext context)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null)
        {
            redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
        }

        return user;
    }
}
