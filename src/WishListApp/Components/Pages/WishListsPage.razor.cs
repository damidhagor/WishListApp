using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using WishListApp.Components.Modals;
using WishListApp.Services;

namespace WishListApp.Components.Pages;

public partial class WishListsPage(
    NavigationManager navigationManager,
    IStringLocalizer<Localization> localizer,
    IUserService userService,
    IWishListRepository wishListRepository,
    IWishListShareRepository shareRepository)
{
    private readonly NavigationManager _navigationManager = navigationManager;
    private readonly IStringLocalizer<Localization> _localizer = localizer;
    private readonly IUserService _userService = userService;
    private readonly IWishListRepository _wishListRepository = wishListRepository;
    private readonly IWishListShareRepository _shareRepository = shareRepository;

    private TextInputModalComponent _inputModal = default!;
    private ConfirmationModalComponent _confirmationModal = default!;

    private WishListUser? _user;

    private List<(WishList WishList, WishListShare[] Shares)>? _wishLists;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _user = await _userService.GetLoggedInWishListUser();
            await LoadWishLists(default);
        }
    }

    private async Task LoadWishLists(CancellationToken cancellationToken)
    {
        _wishLists = null;

        if (_user is null)
        {
            return;
        }

        var wishLists = await _wishListRepository.GetByOwnerId(_user.Identifier, cancellationToken);
        var wishListsWithShares = new List<(WishList, WishListShare[])>(wishLists.Count);
        foreach (var wishList in wishLists.OrderBy(w => w.Name))
        {
            var shares = await _shareRepository.GetByWishListId(wishList.Id, cancellationToken);
            wishListsWithShares.Add((wishList.ToModel(), shares.ToModels().ToArray()));
        }

        _wishLists = wishListsWithShares;
        StateHasChanged();
    }

    private async Task CreateNewWishList()
    {
        await _inputModal.Open(
            title: _localizer["WishListsPage_Add_Title"],
            placeholderText: _localizer["WishListsPage_Add_Placeholder"],
            inputCallback: async (name) =>
            {
                if (string.IsNullOrWhiteSpace(name)
                    || _user is null)
                {
                    return;
                }

                var wishList = await _wishListRepository.Add(name, _user.Identifier, default);
                _navigationManager.NavigateTo($"wishlist?id={wishList.Id}");
            });
    }

    private async Task RenameWishList(WishList list)
    {
        await _inputModal.Open(
            title: "Rename wish list",
            initialText: list.Name,
            inputCanBeEmpty: false,
            inputCallback: async (name) =>
            {
                if (string.IsNullOrWhiteSpace(name)
                    || _user is null)
                {
                    return;
                }

                var wishList = await _wishListRepository.Rename(list.Id, name, default);
                await LoadWishLists(default);
            });
    }

    private async Task DeleteWishList(ObjectId wishListId)
    {
        await _confirmationModal.Open(
            message: _localizer["WishListsPage_Delete_Message"],
            confirmationCallback: async (confirmed) =>
            {
                if (confirmed)
                {
                    await _wishListRepository.Delete(wishListId, default);
                    await LoadWishLists(default);
                }
            });
    }
}
