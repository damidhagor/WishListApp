var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.WishListApp>("wishlistapp");

builder.Build().Run();
