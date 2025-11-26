using Shared.Blazor.Dialogs.Models.Contexts;
using Shared.Blazor.Dialogs.Models.Results;
using WishListApp.Components.Modals;

namespace WishListApp.Models.Modals;

public sealed record SharesModalContext(WishListViewModel WishListViewModel) : BaseModalContext<None>
{
    private static readonly Type _modalType = typeof(SharesModal);

    public override Type ModalType => _modalType;
}
