using Microsoft.AspNetCore.Components;
using WishListApp.Models;

namespace WishListApp.ViewModels;

public sealed class WishListViewModel
{
    private readonly IWishlistRepository _wishListRepository;
    private readonly IWishlistItemRepository _itemRepository;
    private readonly IWishlistShareRepository _shareRepository;
    private readonly IWishlistPurchaseRepository _purchaseRepository;
    private readonly NavigationManager _navigationManager;
    private readonly IMessenger _messenger;

    public WishList WishList { get; private set; } = default!;

    public WishListUser? LoggedInUser { get; private set; }

    public WishlistShare? LoggedInShare { get; private set; }

    public bool HideBoughtItems { get; set; }

    public bool HideBuyInformation { get; set; }

    public bool ViewedByOwner => LoggedInUser is not null && LoggedInUser.Identifier == WishList.OwnerIdentifier;

    public WishListViewModel(
        IWishlistRepository wishlistRepository,
        IWishlistItemRepository itemRepository,
        IWishlistShareRepository shareRepository,
        IWishlistPurchaseRepository purchaseRepository,
        NavigationManager navigationManager,
        IMessenger messenger,
        Wishlist wishlist,
        WishListUser? loggedInUser,
        WishlistShare? loggedInShare)
    {
        _wishListRepository = wishlistRepository;
        _itemRepository = itemRepository;
        _shareRepository = shareRepository;
        _purchaseRepository = purchaseRepository;
        _navigationManager = navigationManager;
        _messenger = messenger;

        WishList = wishlist;
        LoggedInUser = loggedInUser;
        LoggedInShare = loggedInShare;

        HideBoughtItems = !ViewedByOwner;
        HideBuyInformation = ViewedByOwner;
    }

    public async Task RenameWishlist(string name, CancellationToken cancellationToken)
    {
        await _wishListRepository.RenameWishlist(WishList.Id, name, cancellationToken);
        await ReloadWishlist(cancellationToken);
    }

    public async Task AddWishlistItem(string url, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return;
        }

        await _itemRepository.AddWishlistItem(WishList.Id, url, cancellationToken);
        await ReloadWishlist(cancellationToken);
    }

    public async Task UpdateWishlistItem(WishlistItem item, CancellationToken cancellationToken)
    {
        await _itemRepository.UpdateWishlistItem(item, cancellationToken);
        await ReloadWishlist(cancellationToken);
    }

    public async Task BuyWishlistItem(WishlistItem item, CancellationToken cancellationToken)
    {
        if (LoggedInShare is null)
        {
            return;
        }

        await _purchaseRepository.UpdatePurchaseQuantity(item.Id, LoggedInShare.Id, 1, cancellationToken);
        await ReloadWishlist(cancellationToken);
    }

    public async Task UnbuyWishlistItem(WishlistItem item, CancellationToken cancellationToken)
    {
        if (LoggedInShare is null)
        {
            return;
        }

        await _purchaseRepository.UpdatePurchaseQuantity(item.Id, LoggedInShare.Id, -1, cancellationToken);
        await ReloadWishlist(cancellationToken);
    }

    public async Task SetWishlistItemPriority(WishlistItem item, WishlistItemPriority priority, CancellationToken cancellationToken)
    {
        await _itemRepository.SetWishlistItemPriority(item.Id, priority, cancellationToken);
        await ReloadWishlist(cancellationToken);
    }

    public async Task DeleteWishlistItem(WishlistItem item, CancellationToken cancellationToken)
    {
        await _itemRepository.DeleteWishlistItem(item.Id, cancellationToken);
        await ReloadWishlist(cancellationToken);
    }

    public async Task DeleteBoughtWishlistItems(CancellationToken cancellationToken)
    {
        await _itemRepository.DeletePurchasedWishlistItems(WishList.Id, cancellationToken);
        await ReloadWishlist(cancellationToken);
    }

    public async Task AddWishlistShare(string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        await _shareRepository.AddWishlistShare(WishList.Id, name, cancellationToken);
        await ReloadWishlist(cancellationToken);
    }

    public async Task DeleteWishlistShare(WishlistShare share, CancellationToken cancellationToken)
    {
        await _shareRepository.DeleteWishlistShare(share.Id, cancellationToken);
        await ReloadWishlist(cancellationToken);
    }

    private async Task ReloadWishlist(CancellationToken cancellationToken)
    {
        var wishlist = await _wishListRepository.GetWishlist(WishList.Id, cancellationToken);

        if (wishlist is null)
        {
            _navigationManager.NavigateTo("/not-found");
            return;
        }

        WishList = wishlist;
        _messenger.Send(new WishlistUpdated(wishlist));
    }
}
