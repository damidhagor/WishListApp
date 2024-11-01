using BlazorDialogs.Models.Contexts;
using BlazorDialogs.Models.Results;
using WishListApp.Components.Modals;

namespace WishListApp.Models.Modals;

public sealed record SharesModalContext(WishListViewModel WishListViewModel) : BaseModalContext<None>
{
    private static Type _modalType = typeof(SharesModal);

    public override Type ModalType => _modalType;
}
