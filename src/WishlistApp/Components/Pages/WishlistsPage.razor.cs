using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishlistApp.Services;

namespace WishlistApp.Components.Pages;

public partial class WishlistsPage
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private IUserService UserService { get; set; } = default!;

    [Inject]
    private IWishlistRepository Repository { get; set; } = default!;

    private WishlistUserDto? _user;

    private List<WishlistDto>? _lists;

    private string _newWishlistName = "";

    private bool _isNewWishlistNameEmpty => string.IsNullOrWhiteSpace(_newWishlistName);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _user = await UserService.GetLoggedInWishlistUser();
            await LoadWishlists(default);
        }
    }

    private async Task LoadWishlists(CancellationToken cancellationToken)
    {
        _lists = null;
        var lists = await Repository.GetAll(default);
        _lists = [.. lists.OrderBy(l => l.Name)];
        StateHasChanged();
    }

    private async Task CreateNewWishlist()
    {
        if (string.IsNullOrWhiteSpace(_newWishlistName)
            || _user is null)
        {
            return;
        }

        var wishlist = await Repository.CreateWishlist(_newWishlistName, _user.Identifier, default);
        NavigationManager.NavigateTo($"editwishlist?id={wishlist.Id}");
    }

    private void OpenWishlistAsync(int id)
    {
        NavigationManager.NavigateTo($"editwishlist?id={id}");
    }

    private async Task DeleteWishlist(int id)
    {
        bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Möchten Sie die Wunschliste löschen?");
        if (confirmed)
        {
            await Repository.DeleteWishlist(id, default);
            _lists = await Repository.GetAll(default);
        }
    }

    private async Task RenameWishlist(int id, string name)
    {
        await Repository.RenameWishlist(id, name, default);
        await LoadWishlists(default);
    }
}
