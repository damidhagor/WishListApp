namespace WishListApp.AppHost.Keycloak;

internal sealed class KeycloakResource(string name)
    : ContainerResource(name),
    IResourceWithConnectionString,
    IResourceWithEndpoints
{
    public const string HttpEndpointName = "http";

    private EndpointReference? _httpEndpoint;

    public EndpointReference HttpEndpoint => _httpEndpoint ??= new(this, HttpEndpointName);

    public ReferenceExpression ConnectionStringExpression
        => ReferenceExpression.Create(
            $"http://{HttpEndpoint.Property(EndpointProperty.Host)}:{HttpEndpoint.Property(EndpointProperty.Port)}");

    public string? Realm { get; set; }

    public string? ClientId { get; set; }

    public string? ClientName { get; set; }

    public string? ClientSecret { get; set; }
}
