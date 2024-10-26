using WishListApp.Models.Modals.Results;

namespace WishListApp.Models.Modals;

public sealed class SelectWishListModalContext : ModalContext<SelectWishListResult>
{
    public required WishList[] Lists { get; init; }
}
