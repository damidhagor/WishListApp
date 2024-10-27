using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using WishListApp.Components.Modals.Base;
using WishListApp.Models.Modals;
using WishListApp.Models.Modals.Results;

namespace WishListApp.Components.Modals;

public sealed partial class TextInputModal(IJSRuntime jsRuntime)
    : BaseModal<TextInputModalContext, TextInputResult>(jsRuntime)
{
    private ElementReference _input = default!;

    private string _text = "";

    private bool _isConfirmButtonDisabled => string.IsNullOrWhiteSpace(_text) && !Context.InputCanBeEmpty;

    private string _confirmText => Context.ConfirmText ?? _localization.OK;

    protected override void OnParametersSet()
    {
        _text = Context.InitialText ?? "";
        base.OnParametersSet();
    }

    public override async Task OnModalShown()
    {
        await _input.FocusAsync();
    }

    private async Task OnConfirm()
    {
        Context.SetResult(new TextInput(_text));
        await Hide();
    }

    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Code is "Enter" or "NumpadEnter")
        {
            if (!_isConfirmButtonDisabled)
            {
                await OnConfirm();
            }
        }
    }
}
