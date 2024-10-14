using CommunityToolkit.Mvvm.Messaging;
using MongoDB.Bson;
using MongoDB.Driver;
using NSubstitute;
using OneOf.Types;
using WishListApp.Data.Accessors;
using WishListApp.Data.Migration.Migrations;
using WishListApp.Data.Migration.Results;
using WishListApp.Data.Migration.Services;
using WishListApp.Data.Models;

namespace WishListApp.Tests.Data.MigrationExecutor;

public sealed class MigrateBatchTests
{
    [Fact]
    public void MigrateBatch_NoDocuments_Success()
    {
        var collection = Substitute.For<IMongoCollection<BsonDocument>>();
        var messenger = Substitute.For<IMessenger>();

        var migration = Substitute.For<IMigration<WishListShare>>();
        migration.Migrate(Arg.Any<BsonDocument>()).Returns(new Success());

        var executor = new MigrationExecutor<WishListShare>(collection, [migration], messenger);

        var result = MigrationExecutorAccessors<WishListShare>.MigrateBatch(executor, []);

        Assert.True(result.IsT0);
        Assert.Empty(result.AsT0);
    }

    [Fact]
    public void MigrateBatch_OneDocument_Success()
    {
        var collection = Substitute.For<IMongoCollection<BsonDocument>>();
        var messenger = Substitute.For<IMessenger>();

        var migration = Substitute.For<IMigration<WishListShare>>();
        migration.Migrate(Arg.Any<BsonDocument>())
            .Returns(new Success())
            .AndDoes(callInfo => ((BsonDocument)callInfo[0]).SetVersion(1));

        var document = new BsonDocument
        {
            { "_id", ObjectId.GenerateNewId() },
            { "WishListId", ObjectId.GenerateNewId() },
            { "Name", "MyName" },
            { "AccessKey", "MyAccessKey" }
        };

        var executor = new MigrationExecutor<WishListShare>(collection, [migration], messenger);

        var result = MigrationExecutorAccessors<WishListShare>.MigrateBatch(executor, [document]);

        Assert.True(result.IsT0);
        var replaceModel = Assert.Single(result.AsT0);
        Assert.Equal(document["_id"], replaceModel.Replacement["_id"]);
        Assert.Equal(1L, document["Version"].AsInt64);
    }

    [Fact]
    public void MigrateBatch_MultipleDocuments_Success()
    {
        var collection = Substitute.For<IMongoCollection<BsonDocument>>();
        var messenger = Substitute.For<IMessenger>();

        var migration = Substitute.For<IMigration<WishListShare>>();
        migration.Migrate(Arg.Any<BsonDocument>())
            .Returns(new Success())
            .AndDoes(callInfo => ((BsonDocument)callInfo[0]).SetVersion(1));

        BsonDocument[] documents =
            [
                new BsonDocument
                {
                    { "_id", ObjectId.GenerateNewId() },
                    { "WishListId", ObjectId.GenerateNewId() },
                    { "Name", "MyName1" },
                    { "AccessKey", "MyAccessKey" }
                },
                new BsonDocument
                {
                    { "_id", ObjectId.GenerateNewId() },
                    { "WishListId", ObjectId.GenerateNewId() },
                    { "Name", "MyName2" },
                    { "AccessKey", "MyAccessKey" },
                    { "Version", 0L }
                }
            ];

        var executor = new MigrationExecutor<WishListShare>(collection, [migration], messenger);

        var result = MigrationExecutorAccessors<WishListShare>.MigrateBatch(executor, documents);

        Assert.True(result.IsT0);
        Assert.Equal(2, result.AsT0.Count);

        var replaceModel1 = result.AsT0[0];
        Assert.Equal(documents[0]["_id"], replaceModel1.Replacement["_id"]);
        Assert.Equal(1L, documents[0]["Version"].AsInt64);

        var replaceModel2 = result.AsT0[1];
        Assert.Equal(documents[1]["_id"], replaceModel2.Replacement["_id"]);
        Assert.Equal(1L, documents[1]["Version"].AsInt64);
    }

    [Fact]
    public void MigrateBatch_InvalidVersionType()
    {
        var collection = Substitute.For<IMongoCollection<BsonDocument>>();
        var messenger = Substitute.For<IMessenger>();

        var migration = Substitute.For<IMigration<WishListShare>>();
        migration.Migrate(Arg.Any<BsonDocument>())
            .Returns(new Success())
            .AndDoes(callInfo => ((BsonDocument)callInfo[0]).SetVersion(1));

        var document = new BsonDocument
        {
            { "_id", ObjectId.GenerateNewId() },
            { "WishListId", ObjectId.GenerateNewId() },
            { "Name", "MyName" },
            { "AccessKey", "MyAccessKey" },
            { "Version", "0" }
        };

        var executor = new MigrationExecutor<WishListShare>(collection, [migration], messenger);

        var result = MigrationExecutorAccessors<WishListShare>.MigrateBatch(executor, [document]);

        Assert.True(result.IsT1);
        Assert.Equal("'Version' field must be an Int64.", result.AsT1.Error);
    }

    [Fact]
    public void MigrateBatch_MissingId()
    {
        var collection = Substitute.For<IMongoCollection<BsonDocument>>();
        var messenger = Substitute.For<IMessenger>();

        var migration = Substitute.For<IMigration<WishListShare>>();
        migration.Migrate(Arg.Any<BsonDocument>())
            .Returns(new Success())
            .AndDoes(callInfo => ((BsonDocument)callInfo[0]).SetVersion(1));

        var document = new BsonDocument
        {
            { "WishListId", ObjectId.GenerateNewId() },
            { "Name", "MyName" },
            { "AccessKey", "MyAccessKey" }
        };

        var executor = new MigrationExecutor<WishListShare>(collection, [migration], messenger);

        var result = MigrationExecutorAccessors<WishListShare>.MigrateBatch(executor, [document]);

        Assert.True(result.IsT1);
        Assert.Equal("The document is missing an '_id' field.", result.AsT1.Error);
    }

    [Fact]
    public void MigrateBatch_InvalidId()
    {
        var collection = Substitute.For<IMongoCollection<BsonDocument>>();
        var messenger = Substitute.For<IMessenger>();

        var migration = Substitute.For<IMigration<WishListShare>>();
        migration.Migrate(Arg.Any<BsonDocument>())
            .Returns(new Success())
            .AndDoes(callInfo => ((BsonDocument)callInfo[0]).SetVersion(1));

        var document = new BsonDocument
        {
            { "_id", "MyId" },
            { "WishListId", ObjectId.GenerateNewId() },
            { "Name", "MyName" },
            { "AccessKey", "MyAccessKey" }
        };

        var executor = new MigrationExecutor<WishListShare>(collection, [migration], messenger);

        var result = MigrationExecutorAccessors<WishListShare>.MigrateBatch(executor, [document]);

        Assert.True(result.IsT1);
        Assert.Equal("'_id' field must be an ObjectId.", result.AsT1.Error);
    }

    [Fact]
    public void MigrateBatch_SkipTooHighVersion()
    {
        var collection = Substitute.For<IMongoCollection<BsonDocument>>();
        var messenger = Substitute.For<IMessenger>();

        var migration = Substitute.For<IMigration<WishListShare>>();
        migration.Migrate(Arg.Any<BsonDocument>())
            .Returns(new Success())
            .AndDoes(callInfo => ((BsonDocument)callInfo[0]).SetVersion(1));

        BsonDocument[] documents =
            [
                new BsonDocument
                {
                    { "_id", ObjectId.GenerateNewId() },
                    { "WishListId", ObjectId.GenerateNewId() },
                    { "Name", "MyName1" },
                    { "AccessKey", "MyAccessKey" },
                    { "Version", 0L }
                },
                new BsonDocument
                {
                    { "_id", ObjectId.GenerateNewId() },
                    { "WishListId", ObjectId.GenerateNewId() },
                    { "Name", "MyName2" },
                    { "AccessKey", "MyAccessKey" },
                    { "Version", 99L }
                }
            ];

        var executor = new MigrationExecutor<WishListShare>(collection, [migration], messenger);

        var result = MigrationExecutorAccessors<WishListShare>.MigrateBatch(executor, documents);

        Assert.True(result.IsT0);
        var replaceModel = Assert.Single(result.AsT0);
        Assert.Equal(documents[0]["_id"], replaceModel.Replacement["_id"]);
        Assert.Equal(1L, documents[0]["Version"].AsInt64);
    }

    [Fact]
    public void MigrateBatch_InvalidVersionNumber()
    {
        var collection = Substitute.For<IMongoCollection<BsonDocument>>();
        var messenger = Substitute.For<IMessenger>();

        var migration = Substitute.For<IMigration<WishListShare>>();
        migration.Migrate(Arg.Any<BsonDocument>())
            .Returns(new InvalidVersion(1, 0));

        var document = new BsonDocument
        {
            { "_id", ObjectId.GenerateNewId() },
            { "WishListId", ObjectId.GenerateNewId() },
            { "Name", "MyName" },
            { "AccessKey", "MyAccessKey" },
            { "Version", 0L }
        };

        var executor = new MigrationExecutor<WishListShare>(collection, [migration], messenger);

        var result = MigrationExecutorAccessors<WishListShare>.MigrateBatch(executor, [document]);

        Assert.True(result.IsT1);
        Assert.Equal("The document's version 0 does not match the supported version 1.", result.AsT1.Error);
    }

    [Fact]
    public void MigrateBatch_InvalidDocument()
    {
        var collection = Substitute.For<IMongoCollection<BsonDocument>>();
        var messenger = Substitute.For<IMessenger>();

        var migration = Substitute.For<IMigration<WishListShare>>();
        migration.Migrate(Arg.Any<BsonDocument>())
            .Returns(new InvalidDocument("The document is invalid."));

        var document = new BsonDocument
        {
            { "_id", ObjectId.GenerateNewId() },
            { "WishListId", ObjectId.GenerateNewId() },
            { "Name", "MyName" },
            { "AccessKey", "MyAccessKey" },
            { "Version", 0L }
        };

        var executor = new MigrationExecutor<WishListShare>(collection, [migration], messenger);

        var result = MigrationExecutorAccessors<WishListShare>.MigrateBatch(executor, [document]);

        Assert.True(result.IsT1);
        Assert.Equal("The document is invalid.", result.AsT1.Error);
    }
}
