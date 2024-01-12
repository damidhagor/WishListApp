using Microsoft.AspNetCore.Components;
using WishlistApp.Components.Controls;

namespace WishlistApp.Components.Wishlist;

public partial class SharesModalComponent
{
    [Parameter, EditorRequired]
    public WishlistShareDto[] Shares { get; set; } = [];

    [Parameter]
    public EventCallback<string> ShareAdded { get; set; }

    [Parameter]
    public EventCallback<WishlistShareDto> ShareDeleted { get; set; }

    private ModalComponent _modal = default!;
    private string _newShareName = "";

    private bool _isNewShareNameEmpty => string.IsNullOrWhiteSpace(_newShareName);

    public async Task Open() => await _modal.Open();

    private async Task AddNewWishlistShare()
    {
        if (string.IsNullOrWhiteSpace(_newShareName))
        {
            return;
        }

        await ShareAdded.InvokeAsync(_newShareName);
        _newShareName = "";
    }

    private async Task DeleteWishlistShare(WishlistShareDto? wishlistShare)
    {
        if (wishlistShare is null)
        {
            return;
        }

        await ShareDeleted.InvokeAsync(wishlistShare);
    }
}