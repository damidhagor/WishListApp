using Microsoft.JSInterop;
using WishListApp.Components.Modals.Base;
using WishListApp.Models.Modals;
using WishListApp.Models.Modals.Results;
using WishListApp.Services;

namespace WishListApp.Components.Modals;

public partial class EditWishListItemModal(
    IJSRuntime jsRuntime,
    IModalService modalService)
    : BaseModal<EditWishListItemModalContext, EditWishListItemResult>(jsRuntime)
{
    private readonly IModalService _modalService = modalService;
    private ItemEditComponent _itemEditComponent = default!;

    private async Task LoadProductInformation()
    {
        try
        {
            await _itemEditComponent.LoadProductInformation();
        }
        catch (Exception)
        {

        }
    }

    private async Task SaveWishListItem()
    {
        await _itemEditComponent.SaveWishListItem();

        Context.SetResult(new Edited());
        await Hide();
    }
}
