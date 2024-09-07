using Microsoft.AspNetCore.Components;
using WishListApp.Components.Modals;

namespace WishListApp.Components;

public partial class WishListComponent(
    IStringLocalizer<Localization> localizer,
    IMessenger messenger)
    : IRecipient<WishListUpdated>
{
    private readonly IStringLocalizer<Localization> _localizer = localizer;
    private readonly IMessenger _messenger = messenger;

    [CascadingParameter]
    public WishListViewModel ViewModel { get; set; } = default!;

    private SharesModalComponent _shareModal = default!;

    private TextInputModalComponent _inputModal = default!;

    private ConfirmationModalComponent _confirmationModal = default!;

    public void Receive(WishListUpdated message)
    {
        StateHasChanged();
    }

    protected override void OnInitialized()
    {
        _messenger.RegisterAll(this);
    }

    private async Task RenameWishList(string name) => await ViewModel.RenameWishList(name, default);

    private async Task AddNewWishListItem()
    {
        await _inputModal.Open(
            title: _localizer["WishList_Add_Title"],
            placeholderText: _localizer["WishList_Add_Placeholder"],
            inputCallback: (url) => ViewModel.AddWishListItem(url, default));
    }

    private async Task DeletePurchasedWishListItems()
    {
        await _confirmationModal.Open(
            message: _localizer["WishList_DeletePurchasedItems_Message"],
            confirmationCallback: async (confirmed) =>
            {
                if (confirmed)
                {
                    await ViewModel.DeletePurchasedWishListItems(default);
                }
            });
    }

    private IEnumerable<WishListItem> GetFilteredWishListItems()
    {
        if (ViewModel?.WishList is null || ViewModel.WishList.Items.Length == 0)
        {
            return [];
        }

        var filteredItems = ViewModel.HidePurchasedItems
            ? ViewModel.WishList.Items.Where(i => !i.IsPurchased)
            : ViewModel.WishList.Items;

        filteredItems = ViewModel.HidePurchaseDetails
            ? filteredItems.OrderByDescending(i => i.Priority) // Don't order by purchase details if owner is viewing
            : filteredItems.OrderBy(
                i => !i.IsPurchased
                    ? 0 // 1st: Unpurchased items
                    : i.IsPurchasedByShare(ViewModel.LoggedInShare?.Id)
                        ? 1 // 2nd: Items purchased by currently viewing share
                        : 2) // 3rd: Items purchased by other shares
                .ThenByDescending(i => i.Priority);

        return filteredItems;
    }
}
