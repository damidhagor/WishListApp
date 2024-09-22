using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace WishListApp.Tests.Fixtures.MongoDb;

public sealed class MongoDbFixture : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoDbContainer =
        new MongoDbBuilder()
            .WithImage("mongo:7.0.12")
            .Build();

    public IMongoClient GetMongoClient()
    {
        var connectionString = _mongoDbContainer.GetConnectionString();
        return new MongoClient(connectionString);
    }

    public async ValueTask InitializeAsync() => await _mongoDbContainer.StartAsync();

    public async ValueTask DisposeAsync() => await _mongoDbContainer.DisposeAsync();
}
