using WishListApp.Models.Modals;
using WishListApp.Services;

namespace WishListApp.Components.Modals;

public sealed partial class ModalDisplay(IModalService modalService) : IDisposable
{
    private readonly IModalService _modalService = modalService;
    private readonly List<IModalContext> _modals = [];

    public async Task AddModal(IModalContext modalContext)
    {
        if (!IsModalContextSupported(modalContext))
        {
            return;
        };

        _modals.Add(modalContext);
        await InvokeAsync(StateHasChanged);
    }

    private async Task RemoveModal(IModalContext modalContext)
    {
        if (_modals.Remove(modalContext))
        {
            await InvokeAsync(StateHasChanged);
        }
    }

    protected override void OnParametersSet()
    {
        _modalService.RegisterModalDisplay(this);
    }

    public void Dispose()
    {
        _modalService.UnregisterModalDisplay(this);
    }

    private static bool IsModalContextSupported(IModalContext modalContext)
        => modalContext switch
        {
            ConfirmationModalContext _ => true,
            TextInputModalContext _ => true,
            SharesModalContext _ => true,
            SelectWishListModalContext _ => true,
            EditWishListItemModalContext _ => true,
            ErrorModalContext _ => true,
            _ => false
        };
}
