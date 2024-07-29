using Microsoft.AspNetCore.Components;

namespace WishListApp.Components.Controls;

public partial class DropdownComponent<T>
{
    [Inject]
    private IStringLocalizer<Localization> _localizer { get; set; } = default!;

    [Parameter, EditorRequired]
    public IReadOnlyList<T> Items { get; set; } = [];

    [Parameter]
    public RenderFragment<T>? ItemTemplate { get; set; }

    [Parameter]
    public RenderFragment? EmptySelectionTemplate { get; set; }

    [Parameter]
    public EventCallback<T> SelectedItemChanged { get; set; }

    [Parameter]
    public T? SelectedItem { get; set; }
}
