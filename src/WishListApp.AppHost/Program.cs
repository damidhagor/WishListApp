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

builder.AddKeycloak();

builder.Build().Run();
