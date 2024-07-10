using CommunityToolkit.Diagnostics;

namespace WishListApp.AppHost.Keycloak;

internal static class KeycloakResourceBuilderExtensions
{
    public static IResourceBuilder<KeycloakResource> AddKeycloak(
        this IDistributedApplicationBuilder builder,
        string name = "keycloak",
        int port = 8080,
        string adminUsername = "admin",
        string adminPassword = "admin")
    {
        var keycloak = builder.AddResource(new KeycloakResource(name))
            .WithImage("quay.io/keycloak/keycloak", "25.0.1")
            .WithEndpoint(
                port: port,
                targetPort: 8080,
                scheme: "http",
                name: KeycloakResource.HttpEndpointName)
            .WithEnvironment("KEYCLOAK_ADMIN", adminUsername)
            .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", adminPassword)
            .WithBindMount(
                "Keycloak/Resources/wishlist-dev-realm.json",
                "/opt/keycloak/data/import/wishlist-dev-realm.json")
            .WithArgs(
                "start-dev",
                "--import-realm")
            .WithRealm()
            .WithClientId()
            .WithClientName()
            .WithClientSecret()
            .WithDevUserEmail()
            .WithDevUserPassword()
            .WithDevUserFirstName()
            .WithDevUserLastName();

        return keycloak;
    }

    public static IResourceBuilder<KeycloakResource> WithRealm(
        this IResourceBuilder<KeycloakResource> builder,
        string? realm = "wishlist-dev")
    {
        Guard.IsNotNullOrWhiteSpace(realm);
        builder.Resource.Realm = realm;
        builder.WithEnvironment(Constants.RealmEnvironmentKey, realm);
        return builder;
    }

    public static IResourceBuilder<KeycloakResource> WithClientId(
        this IResourceBuilder<KeycloakResource> builder,
        string clientId = "wishlist-app")
    {
        Guard.IsNotNullOrWhiteSpace(clientId);
        builder.Resource.ClientId = clientId;
        builder.WithEnvironment(Constants.ClientIdEnvironmentKey, clientId);
        return builder;
    }

    public static IResourceBuilder<KeycloakResource> WithClientName(
        this IResourceBuilder<KeycloakResource> builder,
        string clientName = "WishlistApp (Development)")
    {
        Guard.IsNotNullOrWhiteSpace(clientName);
        builder.Resource.ClientName = clientName;
        builder.WithEnvironment(Constants.ClientNameEnvironmentKey, clientName);
        return builder;
    }

    public static IResourceBuilder<KeycloakResource> WithClientSecret(
        this IResourceBuilder<KeycloakResource> builder,
        string? clientSecret = null)
    {
        builder.Resource.ClientSecret = clientSecret ?? Guid.NewGuid().ToString();
        builder.WithEnvironment(Constants.ClientSecretEnvironmentKey, clientSecret);
        return builder;
    }

    public static IResourceBuilder<KeycloakResource> WithDevUserEmail(
        this IResourceBuilder<KeycloakResource> builder,
        string email = "dev@dev.de")
    {
        Guard.IsNotNullOrWhiteSpace(email);
        builder.Resource.DevUserEmail = email;
        builder.WithEnvironment(Constants.DevUserEmailEnvironmentKey, email);
        return builder;
    }

    public static IResourceBuilder<KeycloakResource> WithDevUserPassword(
        this IResourceBuilder<KeycloakResource> builder,
        string password = "development")
    {
        Guard.IsNotNullOrWhiteSpace(password);
        builder.Resource.DevUserPassword = password;
        builder.WithEnvironment(Constants.DevUserPasswordEnvironmentKey, password);
        return builder;
    }

    public static IResourceBuilder<KeycloakResource> WithDevUserFirstName(
        this IResourceBuilder<KeycloakResource> builder,
        string firstName = "David")
    {
        Guard.IsNotNullOrWhiteSpace(firstName);
        builder.Resource.DevUserFirstName = firstName;
        builder.WithEnvironment(Constants.DevUserFirstNameEnvironmentKey, firstName);
        return builder;
    }

    public static IResourceBuilder<KeycloakResource> WithDevUserLastName(
        this IResourceBuilder<KeycloakResource> builder,
        string lastName = "Developer")
    {
        Guard.IsNotNullOrWhiteSpace(lastName);
        builder.Resource.DevUserLastName = lastName;
        builder.WithEnvironment(Constants.DevUserLastNameEnvironmentKey, lastName);
        return builder;
    }
}
