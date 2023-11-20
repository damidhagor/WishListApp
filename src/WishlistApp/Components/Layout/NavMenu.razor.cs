using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace WishlistApp.Components.Layout;

public partial class NavMenu : IDisposable
{
    private string? _currentUrl;

    [Inject]
    public NavigationManager? NavigationManager { get; set; }

    protected override void OnInitialized()
    {
        if (NavigationManager is not null)
        {
            _currentUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            NavigationManager.LocationChanged += OnLocationChanged;
        }
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        if (NavigationManager is not null)
        {
            _currentUrl = NavigationManager.ToBaseRelativePath(e.Location);
            StateHasChanged();
        }
    }

    public void Dispose()
    {
        if (NavigationManager is not null)
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }
    }
}