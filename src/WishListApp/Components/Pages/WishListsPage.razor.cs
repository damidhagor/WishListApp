using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using WishListApp.Components.Controls.Modals;
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

        var wishLists = await _wishListRepository.GetByOwner(_user.Identifier, cancellationToken);
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
            inputCallback: async (string name) =>
            {
                if (string.IsNullOrWhiteSpace(name)
                    || _user is null)
                {
                    return;
                }

                var wishList = await _wishListRepository.Add(name, _user.Identifier, default);
                _navigationManager.NavigateTo($"editwishlist?id={wishList.Id}");
            });
    }

    private async Task DeleteWishList(ObjectId wishListId)
    {
        await _confirmationModal.Open(
            message: _localizer["WishListsPage_Delete_Message"],
            confirmationCallback: async (bool confirmed) =>
            {
                if (confirmed)
                {
                    await _wishListRepository.Delete(wishListId, default);
                    await LoadWishLists(default);
                }
            });
    }

    private async Task RenameWishList(ObjectId wishListId, string name)
    {
        await _wishListRepository.Rename(wishListId, name, default);
        await LoadWishLists(default);
    }
}
