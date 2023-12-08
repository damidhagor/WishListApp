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

    [Parameter]
    public WishlistShareDto? WishlistShare { get; set; }

    private IEnumerable<WishlistItemDto> _filteredItems =>
        Wishlist is not null
        ? Wishlist.Items
            .Where(i => !_hideBoughtItems || (_hideBoughtItems && i.BoughtByWishlistShareId is null))
            .OrderBy(i => i.BoughtByWishlistShareId is null
                            ? 0
                            : i.BoughtByWishlistShareId is not null && i.BoughtByWishlistShareId == WishlistShare?.Id
                                ? 1
                                : 2)
            .ThenByDescending(i => i.Priority.Priority)
        : [];

    private string _newItemUrl = "";

    private bool _hideBoughtItems = true;

    private bool _isNewItemUrlEmpty => string.IsNullOrWhiteSpace(_newItemUrl);

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
}