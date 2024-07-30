using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace WishListApp.Components.Controls;

public partial class TextEditToggleComponent(IStringLocalizer<Localization> localizer)
{
    private readonly IStringLocalizer<Localization> _localizer = localizer;

    private string _editedValue;

    private bool _isInEditMode;

    private bool _isAcceptingValue;

    private bool _shouldFocusInputAfterRender;

    private ElementReference _input;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string Value { get; set; }

    [Parameter]
    public EventCallback<string> OnValueAccepted { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_shouldFocusInputAfterRender)
        {
            await _input.FocusAsync();
            _shouldFocusInputAfterRender = false;
        }
    }

    private void StartEdit()
    {
        _editedValue = Value;
        _isInEditMode = true;
        _shouldFocusInputAfterRender = true;
    }

    private void CancelEdit()
    {
        _isInEditMode = false;
    }

    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Code is "Enter" or "NumpadEnter")
        {
            _isAcceptingValue = true;
            bool acceptedSuccessfully = true;

            try
            {
                await OnValueAccepted.InvokeAsync(_editedValue);
            }
            catch
            {
                acceptedSuccessfully = false;
            }

            if (acceptedSuccessfully)
            {
                _isInEditMode = false;
            }

            _isAcceptingValue = false;
        }
        else if (e.Code is "Escape")
        {
            CancelEdit();
        }
    }
}
