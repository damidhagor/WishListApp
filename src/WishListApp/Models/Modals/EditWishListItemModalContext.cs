using Shared.Blazor.Dialogs.Models.Contexts;
using WishListApp.Components.Modals;
using WishListApp.Models.Modals.Results;

namespace WishListApp.Models.Modals;

public sealed record EditWishListItemModalContext(WishListItem Item) : BaseModalContext<EditWishListItemResult>
{
    private static readonly Type _modalType = typeof(EditWishListItemModal);

    public override Type ModalType => _modalType;
}
