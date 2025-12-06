using System.Diagnostics.CodeAnalysis;
using Shared.Blazor.Dialogs.Models.Results;

namespace WishListApp.Models.Modals.Results;

public static class ModalResultExtensions
{
    extension(ModalResult<SelectWishListResult> result)
    {
        public bool IsSelected() => result.IsT0 && result.AsT0.IsT0;

        public bool TryGetList([NotNullWhen(true)] out WishList? list)
        {
            list = null;

            if (result.IsSelected())
            {
                list = result.AsT0.AsT0.List;
                return true;
            }

            return list is not null;
        }
    }

    extension(ModalResult<EditWishListItemResult> result)
    {
        public bool IsEdited() => result.IsT0 && result.AsT0.IsT0;
    }
}
