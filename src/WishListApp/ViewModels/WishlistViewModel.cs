using Microsoft.AspNetCore.Components;
using WishlistApp.Models;

namespace WishlistApp.ViewModels;

public sealed class WishlistViewModel
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly IWishlistItemRepository _itemRepository;
    private readonly IWishlistShareRepository _shareRepository;
    private readonly IWishlistPurchaseRepository _purchaseRepository;
    private readonly NavigationManager _navigationManager;
    private readonly IMessenger _messenger;

    public Wishlist Wishlist { get; private set; } = default!;

    public WishlistUser? LoggedInUser { get; private set; }

    public WishlistShare? LoggedInShare { get; private set; }

    public bool HideBoughtItems { get; set; }

    public bool HideBuyInformation { get; set; }

    public bool ViewedByOwner => LoggedInUser is not null && LoggedInUser.Identifier == Wishlist.OwnerIdentifier;

    public WishlistViewModel(
        IWishlistRepository wishlistRepository,
        IWishlistItemRepository itemRepository,
        IWishlistShareRepository shareRepository,
        IWishlistPurchaseRepository purchaseRepository,
        NavigationManager navigationManager,
        IMessenger messenger,
        Wishlist wishlist,
        WishlistUser? loggedInUser,
        WishlistShare? loggedInShare)
    {
        _wishlistRepository = wishlistRepository;
        _itemRepository = itemRepository;
        _shareRepository = shareRepository;
        _purchaseRepository = purchaseRepository;
        _navigationManager = navigationManager;
        _messenger = messenger;

        Wishlist = wishlist;
        LoggedInUser = loggedInUser;
        LoggedInShare = loggedInShare;

        HideBoughtItems = !ViewedByOwner;
        HideBuyInformation = ViewedByOwner;
    }

    public async Task RenameWishlist(string name, CancellationToken cancellationToken)
    {
        await _wishlistRepository.RenameWishlist(Wishlist.Id, name, cancellationToken);
        await ReloadWishlist(cancellationToken);
    }

    public async Task AddWishlistItem(string url, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return;
        }

        await _itemRepository.AddWishlistItem(Wishlist.Id, url, cancellationToken);
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
        await _itemRepository.DeletePurchasedWishlistItems(Wishlist.Id, cancellationToken);
        await ReloadWishlist(cancellationToken);
    }

    public async Task AddWishlistShare(string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        await _shareRepository.AddWishlistShare(Wishlist.Id, name, cancellationToken);
        await ReloadWishlist(cancellationToken);
    }

    public async Task DeleteWishlistShare(WishlistShare share, CancellationToken cancellationToken)
    {
        await _shareRepository.DeleteWishlistShare(share.Id, cancellationToken);
        await ReloadWishlist(cancellationToken);
    }

    private async Task ReloadWishlist(CancellationToken cancellationToken)
    {
        var wishlist = await _wishlistRepository.GetWishlist(Wishlist.Id, cancellationToken);

        if (wishlist is null)
        {
            _navigationManager.NavigateTo("/not-found");
            return;
        }

        Wishlist = wishlist;
        _messenger.Send(new WishlistUpdated(wishlist));
    }
}
