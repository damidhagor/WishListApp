using MongoDB.Bson;
using MongoDB.Driver;
using NSubstitute;
using OneOf.Types;
using WishListApp.Data.Migration.Messages;
using WishListApp.Data.Migration.Migrations;
using WishListApp.Data.Migration.Services;
using WishListApp.Data.Models;
using WishListApp.Tests.Fixtures;
using WishListApp.Tests.Fixtures.MongoDb;

namespace WishListApp.Tests.Data.MigrationExecutor;

[Collection(typeof(MongoDbCollection))]
public sealed class MigrateTests(MongoDbFixture mongoDbFixture)
{
    private readonly IMongoClient _mongoClient = mongoDbFixture.GetMongoClient();
    private readonly string _databaseName = Guid.NewGuid().ToString();
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    [Fact]
    public async Task Migrate_NoBatch()
    {
        var collection = _mongoClient.GetDatabase(_databaseName).GetCollection<BsonDocument>("shares");
        var messenger = new MessengerMock();

        var migration = Substitute.For<IMigration<WishListShare>>();
        migration.Migrate(Arg.Any<BsonDocument>()).Returns(new Success());

        var executor = new MigrationExecutor<WishListShare>(collection, [migration], messenger);

        var result = await executor.Migrate(_cancellationToken);

        Assert.True(result.IsT0);

        Assert.False(executor.Status.IsRunning);
        Assert.Null(executor.Status.Error);
        Assert.Equal(0, executor.Status.DocumentsToMigrate);
        Assert.Equal(0, executor.Status.DocumentsMigrated);

        Assert.Equal(2, messenger.Messages.Count);

        var progress1 = Assert.IsType<MigrationProgress>(messenger.Messages[0]);
        Assert.True(progress1.IsRunning);
        Assert.Equal(0, progress1.DocumentsToMigrate);
        Assert.Equal(0, progress1.DocumentsMigrated);

        var progress2 = Assert.IsType<MigrationProgress>(messenger.Messages[1]);
        Assert.False(progress2.IsRunning);
        Assert.Equal(0, progress2.DocumentsToMigrate);
        Assert.Equal(0, progress2.DocumentsMigrated);
    }

    [Fact]
    public async Task Migrate_OneBatch()
    {
        var collection = _mongoClient.GetDatabase(_databaseName).GetCollection<BsonDocument>("shares");
        var messenger = new MessengerMock();

        var migration = Substitute.For<IMigration<WishListShare>>();
        migration.Migrate(Arg.Any<BsonDocument>())
            .Returns(new Success())
            .AndDoes(callInfo => ((BsonDocument)callInfo[0])["Version"] = 1L);

        for (var i = 0; i < 4; i++)
        {
            await collection.InsertOneAsync(
                new BsonDocument()
                {
                    { "_id", ObjectId.GenerateNewId() },
                    { "Version", 0L },
                    { "Count", i }
                },
                cancellationToken: _cancellationToken);
        }

        var executor = new MigrationExecutor<WishListShare>(collection, [migration], messenger);

        var result = await executor.Migrate(_cancellationToken);

        Assert.True(result.IsT0);

        Assert.False(executor.Status.IsRunning);
        Assert.Null(executor.Status.Error);
        Assert.Equal(4, executor.Status.DocumentsToMigrate);
        Assert.Equal(4, executor.Status.DocumentsMigrated);

        var documents = await collection.Find(Builders<BsonDocument>.Filter.Empty)
            .ToListAsync(_cancellationToken);

        Assert.Equal(4, documents.Count);
        Assert.All(documents, doc => Assert.Equal(1L, doc["Version"].AsInt64));

        Assert.Equal(3, messenger.Messages.Count);

        var progress1 = Assert.IsType<MigrationProgress>(messenger.Messages[0]);
        Assert.True(progress1.IsRunning);
        Assert.Equal(4, progress1.DocumentsToMigrate);
        Assert.Equal(0, progress1.DocumentsMigrated);

        var progress2 = Assert.IsType<MigrationProgress>(messenger.Messages[1]);
        Assert.True(progress2.IsRunning);
        Assert.Equal(4, progress2.DocumentsToMigrate);
        Assert.Equal(4, progress2.DocumentsMigrated);

        var progress3 = Assert.IsType<MigrationProgress>(messenger.Messages[2]);
        Assert.False(progress3.IsRunning);
        Assert.Equal(4, progress3.DocumentsToMigrate);
        Assert.Equal(4, progress3.DocumentsMigrated);
    }

    [Fact]
    public async Task Migrate_MultipleBatches()
    {
        var collection = _mongoClient.GetDatabase(_databaseName).GetCollection<BsonDocument>("shares");
        var messenger = new MessengerMock();

        var migration = Substitute.For<IMigration<WishListShare>>();
        migration.Migrate(Arg.Any<BsonDocument>())
            .Returns(new Success())
            .AndDoes(callInfo => ((BsonDocument)callInfo[0])["Version"] = 1L);

        for (var i = 0; i < 150; i++)
        {
            await collection.InsertOneAsync(
                new BsonDocument()
                {
                    { "_id", ObjectId.GenerateNewId() },
                    { "Version", 0L },
                    { "Count", i }
                },
                cancellationToken: _cancellationToken);
        }

        var executor = new MigrationExecutor<WishListShare>(collection, [migration], messenger);

        var result = await executor.Migrate(_cancellationToken);

        Assert.True(result.IsT0);

        Assert.False(executor.Status.IsRunning);
        Assert.Null(executor.Status.Error);
        Assert.Equal(150, executor.Status.DocumentsToMigrate);
        Assert.Equal(150, executor.Status.DocumentsMigrated);

        var documents = await collection.Find(Builders<BsonDocument>.Filter.Empty)
            .ToListAsync(_cancellationToken);

        Assert.Equal(150, documents.Count);
        Assert.All(documents, doc => Assert.Equal(1L, doc["Version"].AsInt64));

        Assert.Equal(4, messenger.Messages.Count);

        var progress1 = Assert.IsType<MigrationProgress>(messenger.Messages[0]);
        Assert.True(progress1.IsRunning);
        Assert.Equal(150, progress1.DocumentsToMigrate);
        Assert.Equal(0, progress1.DocumentsMigrated);

        var progress2 = Assert.IsType<MigrationProgress>(messenger.Messages[1]);
        Assert.True(progress2.IsRunning);
        Assert.Equal(150, progress2.DocumentsToMigrate);
        Assert.Equal(100, progress2.DocumentsMigrated);

        var progress3 = Assert.IsType<MigrationProgress>(messenger.Messages[2]);
        Assert.True(progress3.IsRunning);
        Assert.Equal(150, progress3.DocumentsToMigrate);
        Assert.Equal(150, progress3.DocumentsMigrated);

        var progress4 = Assert.IsType<MigrationProgress>(messenger.Messages[3]);
        Assert.False(progress4.IsRunning);
        Assert.Equal(150, progress4.DocumentsToMigrate);
        Assert.Equal(150, progress4.DocumentsMigrated);
    }

    [Fact]
    public async Task Migrate_InvalidMigrationVersions()
    {
        var collection = _mongoClient.GetDatabase(_databaseName).GetCollection<BsonDocument>("shares");
        var messenger = new MessengerMock();

        var migration1 = Substitute.For<IMigration<WishListShare>>();
        migration1.SupportedVersion.Returns(0u);
        var migration2 = Substitute.For<IMigration<WishListShare>>();
        migration2.SupportedVersion.Returns(2u);

        var executor = new MigrationExecutor<WishListShare>(collection, [migration1, migration2], messenger);

        var result = await executor.Migrate(_cancellationToken);

        Assert.True(result.IsT1);
        Assert.Equal("Migration versions are invalid. Version 1 is expected but actual version is 2.", result.AsT1.Error);

        Assert.False(executor.Status.IsRunning);
        Assert.Equal("Migration versions are invalid. Version 1 is expected but actual version is 2.", executor.Status.Error);
        Assert.Equal(0, executor.Status.DocumentsToMigrate);
        Assert.Equal(0, executor.Status.DocumentsMigrated);

        Assert.Empty(messenger.Messages);
    }

    [Fact]
    public async Task Migrate_MongoDbException()
    {
        var collection = Substitute.For<IMongoCollection<BsonDocument>>();
        collection.CountDocumentsAsync(
            Arg.Any<FilterDefinition<BsonDocument>>(),
            Arg.Any<CountOptions>(),
            Arg.Any<CancellationToken>())
            .Returns(1L)
            .AndDoes(callInfo => throw new Exception("I failed!"));

        var messenger = new MessengerMock();

        var migration = Substitute.For<IMigration<WishListShare>>();
        migration.Migrate(Arg.Any<BsonDocument>())
            .Returns(new Success())
            .AndDoes(callInfo => ((BsonDocument)callInfo[0])["Version"] = 1L);

        var executor = new MigrationExecutor<WishListShare>(collection, [migration], messenger);

        var result = await executor.Migrate(_cancellationToken);

        Assert.True(result.IsT1);
        Assert.Equal("The migration of the WishListShares collection failed: I failed!", result.AsT1.Error);

        Assert.False(executor.Status.IsRunning);
        Assert.Equal("The migration of the WishListShares collection failed: I failed!", executor.Status.Error);
        Assert.Equal(0, executor.Status.DocumentsToMigrate);
        Assert.Equal(0, executor.Status.DocumentsMigrated);

        Assert.Single(messenger.Messages);

        var progress = Assert.IsType<MigrationProgress>(messenger.Messages[0]);
        Assert.False(progress.IsRunning);
        Assert.Equal(0, progress.DocumentsToMigrate);
        Assert.Equal(0, progress.DocumentsMigrated);
    }

    [Fact]
    public async Task Migrate_InvalidDocument()
    {
        var collection = _mongoClient.GetDatabase(_databaseName).GetCollection<BsonDocument>("shares");
        var messenger = new MessengerMock();

        var migration = Substitute.For<IMigration<WishListShare>>();
        migration.Migrate(Arg.Any<BsonDocument>())
            .Returns(new Success())
            .AndDoes(callInfo => ((BsonDocument)callInfo[0])["Version"] = 1L);

        await collection.InsertOneAsync(
            new BsonDocument()
            {
                { "Version", "0" }
            },
            cancellationToken: _cancellationToken);

        var executor = new MigrationExecutor<WishListShare>(collection, [migration], messenger);

        var result = await executor.Migrate(_cancellationToken);

        Assert.True(result.IsT1);
        Assert.Equal("The migration encountered an invalid document: 'Version' field must be an Int64.", result.AsT1.Error);

        Assert.False(executor.Status.IsRunning);
        Assert.Equal("The migration encountered an invalid document: 'Version' field must be an Int64.", executor.Status.Error);
        Assert.Equal(1, executor.Status.DocumentsToMigrate);
        Assert.Equal(0, executor.Status.DocumentsMigrated);

        var documents = await collection.Find(Builders<BsonDocument>.Filter.Empty)
            .ToListAsync(_cancellationToken);

        var document = Assert.Single(documents);
        Assert.Equal("0", document["Version"].AsString);

        Assert.Equal(2, messenger.Messages.Count);

        var progress1 = Assert.IsType<MigrationProgress>(messenger.Messages[0]);
        Assert.True(progress1.IsRunning);
        Assert.Equal(1, progress1.DocumentsToMigrate);
        Assert.Equal(0, progress1.DocumentsMigrated);

        var progress2 = Assert.IsType<MigrationProgress>(messenger.Messages[1]);
        Assert.False(progress2.IsRunning);
        Assert.Equal(1, progress2.DocumentsToMigrate);
        Assert.Equal(0, progress2.DocumentsMigrated);
    }
}
