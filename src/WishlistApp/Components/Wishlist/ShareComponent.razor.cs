using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishlistApp.Data.Services;
using WishlistApp.Services;

namespace WishlistApp.Components.Wishlist;

public partial class ShareComponent
{
    [Inject]
    private IJSRuntime _jsRuntime { get; set; } = default!;

    [Inject]
    private IAccessKeyGenerator _accessKeyGenerator { get; set; } = default!;

    [CascadingParameter]
    public WishlistShare Share { get; set; } = default!;

    [CascadingParameter]
    public WishlistViewModel ViewModel { get; set; } = default!;

    private string Url => _accessKeyGenerator.GenerateWishlistShareUrl(Share.AccessKey);

    private async Task CopyShareUrlToClipboard() => await _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", Url);

    private async Task DeleteShare()
    {
        bool confirmed = await _jsRuntime.InvokeAsync<bool>("confirm", $"Möchten Sie die Freigabe \"{Share.Name}\" löschen?");
        if (confirmed)
        {
            await ViewModel.DeleteWishlistShare(Share, default);
        }
    }
}