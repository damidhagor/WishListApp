using Microsoft.AspNetCore.Components;
using WishlistApp.Data.Models;
using WishlistApp.Services;

namespace WishlistApp.Components.Pages;

public partial class EditWishlist
{
    [Inject]
    private IWishlistRepository Repository { get; set; }

    [Inject]
    private NavigationManager NavigationManager { get; set; }

    [Parameter]
    [SupplyParameterFromQuery(Name = "id")]
    public int? WishlistId { get; set; }

    private Wishlist? Wishlist { get; set; }

    private string NewItemUrl { get; set; } = "";

    private bool IsNewItemUrlEmpty => string.IsNullOrWhiteSpace(NewItemUrl);

    protected override async Task OnInitializedAsync()
    {
        if (WishlistId is not null)
        {
            Wishlist = await Repository.GetWishlist(WishlistId.Value, default);
            if (Wishlist is null)
            {
                NavigationManager.NavigateTo("/");
            }
        }
        else
        {
            Wishlist = new();
        }
    }

    private async void AddNewWishlistItem()
    {

    }
}
