using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishlistApp.Models;
using WishlistApp.Services;

namespace WishlistApp.Components.Pages;

public partial class WishlistsPage
{
    [Inject]
    private NavigationManager _navigationManager { get; set; } = default!;

    [Inject]
    private IJSRuntime _jsRuntime { get; set; } = default!;

    [Inject]
    private IUserService _userService { get; set; } = default!;

    [Inject]
    private IWishlistRepository _repository { get; set; } = default!;

    private WishlistUser? _user;

    private List<Data.Models.Wishlist>? _wishlists;

    private string _newWishlistName = "";

    private bool _isNewWishlistNameEmpty => string.IsNullOrWhiteSpace(_newWishlistName);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _user = await _userService.GetLoggedInWishlistUser();
            await LoadWishlists(default);
        }
    }

    private async Task LoadWishlists(CancellationToken cancellationToken)
    {
        _wishlists = null;

        if (_user is null)
        {
            return;
        }

        var wishlists = await _repository.GetAll(_user.Identifier, cancellationToken);
        _wishlists = [.. wishlists.OrderBy(l => l.Name)];
        StateHasChanged();
    }

    private async Task CreateNewWishlist()
    {
        if (string.IsNullOrWhiteSpace(_newWishlistName)
            || _user is null)
        {
            return;
        }

        var wishlist = await _repository.CreateWishlist(_newWishlistName, _user.Identifier, default);
        _navigationManager.NavigateTo($"editwishlist?id={wishlist.Id}");
    }

    private async Task DeleteWishlist(int id)
    {
        bool confirmed = await _jsRuntime.InvokeAsync<bool>("confirm", "Möchten Sie die Wunschliste löschen?");
        if (confirmed)
        {
            await _repository.DeleteWishlist(id, default);
            await LoadWishlists(default);
        }
    }

    private async Task RenameWishlist(int id, string name)
    {
        await _repository.RenameWishlist(id, name, default);
        await LoadWishlists(default);
    }
}
