using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace WishlistApp.Components;

public partial class TextInputToggle
{
    private string Value { get; set; }

    private bool IsInEditMode { get; set; }

    private bool IsAcceptingValue { get; set; }

    private bool ShouldFocusInputOnAfterRender { get; set; }

    private ElementReference Input { get; set; }

    [Parameter]
    public string OriginalValue { get; set; }

    [Parameter]
    public EventCallback<string> OnValueAccepted { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (ShouldFocusInputOnAfterRender)
        {
            await Input.FocusAsync();
            ShouldFocusInputOnAfterRender = false;
        }
    }

    private void StartEdit()
    {
        Value = OriginalValue;
        IsInEditMode = true;
        ShouldFocusInputOnAfterRender = true;
    }

    private void CancelEdit()
    {
        IsInEditMode = false;
    }

    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Code is "Enter" or "NumpadEnter")
        {
            IsAcceptingValue = true;
            bool acceptedSuccessfully = true;

            try
            {
                await OnValueAccepted.InvokeAsync(Value);
            }
            catch
            {
                acceptedSuccessfully = false;
            }

            if (acceptedSuccessfully)
            {
                IsInEditMode = false;
            }

            IsAcceptingValue = false;
        }
        else if (e.Code is "Escape")
        {
            CancelEdit();
        }
    }
}