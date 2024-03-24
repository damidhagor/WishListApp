namespace WishlistApp.Components.Controls.Modals;

public partial class TextInputModalComponent
{
    private ModalComponent _modal = default!;

    private string _title = "";
    private string _placeholderText = "";
    private bool _inputCanBeEmpty = false;
    private Func<string, Task>? _inputCallback = null;

    private string _text = "";

    private bool _isOkButtonDisabled => string.IsNullOrWhiteSpace(_text) && !_inputCanBeEmpty;

    public async Task Open(
        string title = "Eingabe",
        string initialText = "",
        string placeholderText = "",
        bool inputCanBeEmpty = false,
        Func<string, Task>? inputCallback = null)
    {
        _title = title;
        _placeholderText = placeholderText;
        _inputCanBeEmpty = inputCanBeEmpty;
        _inputCallback = inputCallback;
        _text = initialText;

        StateHasChanged();

        await _modal.Open();
    }

    private async Task OnOkButtonClicked()
    {
        if (_inputCallback is not null)
        {
            await _inputCallback.Invoke(_text);
        }

        await _modal.Close();
    }
}