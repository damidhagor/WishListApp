using Microsoft.AspNetCore.Components;
using WishlistApp.Services;

namespace WishlistApp.Components.Pages;

public partial class WishlistPage : IRecipient<WishlistUpdated>
{
    [Inject]
    private IUserService _userService { get; set; } = default!;

    [Inject]
    private IWishlistRepository _wishlistRepository { get; set; } = default!;

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
    public int? WishlistId { get; set; }

    [Parameter]
    public string AccessKey { get; set; } = "";

    private WishlistViewModel? _viewModel;


    protected override async Task OnInitializedAsync()
    {
        _messenger.RegisterAll(this);

        var share = !string.IsNullOrWhiteSpace(AccessKey)
            ? await _shareRepository.GetWishlistShareByAccessKey(AccessKey, default)
            : null;

        var user = await _userService.GetLoggedInWishlistUser();

        await LoadWishlistAndValidateAccess(user, share, default);
    }

    private async Task LoadWishlistAndValidateAccess(WishlistUserDto? user, WishlistShareDto? share, CancellationToken cancellationToken)
    {
        var wishlistId = share?.WishlistId ?? WishlistId;

        var wishlist = wishlistId is not null
            ? await _wishlistRepository.GetWishlist(wishlistId.Value, cancellationToken)
            : null;

        if (wishlist is null)
        {
            _navigationManager.NavigateTo("/not-found");
            return;
        }

        var validOwner = user is not null
            && wishlist.OwnerIdentifier == user.Identifier;

        var validShare = share is not null
            && wishlist.Id == share.WishlistId;

        if (!validOwner && !validShare)
        {
            _navigationManager.NavigateTo("/");
            return;
        }

        _viewModel = new WishlistViewModel(
            _wishlistRepository,
            _itemRepository,
            _shareRepository,
            _purchaseRepository,
            _navigationManager,
            _messenger,
            wishlist,
            user,
            share);
    }

    public void Receive(WishlistUpdated message) => StateHasChanged();
}