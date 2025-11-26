using Shared.Blazor.Dialogs.Models.Results;
using Shared.Blazor.Dialogs.Services;
using WishListApp.Models.Modals;
using WishListApp.Models.Modals.Results;

namespace WishListApp.Extensions;

public static class ModalServiceExtensions
{
    extension(IModalService modalService)
    {
        public async Task<ModalResult<None>> ShowShares(WishListViewModel viewModel)
        => await modalService.ShowModal<SharesModalContext, None>(new(viewModel));

        public async Task<ModalResult<SelectWishListResult>> ShowSelectWishList(WishList[] lists)
            => await modalService.ShowModal<SelectWishListModalContext, SelectWishListResult>(new(lists));

        public async Task<ModalResult<EditWishListItemResult>> ShowWishListItemEdit(WishListItem item)
            => await modalService.ShowModal<EditWishListItemModalContext, EditWishListItemResult>(new(item));
    }
}
