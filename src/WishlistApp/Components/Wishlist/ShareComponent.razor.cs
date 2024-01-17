using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishlistApp.Services;

namespace WishlistApp.Components.Wishlist;

public partial class ShareComponent
{
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private IAccessKeyGenerator AccessKeyGenerator { get; set; } = default!;

    [CascadingParameter]
    public WishlistShareDto Share { get; set; } = default!;

    [CascadingParameter]
    public WishlistViewModel ViewModel { get; set; } = default!;

    private string Url => AccessKeyGenerator.GenerateWishlistShareUrl(Share.AccessKey);

    private async Task CopyShareUrlToClipboard() => await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", Url);

    private async Task DeleteShare()
    {
        bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", $"Möchten Sie die Freigabe \"{Share.Name}\" löschen?");
        if (confirmed)
        {
            await ViewModel.DeleteWishlistShare(Share, default);
        }
    }
}