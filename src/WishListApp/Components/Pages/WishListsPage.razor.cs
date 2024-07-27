using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MongoDB.Bson;
using WishListApp.Components.Controls.Modals;
using WishListApp.Services;

namespace WishListApp.Components.Pages;

public partial class WishListsPage
{
    [Inject]
    private NavigationManager _navigationManager { get; set; } = default!;

    [Inject]
    private IJSRuntime _jsRuntime { get; set; } = default!;

    [Inject]
    private IUserService _userService { get; set; } = default!;

    [Inject]
    private IWishListRepository _wishListRepository { get; set; } = default!;

    [Inject]
    private IWishListShareRepository _shareRepository { get; set; } = default!;

    private TextInputModalComponent _inputModal = default!;

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
            title: "Wunschliste hinzufügen",
            placeholderText: "Name",
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
        bool confirmed = await _jsRuntime.InvokeAsync<bool>("confirm", "Möchten Sie die Wunschliste löschen?");
        if (confirmed)
        {
            await _wishListRepository.Delete(wishListId, default);
            await LoadWishLists(default);
        }
    }

    private async Task RenameWishList(ObjectId wishListId, string name)
    {
        await _wishListRepository.Rename(wishListId, name, default);
        await LoadWishLists(default);
    }
}
