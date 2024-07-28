using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishListApp.Components.Controls.Modals;
using WishListApp.Models;

namespace WishListApp.Components.Wishlist;

public partial class WishListComponent : IRecipient<WishListUpdated>
{
    [Inject]
    private IJSRuntime _jsRuntime { get; set; } = default!;

    [Inject]
    private IMessenger _messenger { get; set; } = default!;

    [CascadingParameter]
    public WishListViewModel ViewModel { get; set; } = default!;

    private string _newItemUrl = "";

    private bool _isNewItemUrlEmpty => string.IsNullOrWhiteSpace(_newItemUrl);

    private SharesModalComponent _shareModal = default!;

    private TextInputModalComponent _inputModal = default!;

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
            title: "Eintrag hinzufügen",
            placeholderText: "Produkt Url",
            inputCallback: (string url) => ViewModel.AddWishListItem(url, default));
    }

    private async Task DeleteBoughtWishListItems()
    {
        bool confirmed = await _jsRuntime.InvokeAsync<bool>("confirm", "Möchten Sie alle gekauften Einträge von der Wunschliste entfernen?");
        if (confirmed)
        {
            await ViewModel.DeleteBoughtWishListItems(default);
        }
    }

    private IEnumerable<WishListItem> GetFilteredWishListItems()
    {
        if (ViewModel?.WishList is null || ViewModel.WishList.Items.Length == 0)
        {
            return [];
        }

        var filteredItems = ViewModel.HideBoughtItems
            ? ViewModel.WishList.Items.Where(i => i.RemainingQuantity > 0)
            : ViewModel.WishList.Items;

        filteredItems = ViewModel.HideBuyInformation
            ? filteredItems.OrderByDescending(i => i.Priority) // Don't order by buy-information if owner is viewing
            : filteredItems.OrderBy(i => i.RemainingQuantity > 0
                            ? 0 // 1st: Unbought items
                            : i.GetPurchasedQuantityByShare(ViewModel.LoggedInShare?.Id) > 0
                                ? 1 // 2nd: Items bought by currently viewing share
                                : 2) // 3rd: Items bought by other shares
            .ThenByDescending(i => i.Priority);

        return filteredItems;
    }
}
