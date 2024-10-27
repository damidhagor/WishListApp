using Microsoft.AspNetCore.Components;

namespace WishListApp.Components.Controls;

public sealed partial class LoadingButton
{
    private bool _isLoading = false;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public Func<Task>? LoadingAction { get; set; }

    private async Task OnClick()
    {
        try
        {
            _isLoading = true;

            if (LoadingAction is not null)
            {
                await LoadingAction();
            }
        }
        finally
        {
            _isLoading = false;
        }
    }
}
