using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishlistApp.Services;

namespace WishlistApp.Components.Pages;

public partial class EditWishlist
{
    [Inject]
    private IWishlistRepository Repository { get; set; }

    [Inject]
    private NavigationManager NavigationManager { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; }

    [Inject]
    private IAccessKeyGenerator AccessKeyGenerator { get; set; }

    [Parameter]
    [SupplyParameterFromQuery(Name = "id")]
    public int? WishlistId { get; set; }

    private Wishlist? Wishlist { get; set; }

    private string NewItemUrl { get; set; } = "";

    private string NewShareName { get; set; } = "";

    private bool IsNewItemUrlEmpty => string.IsNullOrWhiteSpace(NewItemUrl);

    private bool IsNewShareNameEmpty => string.IsNullOrWhiteSpace(NewShareName);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadWishlist(default);
        }
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

        StateHasChanged();
    }

    private async Task RenameWishlist(int id, string name)
    {
        var newWishlist = await Repository.RenameWishlist(id, name, default);

        if (newWishlist is not null)
        {
            Wishlist = newWishlist;
        }
    }

    private async Task AddNewWishlistItem()
    {
        if (string.IsNullOrWhiteSpace(NewItemUrl)
            || Wishlist is null)
        {
            return;
        }

        var newWishlist = await Repository.AddWishlistItem(Wishlist.Id, NewItemUrl, default);

        if (newWishlist is not null)
        {
            Wishlist = newWishlist;
            NewItemUrl = "";
        }
    }

    private async Task DeleteWishlistItem(int itemId)
    {
        if (Wishlist is null)
        {
            return;
        }

        bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Do you want to delete the item from the wishlist?");
        if (confirmed)
        {
            var wishlist = await Repository.DeleteWishlistItem(Wishlist.Id, itemId, default);

            if (wishlist is not null)
            {
                Wishlist = wishlist;
                StateHasChanged();
            }
        }
    }

    private async Task AddNewWishlistShare()
    {
        if (string.IsNullOrWhiteSpace(NewShareName)
            || Wishlist is null)
        {
            return;
        }

        var newWishlist = await Repository.AddWishlistShare(Wishlist.Id, NewShareName, default);

        if (newWishlist is not null)
        {
            Wishlist = newWishlist;
            NewShareName = "";
        }
    }

    private async Task DeleteWishlistShare(int shareId)
    {
        if (Wishlist is null)
        {
            return;
        }

        bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Do you want to delete the share for the wishlist?");
        if (confirmed)
        {
            var wishlist = await Repository.DeleteWishlistShare(Wishlist.Id, shareId, default);

            if (wishlist is not null)
            {
                Wishlist = wishlist;
                StateHasChanged();
            }
        }
    }

    private async Task CopyToClipboard(string content)
    {
        await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", content);
    }
}
