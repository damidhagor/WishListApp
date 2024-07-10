var builder = DistributedApplication.CreateBuilder(args);

var mongoDB = builder
    .AddMongoDB("mongodb")
    .WithDataVolume("mongodb-volume")
    .WithOtlpExporter()
    .WithMongoExpress();

var wishListApp = builder
    .AddProject<Projects.WishListApp>("wishlistapp")
    .WithReference(mongoDB, "MongoDB");

builder.AddContainer("keycloak", "quay.io/keycloak/keycloak", "25.0.1")
    .WithEndpoint(
        port: 8080,
        targetPort: 8080,
        scheme: "http",
        name: "web")
    .WithEndpoint(
        port: 9000,
        targetPort: 9000,
        scheme: "http",
        name: "management")
    .WithEnvironment("KEYCLOAK_ADMIN", "admin")
    .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", "admin")
    .WithBindMount(
        "Resources/wishlist-dev-realm.json",
        "/opt/keycloak/data/import/wishlist-dev-realm.json")
    .WithArgs(
        "start-dev",
        "--import-realm");

builder.Build().Run();
