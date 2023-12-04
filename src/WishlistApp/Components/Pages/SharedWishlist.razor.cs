using Microsoft.AspNetCore.Components;
using WishlistApp.Services;

namespace WishlistApp.Components.Pages;

public partial class SharedWishlist
{
    [Inject]
    private IWishlistRepository Repository { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Parameter]
    public string AccessKey { get; set; } = "";

    private WishlistShareDto? WishlistShare { get; set; }

    private WishlistDto? Wishlist { get; set; }


    protected override async Task OnParametersSetAsync()
    {
        await LoadAndValidateWishlistShare(default);
        await LoadWishlist(default);
    }

    private async Task LoadAndValidateWishlistShare(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(AccessKey))
        {
            WishlistShare = await Repository.GetWishlistShareByAccessKey(AccessKey, cancellationToken);
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

    private async Task OnItemBought(WishlistItemDto itemDto)
    {
        Wishlist = await Repository.BuyWishlistItem(itemDto.WishlistId, itemDto.Id, WishlistShare!.Id, default);
    }

    private async Task OnItemUnbought(WishlistItemDto itemDto)
    {
        Wishlist = await Repository.UnbuyWishlistItem(itemDto.WishlistId, itemDto.Id, WishlistShare!.Id, default);
    }
}