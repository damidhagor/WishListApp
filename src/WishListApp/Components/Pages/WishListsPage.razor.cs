using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishListApp.Models;
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
    private IWishlistRepository _repository { get; set; } = default!;

    private WishListUser? _user;

    private List<Data.Models.WishList>? _wishLists;

    private string _newWishListName = "";

    private bool _isNewWishlistNameEmpty => string.IsNullOrWhiteSpace(_newWishListName);

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

        var wishLists = await _repository.GetAll(_user.Identifier, cancellationToken);
        _wishLists = [.. wishLists.OrderBy(l => l.Name)];
        StateHasChanged();
    }

    private async Task CreateNewWishList()
    {
        if (string.IsNullOrWhiteSpace(_newWishListName)
            || _user is null)
        {
            return;
        }

        var wishList = await _repository.CreateWishList(_newWishListName, _user.Identifier, default);
        _navigationManager.NavigateTo($"editwishlist?id={wishList.Id}");
    }

    private async Task DeleteWishlist(int id)
    {
        bool confirmed = await _jsRuntime.InvokeAsync<bool>("confirm", "Möchten Sie die Wunschliste löschen?");
        if (confirmed)
        {
            await _repository.DeleteWishList(id, default);
            await LoadWishLists(default);
        }
    }

    private async Task RenameWishlist(int id, string name)
    {
        await _repository.RenameWishList(id, name, default);
        await LoadWishLists(default);
    }
}
