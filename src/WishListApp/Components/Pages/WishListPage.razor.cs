using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using WishListApp.Services;

namespace WishListApp.Components.Pages;

public partial class WishListPage(
    NavigationManager navigationManager,
    IMessenger messenger,
    ILocalizationService_Localization localizer,
    IUserService userService,
    IWishListRepository wishListRepository,
    IWishListItemRepository itemRepository,
    IWishListShareRepository shareRepository,
    IAccessKeyGenerator accessKeyGenerator)
    : IRecipient<WishListUpdated>
{
    private readonly NavigationManager _navigationManager = navigationManager;
    private readonly IMessenger _messenger = messenger;
    private readonly ILocalizationService_Localization _localizer = localizer;
    private readonly IUserService _userService = userService;
    private readonly IWishListRepository _wishListRepository = wishListRepository;
    private readonly IWishListItemRepository _itemRepository = itemRepository;
    private readonly IWishListShareRepository _shareRepository = shareRepository;
    private readonly IAccessKeyGenerator _accessKeyGenerator = accessKeyGenerator;

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
