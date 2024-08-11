using Microsoft.AspNetCore.Components;
using WishListApp.Components.Modals;

namespace WishListApp.Components;

public partial class ItemComponent(
    IStringLocalizer<Localization> localizer,
    NavigationManager navigationManager)
{
    private readonly IStringLocalizer<Localization> _localizer = localizer;
    private readonly NavigationManager _navigationManager = navigationManager;

    [CascadingParameter]
    public WishListItem Item { get; set; } = default!;

    [CascadingParameter]
    public WishListViewModel ViewModel { get; set; } = default!;

    private bool _canBePurchased => Item.CanBePurchasedByShare(ViewModel.LoggedInShare?.Id);

    private bool _purchasedByOtherShare => Item.Purchases.Length > 0;

    private int _purchasedByLoggedInShare => Item.GetPurchasedQuantityByShare(ViewModel.LoggedInShare?.Id);

    private ItemEditModalComponent _itemEditModal = default!;

    private WishListSelectionModalComponent _wishListSelectionModal = default!;

    private ConfirmationModalComponent _confirmationModal = default!;

    private async Task OpenWishListItemEditModal() => await _itemEditModal.Open(Item);

    private void EditWishListItem() => _navigationManager.NavigateTo($"/edititem?id={Item.Id}");

    private async Task OpenWishListSelectionModal() => await _wishListSelectionModal.Open(Item);

    private async Task MarkItemAsPurchased() => await ViewModel.MarkWishListItemAsPurchased(Item, default);

    private async Task ReversePurchase() => await ViewModel.RevertWishListItemPurchase(Item, default);

    private async Task ResetItemPurchases() => await ViewModel.ResetWishListItemPurchases(Item, default);

    private async Task SetItemPriority(int priority) => await ViewModel.SetWishListItemPriority(Item, priority, default);

    private async Task DeleteItem()
    {
        await _confirmationModal.Open(
            message: _localizer["Item_Delete_Message"],
            confirmationCallback: async (confirmed) =>
            {
                if (confirmed)
                {
                    await ViewModel.DeleteWishListItem(Item, default);
                }
            });
    }
}
