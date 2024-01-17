using Microsoft.AspNetCore.Components;

namespace WishlistApp.ViewModels;

public sealed class WishlistViewModel
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly IWishlistItemRepository _itemRepository;
    private readonly IWishlistShareRepository _shareRepository;
    private readonly NavigationManager _navigationManager;
    private readonly IMessenger _messenger;

    public WishlistDto Wishlist { get; private set; } = default!;

    public WishlistUserDto? LoggedInUser { get; private set; }

    public WishlistShareDto? LoggedInShare { get; private set; }

    public bool HideBoughtItems { get; set; }

    public bool HideBuyInformation { get; set; }

    public bool ViewedByOwner => LoggedInUser is not null && LoggedInUser.Identifier == Wishlist.OwnerIdentifier;

    public WishlistViewModel(
        IWishlistRepository wishlistRepository,
        IWishlistItemRepository itemRepository,
        IWishlistShareRepository shareRepository,
        NavigationManager navigationManager,
        IMessenger messenger,
        WishlistDto wishlist,
        WishlistUserDto? loggedInUser,
        WishlistShareDto? loggedInShare)
    {
        _wishlistRepository = wishlistRepository;
        _itemRepository = itemRepository;
        _shareRepository = shareRepository;
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

    public async Task UpdateWishlistItem(WishlistItemDto item, CancellationToken cancellationToken)
    {
        await _itemRepository.UpdateWishlistItem(item, cancellationToken);
        await ReloadWishlist(cancellationToken);
    }

    public async Task BuyWishlistItem(WishlistItemDto item, CancellationToken cancellationToken)
    {
        if (LoggedInShare is null)
        {
            return;
        }

        await _itemRepository.BuyWishlistItem(item.Id, LoggedInShare.Id, cancellationToken);
        await ReloadWishlist(cancellationToken);
    }

    public async Task UnbuyWishlistItem(WishlistItemDto item, CancellationToken cancellationToken)
    {
        await _itemRepository.UnbuyWishlistItem(item.Id, cancellationToken);
        await ReloadWishlist(cancellationToken);
    }

    public async Task SetWishlistItemPriority(WishlistItemDto item, WishlistItemPriorityDto priority, CancellationToken cancellationToken)
    {
        await _itemRepository.SetWishlistItemPriority(item.Id, priority, cancellationToken);
        await ReloadWishlist(cancellationToken);
    }

    public async Task DeleteWishlistItem(WishlistItemDto item, CancellationToken cancellationToken)
    {
        await _itemRepository.DeleteWishlistItem(item.Id, cancellationToken);
        await ReloadWishlist(cancellationToken);
    }

    public async Task DeleteBoughtWishlistItems(CancellationToken cancellationToken)
    {
        await _itemRepository.DeleteBoughtWishlistItems(Wishlist.Id, cancellationToken);
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

    public async Task DeleteWishlistShare(WishlistShareDto share, CancellationToken cancellationToken)
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
