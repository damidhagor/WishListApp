namespace WishListApp.Models.Modals;

public sealed class SharesModalContext : ModalContext<None>
{
    public required WishListViewModel WishListViewModel { get; init; }
}
