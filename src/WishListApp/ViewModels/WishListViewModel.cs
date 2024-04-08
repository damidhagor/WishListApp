using Microsoft.AspNetCore.Components;
using WishListApp.Models;

namespace WishListApp.ViewModels;

public sealed class WishListViewModel
{
    private readonly IWishListRepository _wishListRepository;
    private readonly IWishListItemRepository _itemRepository;
    private readonly IWishListShareRepository _shareRepository;
    private readonly IAccessKeyGenerator _accessKeyGenerator;
    private readonly NavigationManager _navigationManager;
    private readonly IMessenger _messenger;

    public WishList WishList { get; private set; } = default!;

    public WishListUser? LoggedInUser { get; private set; }

    public WishListShare? LoggedInShare { get; private set; }

    public bool HideBoughtItems { get; set; }

    public bool HideBuyInformation { get; set; }

    public bool ViewedByOwner => LoggedInUser is not null && LoggedInUser.Identifier == WishList.OwnerId;

    public WishListViewModel(
        IWishListRepository wishListRepository,
        IWishListItemRepository itemRepository,
        IWishListShareRepository shareRepository,
        IAccessKeyGenerator accessKeyGenerator,
        NavigationManager navigationManager,
        IMessenger messenger,
        WishList wishList,
        WishListUser? loggedInUser,
        WishListShare? loggedInShare)
    {
        _wishListRepository = wishListRepository;
        _itemRepository = itemRepository;
        _shareRepository = shareRepository;
        _accessKeyGenerator = accessKeyGenerator;
        _navigationManager = navigationManager;
        _messenger = messenger;

        WishList = wishList;
        LoggedInUser = loggedInUser;
        LoggedInShare = loggedInShare;

        HideBoughtItems = !ViewedByOwner;
        HideBuyInformation = ViewedByOwner;
    }

    public async Task RenameWishList(string name, CancellationToken cancellationToken)
    {
        await _wishListRepository.Rename(WishList.Id, name, cancellationToken);
        await ReloadWishList(cancellationToken);
    }

    public async Task AddWishListItem(string url, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return;
        }

        await _itemRepository.Add(WishList.Id, url, cancellationToken);
        await ReloadWishList(cancellationToken);
    }

    public async Task UpdateWishListItem(WishListItem item, CancellationToken cancellationToken)
    {
        await _itemRepository.Update(WishList.Id, item, cancellationToken);
        await ReloadWishList(cancellationToken);
    }

    public async Task BuyWishListItem(WishListItem item, CancellationToken cancellationToken)
    {
        if (LoggedInShare is null)
        {
            return;
        }

        await _itemRepository.UpdatePurchaseQuantity(WishList.Id, item.Id, LoggedInShare.Id, 1, cancellationToken);
        await ReloadWishList(cancellationToken);
    }

    public async Task UnbuyWishListItem(WishListItem item, CancellationToken cancellationToken)
    {
        if (LoggedInShare is null)
        {
            return;
        }

        await _itemRepository.UpdatePurchaseQuantity(WishList.Id, item.Id, LoggedInShare.Id, 0, cancellationToken);
        await ReloadWishList(cancellationToken);
    }

    public async Task SetWishListItemPriority(WishListItem item, int priority, CancellationToken cancellationToken)
    {
        await _itemRepository.UpdatePriority(WishList.Id, item.Id, priority, cancellationToken);
        await ReloadWishList(cancellationToken);
    }

    public async Task DeleteWishListItem(WishListItem item, CancellationToken cancellationToken)
    {
        await _itemRepository.Delete(WishList.Id, item.Id, cancellationToken);
        await ReloadWishList(cancellationToken);
    }

    public async Task DeleteBoughtWishListItems(CancellationToken cancellationToken)
    {
        await _itemRepository.DeletePurchasedItems(WishList.Id, cancellationToken);
        await ReloadWishList(cancellationToken);
    }

    public async Task AddWishListShare(string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        var accessKey = _accessKeyGenerator.GenerateAccessKey(16);
        await _shareRepository.Add(WishList.Id, name, accessKey, cancellationToken);
        await ReloadWishList(cancellationToken);
    }

    public async Task DeleteWishListShare(WishListShare share, CancellationToken cancellationToken)
    {
        await _shareRepository.Delete(share.Id, cancellationToken);
        await ReloadWishList(cancellationToken);
    }

    private async Task ReloadWishList(CancellationToken cancellationToken)
    {
        var wishList = await _wishListRepository.GetById(WishList.Id, cancellationToken);

        if (wishList is null)
        {
            _navigationManager.NavigateTo("/not-found");
            return;
        }

        WishList = wishList;
        _messenger.Send(new WishListUpdated(wishList));
    }
}
