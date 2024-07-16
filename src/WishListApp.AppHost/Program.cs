using WishListApp.AppHost.Keycloak;

var builder = DistributedApplication.CreateBuilder(args);

var mongoDB = builder
    .AddMongoDB("mongodb")
    .WithDataVolume("mongodb-volume")
    .WithOtlpExporter()
    .WithMongoExpress();

var wishListApp = builder
    .AddProject<Projects.WishListApp>("wishlistapp")
    .WithReference(mongoDB, "MongoDB");
    
wishListApp.WithEnvironment("ApplicationUrl", wishListApp.GetEndpoint("http"));

var keycloak = builder.AddKeycloak()
    .WithClientReference(wishListApp);

wishListApp.AddKeycloakReference(
    keycloak,
    "IdentityOptions__Authority",
    "IdentityOptions__ClientId",
    "IdentityOptions__ClientSecret");

builder.Build().Run();
