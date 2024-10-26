using WishListApp.Models.Modals.Results;

namespace WishListApp.Models.Modals;

public sealed class EditWishListItemModalContext : ModalContext<EditWishListItemResult>
{
    public required WishListItem Item { get; init; }
}
