using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishlistApp.Components.Controls.Modals;

namespace WishlistApp.Components.Wishlist;

public partial class WishlistComponent : IRecipient<WishlistUpdated>
{
    [Inject]
    private IJSRuntime _jsRuntime { get; set; } = default!;

    [Inject]
    private IMessenger _messenger { get; set; } = default!;

    [Parameter]
    public int? WishlistId { get; set; }

    [CascadingParameter]
    public WishlistViewModel ViewModel { get; set; } = default!;

    private string _newItemUrl = "";

    private bool _isNewItemUrlEmpty => string.IsNullOrWhiteSpace(_newItemUrl);

    private SharesModalComponent _shareModal = default!;

    private TextInputModalComponent _inputModal = default!;

    public void Receive(WishlistUpdated message)
    {
        StateHasChanged();
    }

    protected override void OnInitialized()
    {
        _messenger.RegisterAll(this);
    }

    private async Task RenameWishlist(string name) => await ViewModel.RenameWishlist(name, default);

    private async Task AddNewWishlistItem()
    {
        await _inputModal.Open(
            title: "Eintrag hinzufügen",
            placeholderText: "Produkt Url",
            inputCallback: (string url) => ViewModel.AddWishlistItem(url, default));
    }

    private async Task DeleteBoughtWishlistItems()
    {
        bool confirmed = await _jsRuntime.InvokeAsync<bool>("confirm", "Möchten Sie alle gekauften Einträge von der Wunschliste entfernen?");
        if (confirmed)
        {
            await ViewModel.DeleteBoughtWishlistItems(default);
        }
    }

    private IEnumerable<WishlistItem> GetFilteredWishlistItems()
    {
        if (ViewModel?.Wishlist is null || ViewModel.Wishlist.Items.Length == 0)
        {
            return [];
        }

        var filteredItems = ViewModel.HideBoughtItems
            ? ViewModel.Wishlist.Items.Where(i => i.RemainingQuantity > 0)
            : ViewModel.Wishlist.Items;

        filteredItems = ViewModel.HideBuyInformation
            ? filteredItems.OrderByDescending(i => i.Priority.Priority) // Don't order by buy-information if owner is viewing
            : filteredItems.OrderBy(i => i.RemainingQuantity > 0
                            ? 0 // 1st: Unbought items
                            : i.GetPurchasedQuantityByShare(ViewModel.LoggedInShare) > 0
                                ? 1 // 2nd: Items bought by currently viewing share
                                : 2) // 3rd: Items bought by other shares
            .ThenByDescending(i => i.Priority.Priority);

        return filteredItems;
    }
}