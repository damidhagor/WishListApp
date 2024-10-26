using WishListApp.Components.Modals;
using WishListApp.Models.Modals;
using WishListApp.Models.Modals.Results;

namespace WishListApp.Services;

public sealed class ModalService : IModalService
{
    private readonly Lock _lock = new();
    private ModalDisplay? _modalDisplay;

    public async Task<ModalResult<ConfirmationResult>> ShowConfirmation(
        string message,
        string? title = null,
        string? confirmText = null,
        string? cancelText = null)
        => await ShowModal(new ConfirmationModalContext
        {
            Title = title,
            Message = message,
            ConfirmText = confirmText,
            CancelText = cancelText
        });

    public async Task<ModalResult<TextInputResult>> ShowTextInput(
        string? title = null,
        string? placeholder = null,
        string? initialText = null,
        bool inputCanBeEmpty = true,
        string? confirmText = null)
        => await ShowModal(new TextInputModalContext
        {
            Title = title,
            Placeholder = placeholder,
            InitialText = initialText,
            InputCanBeEmpty = inputCanBeEmpty,
            ConfirmText = confirmText
        });

    public async Task<ModalResult<None>> ShowShares(WishListViewModel viewModel)
        => await ShowModal(new SharesModalContext
        {
            WishListViewModel = viewModel
        });

    public async Task<ModalResult<SelectWishListResult>> ShowSelectWishList(WishList[] lists)
        => await ShowModal(new SelectWishListModalContext
        {
            Lists = lists
        });

    public async Task<ModalResult<EditWishListItemResult>> ShowWishListItemEdit(WishListItem item)
        => await ShowModal(new EditWishListItemModalContext
        {
            Item = item
        });

    public void RegisterModalDisplay(ModalDisplay modalDisplay)
    {
        lock (_lock)
        {
            if (_modalDisplay is not null)
            {
                throw new InvalidOperationException("A ModalDisplayComponent is already registered.");
            }

            _modalDisplay = modalDisplay;
        }
    }

    public void UnregisterModalDisplay(ModalDisplay modalDisplay)
    {
        lock (_lock)
        {
            if (_modalDisplay is null)
            {
                throw new InvalidOperationException("No ModalDisplayComponent is registered.");
            }

            if (_modalDisplay != modalDisplay)
            {
                throw new InvalidOperationException("The specified ModalDisplayComponent is not registered.");
            }

            _modalDisplay = null;
        }
    }

    private async Task<ModalResult<T>> ShowModal<T>(ModalContext<T> context)
    {
        _lock.Enter();

        try
        {
            if (_modalDisplay is null)
            {
                throw new InvalidOperationException("No ModalDisplayComponent is registered.");
            }

            await _modalDisplay.AddModal(context);
        }
        finally
        {
            _lock.Exit();
        }

        return await context.WaitForResult();
    }
}
