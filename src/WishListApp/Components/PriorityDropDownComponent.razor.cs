using Microsoft.AspNetCore.Components;

namespace WishListApp.Components;

public partial class PriorityDropDownComponent(IStringLocalizer<Localization> localizer)
{
    private readonly IStringLocalizer<Localization> _localizer = localizer;

    [Parameter]
    public int SelectedPriority { get; set; } = 0;

    [Parameter]
    public EventCallback<int> SelectedPriorityChanged { get; set; }
}
