using WishListApp.Models.Modals.Results;

namespace WishListApp.Models.Modals;

public sealed class ConfirmationModalContext : ModalContext<ConfirmationResult>
{
    public string? Title { get; init; }

    public required string Message { get; init; }

    public string? ConfirmText { get; init; }

    public string? CancelText { get; init; }
}
