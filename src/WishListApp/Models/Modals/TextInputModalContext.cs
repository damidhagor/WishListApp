using WishListApp.Models.Modals.Results;

namespace WishListApp.Models.Modals;

public sealed record TextInputModalContext(
    string? Title = null,
    string? Placeholder = null,
    string? InitialText = null,
    bool InputCanBeEmpty = false,
    string? ConfirmText = null)
    : ModalContext<TextInputResult>;
