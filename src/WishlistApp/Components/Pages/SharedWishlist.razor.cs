using Microsoft.AspNetCore.Components;

namespace WishlistApp.Components.Pages;

public partial class SharedWishlist
{
    [Parameter]
    public string AccessKey { get; set; } = "";
}