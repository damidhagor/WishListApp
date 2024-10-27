using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishListApp.Components.Modals.Base;
using WishListApp.Models.Modals;
using WishListApp.Models.Modals.Results;

namespace WishListApp.Components.Modals;

public sealed partial class ConfirmationModal(IJSRuntime jsRuntime)
    : BaseModal<ConfirmationModalContext, ConfirmationResult>(jsRuntime)
{
    private ElementReference _confirmButton = default!;
    private ElementReference _cancelButton = default!;

    private string _confirmText => Context.ConfirmText ?? _localization.ConfirmationModal_DefaultOk;

    private string _cancelText => Context.CancelText ?? _localization.ConfirmationModal_DefaultCancel;

    public override async Task OnModalShown()
    {
        await _confirmButton.FocusAsync();
    }

    private async Task OnConfirm()
    {
        Context.SetResult(new Confirmed());
        await Hide();
    }

    private async Task OnCancel()
    {
        Context.SetResult(new Cancelled());
        await Hide();
    }
}
