using Microsoft.AspNetCore.Components;

namespace WishListApp.Components;

public sealed partial class PurchaseButtonComponent
{
    [CascadingParameter]
    public WishListItem Item { get; set; } = default!;

    [CascadingParameter]
    public WishListViewModel ViewModel { get; set; } = default!;

    [Parameter]
    public string? Class { get; set; }

    private string _hiddenClass => ViewModel.LoggedInShare is null && !Item.IsPurchased ? "d-none" : "d-show";

    private string _backgroundClass
        => Item.IsPurchasedByShare(ViewModel.LoggedInShare?.Id)
            ? "bg-warning-subtle"
            : Item.IsPurchasedByOtherShare(ViewModel.LoggedInShare?.Id)
                ? "bg-success"
                : "bg-success-subtle";

    private string _buttonClass
        => Item.IsPurchasedByShare(ViewModel.LoggedInShare?.Id)
            ? "btn-outline-warning"
            : Item.IsPurchasedByOtherShare(ViewModel.LoggedInShare?.Id)
                ? "btn-success disabled"
                : "btn-outline-success";

    private async Task ChangePurchase()
    {
        if (Item.IsPurchasedByShare(ViewModel.LoggedInShare?.Id))
        {
            await ViewModel.ResetWishListItemPurchase(Item, default);
        }
        else if (!Item.IsPurchased)
        {
            await ViewModel.MarkWishListItemAsPurchased(Item, default);
        }
    }
}
