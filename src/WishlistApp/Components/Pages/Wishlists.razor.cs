using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishlistApp.Data.Models;
using WishlistApp.Services;

namespace WishlistApp.Components.Pages;

public partial class Wishlists
{
    [Inject]
    private IWishlistRepository Repository { get; set; }

    [Inject]
    private NavigationManager NavigationManager { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; }

    private List<WishlistDto>? Lists { get; set; } = null;

    private string NewWishlistName { get; set; } = "";

    private bool IsNewWishlistNameEmpty => string.IsNullOrWhiteSpace(NewWishlistName);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadWishlists(default);
        }
    }

    public async void CreateNewWishlist()
    {
        if (string.IsNullOrWhiteSpace(NewWishlistName))
        {
            return;
        }

        var wishlist = await Repository.CreateWishlist(NewWishlistName, default);
        NavigationManager.NavigateTo($"editwishlist?id={wishlist.Id}");
    }

    public void OpenWishlist(int id)
    {
        NavigationManager.NavigateTo($"editwishlist?id={id}");
    }

    public async Task DeleteWishlist(int id)
    {
        bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Do you want to delete the wishlist?");
        if (confirmed)
        {
            await Repository.DeleteWishlist(id, default);
            Lists = await Repository.GetAll(default);
        }
    }

    public async Task RenameWishlist(int id, string name)
    {
        await Repository.RenameWishlist(id, name, default);
        await LoadWishlists(default);
    }

    private async Task LoadWishlists(CancellationToken cancellationToken)
    {
        Lists = null;
        var lists = await Repository.GetAll(default);
        Lists = [.. lists.OrderBy(l => l.Name)];
        StateHasChanged();
    }
}
