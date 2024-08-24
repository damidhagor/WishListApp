using Microsoft.AspNetCore.Components;

namespace WishListApp.Components;
public partial class PriorityComponent
{
    private int _hoveredPriority = -1;
    private int _displayedPriority => _hoveredPriority > -1 ? _hoveredPriority : Priority;

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public int Priority { get; set; }

    [Parameter]
    public EventCallback<int> PriorityChanged { get; set; }

    [Parameter]
    public bool IsReadonly { get; set; }

    private string GetCursorStyle() => IsReadonly ? "" : "pointer";

    private string GetIconClass(int priority) => priority <= _displayedPriority
        ? "bi-suit-heart-fill text-primary"
        : "bi-suit-heart";

    private void OnMouseOver(int priority) => _hoveredPriority = IsReadonly ? -1 : priority;

    private void OnMouseOut() => _hoveredPriority = -1;

    private async Task OnCLick(int priority) => await PriorityChanged.InvokeAsync(priority);
}
