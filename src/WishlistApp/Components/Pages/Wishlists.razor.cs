using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishlistApp.Data.Models;
using WishlistApp.Services;

namespace WishlistApp.Components.Pages;

public partial class Wishlists
{
    [Inject]
    public IWishlistRepository Repository { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    public List<Wishlist> Lists { get; set; } = [];

    private string _newWishlistName = "";
    public string NewWishlistName
    {
        get => _newWishlistName;
        set
        {
            if (_newWishlistName != value)
            {
                _newWishlistName = value;
                StateHasChanged();
            }
        }
    }

    public bool IsNewWishlistNameEmpty => string.IsNullOrWhiteSpace(_newWishlistName);

    protected override async Task OnInitializedAsync()
    {
        Lists = await Repository.GetAll(default);
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
        Lists = [.. Lists.OrderBy(l => l.Name)];
    }
}
