using System.Diagnostics.CodeAnalysis;
using BlazorDialogs.Models.Results;

namespace WishListApp.Models.Modals.Results;

public static class ModalResultExtensions
{
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
