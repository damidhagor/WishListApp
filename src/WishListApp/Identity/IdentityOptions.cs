namespace WishListApp.Identity;

public sealed class IdentityOptions
{
    public string Authority { get; set; } = "";

    public string ClientId { get; set; } = "";

    public string ClientSecret { get; set; } = "";

    public string[] Scopes { get; set; } = [];

    public bool RequireHttpsMetadata { get; set; } = true;
}
