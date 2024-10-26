using System.Diagnostics.CodeAnalysis;

namespace WishListApp.Models.Modals.Results;

public static class ModalResultExtensions
{
    public static bool IsConfirmed(this ModalResult<ConfirmationResult> result) => result.IsT0 && result.AsT0.IsT0;

    public static bool IsTextInput(this ModalResult<TextInputResult> result) => result.IsT0 && result.AsT0.IsT0;

    public static bool TryGetText(this ModalResult<TextInputResult> result, [NotNullWhen(true)] out string? text)
    {
        text = null;

        if (result.IsTextInput())
        {
            text = result.AsT0.AsT0.Text;
            return true;
        }

        return text is not null;
    }

    public static bool IsSelected(this ModalResult<SelectWishListResult> result) => result.IsT0 && result.AsT0.IsT0;

    public static bool TryGetList(this ModalResult<SelectWishListResult> result, [NotNullWhen(true)] out WishList? list)
    {
        list = null;

        if (result.IsSelected())
        {
            list = result.AsT0.AsT0.List;
            return true;
        }

        return list is not null;
    }

    public static bool IsEdited(this ModalResult<EditWishListItemResult> result) => result.IsT0 && result.AsT0.IsT0;
}
