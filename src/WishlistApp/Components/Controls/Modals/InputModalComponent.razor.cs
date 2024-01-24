namespace WishlistApp.Components.Controls.Modals;

public partial class InputModalComponent
{
    private ModalComponent _modal = default!;

    private string _title = "";
    private string _placeholderText = "";
    private bool _inputCanBeEmpty = false;

    private string _text = "";

    private bool _isOkButtonDisabled => string.IsNullOrWhiteSpace(_text) && !_inputCanBeEmpty;

    public async Task Open(string title = "Eingabe", string initialText = "", string placeholderText = "", bool inputCanBeEmpty = false)
    {
        _title = title;
        _placeholderText = placeholderText;
        _inputCanBeEmpty = inputCanBeEmpty;
        _text = initialText;

        StateHasChanged();

        await _modal.Open();
    }

    private async Task OnOkButtonClicked()
    {
        await _modal.Close();
    }
}