using BlazorDialogs.Models.Results;
using BlazorDialogs.Services;
using WishListApp.Models.Modals;
using WishListApp.Models.Modals.Results;

namespace WishListApp.Extensions;

public static class ModalServiceExtensions
{
    public static async Task<ModalResult<None>> ShowShares(this IModalService modalService, WishListViewModel viewModel)
        => await modalService.ShowModal<SharesModalContext, None>(new(viewModel));

    public static async Task<ModalResult<SelectWishListResult>> ShowSelectWishList(this IModalService modalService, WishList[] lists)
        => await modalService.ShowModal<SelectWishListModalContext, SelectWishListResult>(new(lists));

    public static async Task<ModalResult<EditWishListItemResult>> ShowWishListItemEdit(this IModalService modalService, WishListItem item)
        => await modalService.ShowModal<EditWishListItemModalContext, EditWishListItemResult>(new(item));
}
