using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishListApp.Components.Controls.Modals;

namespace WishListApp.Components.Wishlist;

public partial class ShareComponent
{
    [Inject]
    private IStringLocalizer<Strings> _localizer { get; set; } = default!;

    [Inject]
    private IJSRuntime _jsRuntime { get; set; } = default!;

    [Inject]
    private IAccessKeyGenerator _accessKeyGenerator { get; set; } = default!;

    [CascadingParameter]
    public WishListShare Share { get; set; } = default!;

    [CascadingParameter]
    public WishListViewModel ViewModel { get; set; } = default!;

    private ConfirmationModalComponent _modal = default!;

    private string Url => _accessKeyGenerator.GenerateShareUrl(Share.AccessKey);

    private async Task CopyShareUrlToClipboard() => await _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", Url);

    private async Task DeleteShare()
    {
        await _modal.Open(
            message: _localizer["ShareComponent_Delete_Message", Share.Name],
            confirmationCallback: async (bool confirmed) =>
            {
                if (confirmed)
                {
                    await ViewModel.DeleteWishListShare(Share, default);
                }
            });
    }
}
