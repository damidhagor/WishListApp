using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishListApp.ProductCrawling.Services;

namespace WishListApp.Components.Wishlist;

public partial class ItemComponent
{
    [Inject]
    private IJSRuntime _jsRuntime { get; set; } = default!;

    [Inject]
    private IStringLocalizer<Localization> _localizer { get; set; } = default!;

    [Inject]
    private IProductCrawlerService _productCrawler { get; set; } = default!;

    [CascadingParameter]
    public WishListItem Item { get; set; } = default!;

    [CascadingParameter]
    public WishListViewModel ViewModel { get; set; } = default!;

    private string? ImageUrl { get; set; }

    private bool _canBePurchased => Item.CanBePurchasedByShare(ViewModel.LoggedInShare?.Id);

    private bool _purchasedByOtherShare => Item.Purchases.Length > 0;

    private int _purchasedByLoggedInShare => Item.GetPurchasedQuantityByShare(ViewModel.LoggedInShare?.Id);

    private ItemEditModalComponent _itemEditModal = default!;

    private WishListSelectionModalComponent _wishListSelectionModal = default!;

    private async Task OpenWishListItemEditModal() => await _itemEditModal.Open(Item);

    private async Task OpenWishListSelectionModal() => await _wishListSelectionModal.Open(Item);

    private async Task BuyItem() => await ViewModel.BuyWishListItem(Item, default);

    private async Task UnbuyItem() => await ViewModel.UnbuyWishListItem(Item, default);

    private async Task ResetItemPurchases() => await ViewModel.ResetWishListItemPurchases(Item, default);

    private async Task SetItemPriority(int priority) => await ViewModel.SetWishListItemPriority(Item, priority, default);

    private async Task DeleteItem()
    {
        bool confirmed = await _jsRuntime.InvokeAsync<bool>("confirm", "Möchten Sie den Eintrag von der Wunschliste entfernen?");
        if (confirmed)
        {
            await ViewModel.DeleteWishListItem(Item, default);
        }
    }
}
