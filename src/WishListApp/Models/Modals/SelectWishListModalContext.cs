using WishListApp.Models.Modals.Results;

namespace WishListApp.Models.Modals;

public sealed record SelectWishListModalContext(WishList[] Lists) : ModalContext<SelectWishListResult>;
