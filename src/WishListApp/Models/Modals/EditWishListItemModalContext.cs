using WishListApp.Models.Modals.Results;

namespace WishListApp.Models.Modals;

public sealed record EditWishListItemModalContext(WishListItem Item) : ModalContext<EditWishListItemResult>;
