var builder = DistributedApplication.CreateBuilder(args);

var mongoDB = builder
    .AddMongoDB("mongodb")
    .WithDataVolume("mongodb-volume")
    .WithMongoExpress();

var wishListApp = builder
    .AddProject<Projects.WishListApp>("wishlistapp")
    .WithReference(mongoDB);

builder.Build().Run();
