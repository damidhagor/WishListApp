using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace WishlistApp.Components.Controls;

public partial class TextInputToggle
{
    private string EditedValue { get; set; }

    private bool IsInEditMode { get; set; }

    private bool IsAcceptingValue { get; set; }

    private bool ShouldFocusInputAfterRender { get; set; }

    private ElementReference Input { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string Value { get; set; }

    [Parameter]
    public EventCallback<string> OnValueAccepted { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (ShouldFocusInputAfterRender)
        {
            await Input.FocusAsync();
            ShouldFocusInputAfterRender = false;
        }
    }

    private void StartEdit()
    {
        EditedValue = Value;
        IsInEditMode = true;
        ShouldFocusInputAfterRender = true;
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
                await OnValueAccepted.InvokeAsync(EditedValue);
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