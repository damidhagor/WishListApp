using WishListApp.Models.Modals.Results;

namespace WishListApp.Models.Modals;

public sealed class TextInputModalContext : ModalContext<TextInputResult>
{
    public string? Title { get; init; }

    public string? Placeholder { get; init; }
    
    public string? InitialText { get; init; }

    public bool InputCanBeEmpty { get; init; }

    public string? ConfirmText { get; init; }
}
