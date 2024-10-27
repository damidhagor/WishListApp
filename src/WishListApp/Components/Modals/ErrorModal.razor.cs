using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishListApp.Components.Modals.Base;
using WishListApp.Models.Modals;

namespace WishListApp.Components.Modals;

public sealed partial class ErrorModal(IJSRuntime jsRuntime)
    : BaseModal<ErrorModalContext, None>(jsRuntime)
{
    private ElementReference _confirmButton = default!;

    private string _title => Context.Title ?? "Error";
    private string? _exceptionDetails;
    private bool _exceptionDetailsCopied = false;

    private string _confirmText => Context.ConfirmText ?? _localization.OK;

    public override async Task OnModalShown()
    {
        await _confirmButton.FocusAsync();
    }

    private async Task OnConfirm()
    {
        Context.SetResult(new None());
        await Hide();
    }

    private async Task CopyExceptionDetails()
    {
        await _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", _exceptionDetails);
        _exceptionDetailsCopied = true;
    }

    protected override void OnParametersSet()
    {
        if (Context.Exception is not null)
        {
            List<string> exceptionDetails = [];

            var exception = Context.Exception;
            while (exception is not null)
            {
                exceptionDetails.Add($"{exception.Message}\n\n{exception.StackTrace}");
                exception = exception.InnerException;
            }

            _exceptionDetails = string.Join("\n\n-----------------------\n\n", exceptionDetails);
        }

        base.OnParametersSet();
    }
}
