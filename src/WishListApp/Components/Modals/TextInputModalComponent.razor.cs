using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace WishListApp.Components.Modals;

public partial class TextInputModalComponent(IStringLocalizer<Localization> localizer)
{
    private readonly IStringLocalizer<Localization> _localizer = localizer;

    private ModalComponent _modal = default!;
    private ElementReference _input = default!;

    private string _title = "";
    private string _placeholderText = "";
    private bool _inputCanBeEmpty = false;
    private Func<string, Task>? _inputCallback = null;

    private string _text = "";

    private bool _isOkButtonDisabled => string.IsNullOrWhiteSpace(_text) && !_inputCanBeEmpty;

    public async Task Open(
        string? title = null,
        string? initialText = null,
        string? placeholderText = null,
        bool inputCanBeEmpty = false,
        Func<string, Task>? inputCallback = null)
    {
        _title = title ?? _localizer["InputModal_DefaultTitle"];
        _text = initialText ?? "";
        _placeholderText = placeholderText ?? "";
        _inputCanBeEmpty = inputCanBeEmpty;
        _inputCallback = inputCallback;

        StateHasChanged();

        await _modal.Open();
        await Task.Delay(500);
        await _input.FocusAsync();
    }

    private async Task OnOkButtonClicked()
    {
        if (_inputCallback is not null)
        {
            await _inputCallback.Invoke(_text);
        }

        await _modal.Close();
    }

    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            if (!_isOkButtonDisabled)
            {
                await OnOkButtonClicked();
            }
        }
    }
}
