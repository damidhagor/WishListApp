using Microsoft.AspNetCore.Components;
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

    private WishlistDto? Wishlist { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadWishlist(default);
    }

    private async Task LoadWishlist(CancellationToken cancellationToken)
    {
        if (WishlistId is not null)
        {
            Wishlist = await Repository.GetWishlist(WishlistId.Value, default);
            if (Wishlist is null)
            {
                NavigationManager.NavigateTo("/");
                return;
            }
        }
        else
        {
            Wishlist = new();
        }
    }
}
