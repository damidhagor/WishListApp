using BlazorDialogs.Components.Modals.Base;
using BlazorDialogs.Services;
using Microsoft.JSInterop;
using WishListApp.Models.Modals;
using WishListApp.Models.Modals.Results;

namespace WishListApp.Components.Modals;

public sealed partial class EditWishListItemModal(
    IJSRuntime jsRuntime,
    IModalService modalService)
    : BaseModal<EditWishListItemModalContext, EditWishListItemResult>(jsRuntime)
{
    private readonly IModalService _modalService = modalService;
    private ItemEditor _itemEditor = default!;

    private async Task LoadProductInformation() => await _itemEditor.LoadProductInformation();

    private async Task SaveWishListItem()
    {
        await _itemEditor.SaveWishListItem();
        await Close(new Edited());
    }
}
