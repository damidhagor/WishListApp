using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using WishlistApp.Services;
using Constants = WishlistApp.Authentication.WishlistShareAuthenticationConstants;

namespace WishlistApp.Components.Pages;

public partial class SharedWishlist
{
    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; } = default!;

    [Inject]
    private IWishlistRepository Repository { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Parameter]
    public string AccessKey { get; set; } = "";

    private WishlistShareDto? WishlistShare { get; set; }

    private WishlistDto? Wishlist { get; set; }


    protected override async Task OnInitializedAsync()
    {
        await LoadAndValidateWishlistShare(default);
        await LoadWishlist(default);
    }

    private async Task LoadAndValidateWishlistShare(CancellationToken cancellationToken)
    {
        var state = await AuthenticationStateTask;

        var wishlistShareIdValue = state.User.Claims.FirstOrDefault(c => c.Type == Constants.WishlistShareIdClaim)?.Value;

        if (int.TryParse(wishlistShareIdValue, out var wishlistShareId))
        {
            WishlistShare = await Repository.GetWishlistShareById(wishlistShareId, cancellationToken);
        }

        if (WishlistShare is null)
        {
            NavigationManager.NavigateTo("/");
        }
    }

    private async Task LoadWishlist(CancellationToken cancellationToken)
    {
        Wishlist = WishlistShare is not null
            ? await Repository.GetWishlist(WishlistShare.WishlistId, default)
            : null;

        if (Wishlist is null)
        {
            NavigationManager.NavigateTo("/not-found");
            return;
        }
    }
}