namespace WishListApp.AppHost.Keycloak;

internal static class ResourceExtensions
{
    public static IResourceBuilder<IResource> AddKeycloakReference(
        this IResourceBuilder<IResourceWithEnvironment> builder,
        IResourceBuilder<KeycloakResource> keycloak,
        string authorityEnvironmentKey,
        string clientIdEnvironmentKey,
        string clientSecretEnvironmentKey)
    {
        if (!keycloak.Resource.TryGetEndpoints(out var endpoints))
        {
            return builder;
        }

        var endpoint = endpoints.FirstOrDefault(e => e.Name == Constants.HttpEndpointName);
        if (endpoint is null)
        {
            return builder;
        }

        builder.WithEnvironment(authorityEnvironmentKey, $"http://localhost:{endpoint.Port.ToString()}/realms/{keycloak.Resource.Realm}");
        builder.WithEnvironment(clientIdEnvironmentKey, keycloak.Resource.ClientId);
        builder.WithEnvironment(clientSecretEnvironmentKey, keycloak.Resource.ClientSecret);
        return builder;
    }
}
