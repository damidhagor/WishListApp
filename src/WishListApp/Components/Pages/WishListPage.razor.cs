using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using WishListApp.Services;

namespace WishListApp.Components.Pages;

public partial class WishListPage : IRecipient<WishListUpdated>
{
    [Inject]
    private IUserService _userService { get; set; } = default!;

    [Inject]
    private IWishListRepository _wishListRepository { get; set; } = default!;

    [Inject]
    private IWishListItemRepository _itemRepository { get; set; } = default!;

    [Inject]
    private IWishListShareRepository _shareRepository { get; set; } = default!;

    [Inject]
    private IAccessKeyGenerator _accessKeyGenerator { get; set; } = default!;

    [Inject]
    private NavigationManager _navigationManager { get; set; } = default!;

    [Inject]
    private IMessenger _messenger { get; set; } = default!;

    [Parameter]
    [SupplyParameterFromQuery(Name = "id")]
    public string? WishListId { get; set; }

    [Parameter]
    public string AccessKey { get; set; } = "";

    private WishListViewModel? _viewModel;


    protected override async Task OnInitializedAsync()
    {
        _messenger.RegisterAll(this);

        var share = !string.IsNullOrWhiteSpace(AccessKey)
            ? await _shareRepository.GetByAccessKey(AccessKey, default)
            : null;

        var user = await _userService.GetLoggedInWishListUser();

        await LoadWishListAndValidateAccess(user, share?.ToModel(), default);
    }

    private async Task LoadWishListAndValidateAccess(WishListUser? user, WishListShare? share, CancellationToken cancellationToken)
    {
        var wishListId = share is not null
            ? share.WishListId
            : ObjectId.TryParse(WishListId, out var parsedWishListId)
                ? parsedWishListId
                : (ObjectId?)null;

        var wishList = wishListId is not null
            ? await _wishListRepository.GetById(wishListId.Value, cancellationToken)
            : null;

        if (wishList is null)
        {
            _navigationManager.NavigateTo("/not-found");
            return;
        }

        var validOwner = user is not null
            && wishList.OwnerId == user.Identifier;

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
            _accessKeyGenerator,
            _navigationManager,
            _messenger,
            wishList.ToModel(),
            user,
            share);
    }

    public void Receive(WishListUpdated message) => StateHasChanged();
}
