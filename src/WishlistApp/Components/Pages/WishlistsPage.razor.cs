using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using WishlistApp.Services;

namespace WishlistApp.Components.Pages;

public partial class WishlistsPage
{
    [CascadingParameter]
    public Task<AuthenticationState>? AuthenticationStateTask { get; set; }

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private IWishlistRepository Repository { get; set; } = default!;

    private List<WishlistDto>? _lists;

    private string _newWishlistName = "";

    private bool _isNewWishlistNameEmpty => string.IsNullOrWhiteSpace(_newWishlistName);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
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
            || AuthenticationStateTask is null)
        {
            return;
        }

        var authenticationState = await AuthenticationStateTask;

        var identifier = authenticationState.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
            ?.Value;
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return;
        }

        var wishlist = await Repository.CreateWishlist(_newWishlistName, identifier, default);
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
