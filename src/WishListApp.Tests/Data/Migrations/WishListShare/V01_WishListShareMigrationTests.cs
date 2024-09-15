using MongoDB.Bson;
using WishListApp.Data.Migration.Migrations.WishListShare;

namespace WishListApp.Tests.Data.Migrations.WishListShare;

public sealed class V01_WishListShareMigrationTests
{
    [Fact]
    public void V00_To_V01_Success()
    {
        var shareId = new ObjectId();
        var wishListId = new ObjectId();

        var document = new BsonDocument
        {
            { "_id", shareId },
            { "WishListId", wishListId },
            { "Name", "MyName" },
            { "AccessKey", "MyAccessKey" }
        };

        var migration = new V01_WishListShareMigration();

        var result = migration.Migrate(document);

        Assert.True(result.IsT0);

        Assert.Equal(shareId, document["_id"].AsObjectId);
        Assert.Equal(wishListId, document["WishListId"].AsObjectId);
        Assert.Equal("MyName", document["Name"].AsString);
        Assert.Equal("MyAccessKey", document["AccessKey"].AsString);
        Assert.Equal(1, document["Version"].AsInt64);
    }

    [Theory]
    [InlineData(1u)]
    [InlineData(2u)]
    public void V00_To_V01_InvalidVersion(uint version)
    {
        var shareId = new ObjectId();
        var wishListId = new ObjectId();

        var document = new BsonDocument
        {
            { "_id", shareId },
            { "WishListId", wishListId },
            { "Name", "MyName" },
            { "AccessKey", "MyAccessKey" },
            { "Version", version }
        };

        var migration = new V01_WishListShareMigration();

        var result = migration.Migrate(document);

        Assert.True(result.IsT1);

        Assert.Equal(shareId, document["_id"].AsObjectId);
        Assert.Equal(wishListId, document["WishListId"].AsObjectId);
        Assert.Equal("MyName", document["Name"].AsString);
        Assert.Equal("MyAccessKey", document["AccessKey"].AsString);
        Assert.Equal(version, document["Version"].AsInt64);
    }

    [Fact]
    public void V00_To_V01_InvalidVersionType()
    {
        var shareId = new ObjectId();
        var wishListId = new ObjectId();

        var document = new BsonDocument
        {
            { "_id", shareId },
            { "WishListId", wishListId },
            { "Name", "MyName" },
            { "AccessKey", "MyAccessKey" },
            { "Version", "0" }
        };

        var migration = new V01_WishListShareMigration();

        var result = migration.Migrate(document);

        Assert.True(result.IsT2);
        Assert.Equal("'Version' field must be an Int64.", result.AsT2.Error);

        Assert.Equal(shareId, document["_id"].AsObjectId);
        Assert.Equal(wishListId, document["WishListId"].AsObjectId);
        Assert.Equal("MyName", document["Name"].AsString);
        Assert.Equal("MyAccessKey", document["AccessKey"].AsString);
        Assert.Equal("0", document["Version"].AsString);
    }
}
