using BlazorDialogs.Models.Contexts;
using WishListApp.Components.Modals;
using WishListApp.Models.Modals.Results;

namespace WishListApp.Models.Modals;

public sealed record SelectWishListModalContext(WishList[] Lists) : BaseModalContext<SelectWishListResult>
{
    private static Type _modalType = typeof(SelectWishListModal);

    public override Type ModalType => _modalType;
}
