using Microsoft.AspNetCore.Components;
using WishListApp.Data.Models;
using WishListApp.Data.Repositories;
using WishListApp.Models;
using WishListApp.Services;

namespace WishListApp.Components.Pages;

public partial class WishListPage : IRecipient<WishListUpdated>
{
    [Inject]
    private IUserService _userService { get; set; } = default!;

    [Inject]
    private IWishlistRepository _wishListRepository { get; set; } = default!;

    [Inject]
    private IWishlistItemRepository _itemRepository { get; set; } = default!;

    [Inject]
    private IWishlistShareRepository _shareRepository { get; set; } = default!;

    [Inject]
    private IWishlistPurchaseRepository _purchaseRepository { get; set; } = default!;

    [Inject]
    private NavigationManager _navigationManager { get; set; } = default!;

    [Inject]
    private IMessenger _messenger { get; set; } = default!;

    [Parameter]
    [SupplyParameterFromQuery(Name = "id")]
    public int? WishListId { get; set; }

    [Parameter]
    public string AccessKey { get; set; } = "";

    private WishListViewModel? _viewModel;


    protected override async Task OnInitializedAsync()
    {
        _messenger.RegisterAll(this);

        var share = !string.IsNullOrWhiteSpace(AccessKey)
            ? await _shareRepository.GetWishListShareByAccessKey(AccessKey, default)
            : null;

        var user = await _userService.GetLoggedInWishListUser();

        await LoadWishListAndValidateAccess(user, share, default);
    }

    private async Task LoadWishListAndValidateAccess(WishListUser? user, WishListShare? share, CancellationToken cancellationToken)
    {
        var wishListId = share?.WishListId ?? WishListId;

        var wishList = wishListId is not null
            ? await _wishListRepository.GetWishList(wishListId.Value, cancellationToken)
            : null;

        if (wishList is null)
        {
            _navigationManager.NavigateTo("/not-found");
            return;
        }

        var validOwner = user is not null
            && wishList.OwnerIdentifier == user.Identifier;

        var validShare = share is not null
            && wishList.Id == share.WishListId;

        if (!validOwner && !validShare)
        {
            _navigationManager.NavigateTo("/");
            return;
        }

        _viewModel = new WishListViewModel(
            _wishListRepository,
            _itemRepository,
            _shareRepository,
            _purchaseRepository,
            _navigationManager,
            _messenger,
            wishList,
            user,
            share);
    }

    public void Receive(WishListUpdated message) => StateHasChanged();
}
