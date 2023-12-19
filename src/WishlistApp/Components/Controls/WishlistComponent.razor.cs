using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishlistApp.Data.Models;
using WishlistApp.Services;

namespace WishlistApp.Components.Controls;

public partial class WishlistComponent
{
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private IWishlistRepository Repository { get; set; } = default!;

    [Parameter, EditorRequired]
    public WishlistDto? Wishlist { get; set; }

    [Parameter, EditorRequired]
    public WishlistShareDto? WishlistShare { get; set; }

    [Parameter, EditorRequired]
    public WishlistUserDto? WishlistUser { get; set; }

    private string _newItemUrl = "";

    private bool _isNewItemUrlEmpty => string.IsNullOrWhiteSpace(_newItemUrl);

    private bool _viewedByOwner => WishlistUser is not null && WishlistUser.Identifier == Wishlist?.OwnerIdentifier;

    private bool _showBuyInformation => !_viewedByOwner || (_viewedByOwner && !_hideBuyInformation);

    private bool _hideBoughtItems;

    private bool _hideBuyInformation;

    protected override void OnInitialized()
    {
        _hideBoughtItems = !_viewedByOwner;
        _hideBuyInformation = _viewedByOwner;
    }


    private async Task OnItemBought(WishlistItemDto itemDto)
    {
        Wishlist = await Repository.BuyWishlistItem(itemDto.WishlistId, itemDto.Id, WishlistShare!.Id, default);
    }

    private async Task OnItemUnbought(WishlistItemDto itemDto)
    {
        Wishlist = await Repository.UnbuyWishlistItem(itemDto.WishlistId, itemDto.Id, WishlistShare!.Id, default);
    }

    private async Task OnItemPriorityChanged(WishlistItemDto itemDto, WishlistItemPriority priority)
    {
        Wishlist = await Repository.SetWishlistItemPriority(itemDto.WishlistId, itemDto.Id, priority, default);
    }

    private async Task OnItemDeleted(WishlistItemDto itemDto)
    {
        bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Möchten Sie den Eintrag von der Wunschliste entfernen?");
        if (confirmed)
        {
            Wishlist = await Repository.DeleteWishlistItem(itemDto.WishlistId, itemDto.Id, default);
        }
    }

    private async Task RenameWishlist(string name)
    {
        if (Wishlist is null)
        {
            return;
        }

        var newWishlist = await Repository.RenameWishlist(Wishlist.Id, name, default);

        if (newWishlist is not null)
        {
            Wishlist = newWishlist;
        }
    }

    private async Task AddNewWishlistItem()
    {
        if (string.IsNullOrWhiteSpace(_newItemUrl) || Wishlist is null)
        {
            return;
        }

        var newWishlist = await Repository.AddWishlistItem(Wishlist.Id, _newItemUrl, default);

        if (newWishlist is not null)
        {
            Wishlist = newWishlist;
            _newItemUrl = "";
        }
    }

    private async Task DeleteBoughtWishlistItems()
    {
        bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Möchten Sie alle gekauften Einträge von der Wunschliste entfernen?");
        if (!confirmed || Wishlist is null)
        {
            return;
        }

        var wishlistItemIds = Wishlist.Items
            .Where(i => i.BoughtByWishlistShareId is not null)
            .Select(i => i.Id)
            .ToArray();

        Wishlist = await Repository.DeleteWishlistItems(Wishlist.Id, wishlistItemIds, default);
    }

    private IEnumerable<WishlistItemDto> GetFilteredWishlistItems()
    {
        if (Wishlist is null || Wishlist.Items.Length == 0)
        {
            return [];
        }

        var filteredItems = _hideBoughtItems
            ? Wishlist.Items.Where(i => i.BoughtByWishlistShareId is null)
            : Wishlist.Items;

        filteredItems = _hideBuyInformation
            ? filteredItems.OrderByDescending(i => i.Priority.Priority) // Don't order by buy-information if owner is viewing
            : filteredItems.OrderBy(i => i.BoughtByWishlistShareId is null
                            ? 0 // 1st: Unbought items
                            : i.BoughtByWishlistShareId is not null && i.BoughtByWishlistShareId == WishlistShare?.Id
                                ? 1 // 2nd: Items bought by currently viewing share
                                : 2) // 3rd: Items bought by other shares
            .ThenByDescending(i => i.Priority.Priority);

        return filteredItems;
    }
}