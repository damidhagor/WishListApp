using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using Shared.Blazor.Dialogs.Services;
using WishListApp.Services;

namespace WishListApp.Components.Pages;

public sealed partial class WishListsPage(
    NavigationManager navigationManager,
    IModalService modalService,
    IUserService userService,
    IWishListRepository wishListRepository,
    IWishListShareRepository shareRepository)
{
    private readonly NavigationManager _navigationManager = navigationManager;
    private readonly IModalService _modalService = modalService;
    private readonly IUserService _userService = userService;
    private readonly IWishListRepository _wishListRepository = wishListRepository;
    private readonly IWishListShareRepository _shareRepository = shareRepository;

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

        try
        {
            var wishLists = await _wishListRepository.GetByOwnerId(_user.Identifier, cancellationToken);
            var wishListsWithShares = new List<(WishList, WishListShare[])>(wishLists.Count);
            foreach (var wishList in wishLists.OrderBy(w => w.Name))
            {
                var shares = await _shareRepository.GetByWishListId(wishList.Id, cancellationToken);
                wishListsWithShares.Add((wishList.ToModel(), shares.ToModels().ToArray()));
            }

            _wishLists = wishListsWithShares;
        }
        catch (Exception e)
        {
            await _modalService.ShowError(_localization.Error_ListsLoad, exception: e);
            _wishLists = [];
        }

        StateHasChanged();
    }

    private async Task CreateNewWishList()
    {
        var result = await _modalService.ShowTextInput(
            title: _localization.WishListsPage_Add_Title,
            placeholder: _localization.WishListsPage_Add_Placeholder);


        if (!result.TryGetText(out var name)
            || string.IsNullOrWhiteSpace(name)
            || _user is null)
        {
            return;
        }

        try
        {
            var wishList = await _wishListRepository.Add(name, _user.Identifier, default);
            _navigationManager.NavigateTo($"wishlist?id={wishList.Id}");
        }
        catch (Exception e)
        {
            await _modalService.ShowError(_localization.Error_ListAdd, exception: e);
        }
    }

    private async Task RenameWishList(WishList list)
    {
        var result = await _modalService.ShowTextInput(
            title: _localization.WishListsPage_Rename_Title,
            initialText: list.Name,
            inputCanBeEmpty: false);

        if (!result.TryGetText(out var name)
            || string.IsNullOrWhiteSpace(name)
            || _user is null)
        {
            return;
        }

        try
        {
            await _wishListRepository.Rename(list.Id, name, default);
            await LoadWishLists(default);
        }
        catch (Exception e)
        {
            await _modalService.ShowError(_localization.Error_ListRename, exception: e);
        }
    }

    private async Task DeleteWishList(ObjectId wishListId)
    {
        var result = await _modalService.ShowConfirmation(_localization.WishListsPage_Delete_Message);
        if (result.IsConfirmed())
        {
            try
            {
                await _wishListRepository.Delete(wishListId, default);
                await LoadWishLists(default);
            }
            catch (Exception e)
            {
                await _modalService.ShowError(_localization.Error_ListDelete, exception: e);
            }
        }
    }
}
