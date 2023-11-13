using Microsoft.AspNetCore.Components;
using WishlistApp.Data.Models;
using WishlistApp.Services;

namespace WishlistApp.Components.Pages;

public partial class EditWishlist
{
    [Inject]
    public IWishlistRepository Repository { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter]
    [SupplyParameterFromQuery(Name = "id")]
    public int? WishlistId { get; set; }

    public Wishlist? Wishlist { get; set; }

    public bool IsNewWishlist => WishlistId is null;

    public string Title => IsNewWishlist ? $"Create New Wishlist" : $"Edit {Wishlist?.Name ?? "Wishlist"}";

    protected override async Task OnInitializedAsync()
    {
        if (WishlistId is not null)
        {
            Wishlist = await Repository.GetWishlist(WishlistId.Value, default);
            if (Wishlist is null)
            {
                NavigationManager.NavigateTo("editwishlist");
            }
        }

        Wishlist = new();
    }

    public async Task SaveWishlist(CancellationToken cancellationToken = default)
    {

    }

    public async Task AddWishlistItem() { }
}
