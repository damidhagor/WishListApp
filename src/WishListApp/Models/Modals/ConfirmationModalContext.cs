using WishListApp.Models.Modals.Results;

namespace WishListApp.Models.Modals;

public sealed record ConfirmationModalContext(
    string Message,
    string? Title = null,
    string? ConfirmText = null,
    string? CancelText = null)
    : ModalContext<ConfirmationResult>;
