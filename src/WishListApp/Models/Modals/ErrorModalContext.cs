namespace WishListApp.Models.Modals;

public sealed record ErrorModalContext(
    string Message,
    string? Title = null,
    string? Details = null,
    Exception? Exception = null,
    string? ConfirmText = null)
    : ModalContext<None>;
