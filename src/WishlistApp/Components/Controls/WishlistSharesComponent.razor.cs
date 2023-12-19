using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishlistApp.Services;

namespace WishlistApp.Components.Controls;

public partial class WishlistSharesComponent
{
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private IWishlistRepository Repository { get; set; } = default!;

    [Parameter, EditorRequired]
    public WishlistDto? Wishlist { get; set; }

    private string _newShareName = "";

    private bool _isNewShareNameEmpty => string.IsNullOrWhiteSpace(_newShareName);

    private async Task AddNewWishlistShare()
    {
        if (string.IsNullOrWhiteSpace(_newShareName) || Wishlist is null)
        {
            return;
        }

        var newWishlist = await Repository.AddWishlistShare(Wishlist.Id, _newShareName, default);

        if (newWishlist is not null)
        {
            Wishlist = newWishlist;
            _newShareName = "";
        }
    }

    private async Task DeleteWishlistShare(WishlistShareDto? wishlistShare)
    {
        if (Wishlist is null || wishlistShare is null)
        {
            return;
        }

        bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", $"Möchten Sie die Freigabe \"{wishlistShare.Name}\" löschen?");
        if (confirmed)
        {
            var wishlist = await Repository.DeleteWishlistShare(Wishlist.Id, wishlistShare.Id, default);

            if (wishlist is not null)
            {
                Wishlist = wishlist;
                StateHasChanged();
            }
        }
    }
}