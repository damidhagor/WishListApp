using Microsoft.AspNetCore.Components;

namespace WishListApp.Components.Controls.Modals;

public partial class ConfirmationModalComponent(IStringLocalizer<Localization> localizer)
{
    private readonly IStringLocalizer<Localization> _localizer = localizer;

    private ModalComponent _modal = default!;
    private ElementReference _okButton = default!;
    private ElementReference _cancelButton = default!;

    private string? _title = null;
    private string? _message = null;
    private string? _okButtonText = null;
    private string? _cancelButtonText = null;
    private Func<bool, Task>? _confirmationCallback = null;

    public async Task Open(
        string? title = null,
        string? message = null,
        string? okButtonText = null,
        string? cancelButtonText = null,
        Func<bool, Task>? confirmationCallback = null)
    {
        _title = title;
        _message = message;
        _okButtonText = okButtonText ?? _localizer["ConfirmationModal_DefaultOk"];
        _cancelButtonText = cancelButtonText ?? _localizer["ConfirmationModal_DefaultCancel"];
        _confirmationCallback = confirmationCallback;

        StateHasChanged();

        await _modal.Open();
        await Task.Delay(500);
        await _okButton.FocusAsync();
    }

    private async Task OnOkButtonClicked() => await CloseDialog(true);

    private async Task OnCancelButtonClicked() => await CloseDialog(false);

    private async Task CloseDialog(bool isConfirmation)
    {
        if (_confirmationCallback is not null)
        {
            await _confirmationCallback.Invoke(isConfirmation);
        }

        try
        {
            await _modal.Close();
        }
        catch { }
    }
}
