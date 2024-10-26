using WishListApp.Components.Modals;
using WishListApp.Models.Modals;
using WishListApp.Models.Modals.Results;

namespace WishListApp.Services;

public interface IModalService
{
    Task<ModalResult<ConfirmationResult>> ShowConfirmation(string message, string? title = null, string? confirmText = null, string? cancelText = null);

    Task<ModalResult<TextInputResult>> ShowTextInput(string? title = null, string? placeholder = null, string? initialText = null, bool inputCanBeEmpty = false, string? confirmText = null);

    Task<ModalResult<None>> ShowShares(WishListViewModel viewModel);

    Task<ModalResult<SelectWishListResult>> ShowSelectWishList(WishList[] lists);

    Task<ModalResult<EditWishListItemResult>> ShowWishListItemEdit(WishListItem item);

    void RegisterModalDisplay(ModalDisplay modalDisplay);

    void UnregisterModalDisplay(ModalDisplay modalDisplay);
}
