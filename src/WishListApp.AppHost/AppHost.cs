using WishListApp.AppHost.Keycloak;

var builder = DistributedApplication.CreateBuilder(args);

var mongodb = builder
    .AddMongoDB("wishlist-mongodb")
    .WithDataVolume("wishlist-mongodb-volume")
    .WithOtlpExporter()
    .WithMongoExpress(containerName: "wishlist-mongodb-express");

var wishListApp = builder
    .AddProject<Projects.WishListApp>("wishlistapp")
    .WithEnvironment(ctx => ctx.EnvironmentVariables["ApplicationUrl"] = ((IResourceWithEndpoints)ctx.Resource).GetEndpoint("http"))
    .WithReference(mongodb, "mongodb")
    .WaitFor(mongodb);

var keycloak = builder.AddKeycloak(port: 8888)
    .WithClientReference(wishListApp);

wishListApp.AddKeycloakReference(
    keycloak,
    "IdentityOptions__Authority",
    "IdentityOptions__ClientId",
    "IdentityOptions__ClientSecret");

builder.Build().Run();
