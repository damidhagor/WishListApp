using Microsoft.JSInterop;
using WishListApp.Components.Modals.Base;
using WishListApp.Models.Modals;
using WishListApp.Models.Modals.Results;
using WishListApp.Services;

namespace WishListApp.Components.Modals;

public sealed partial class EditWishListItemModal(
    IJSRuntime jsRuntime,
    IModalService modalService)
    : BaseModal<EditWishListItemModalContext, EditWishListItemResult>(jsRuntime)
{
    private readonly IModalService _modalService = modalService;
    private ItemEditor _itemEditor = default!;

    private async Task LoadProductInformation()
    {
        try
        {
            await _itemEditor.LoadProductInformation();
        }
        catch (Exception)
        {

        }
    }

    private async Task SaveWishListItem()
    {
        await _itemEditor.SaveWishListItem();

        Context.SetResult(new Edited());
        await Hide();
    }
}
