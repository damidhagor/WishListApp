using MongoDB.Bson;
using WishListApp.Data.Migration.Migrations;
using WishListApp.Data.Migration.Migrations.WishList;

namespace WishListApp.Tests.Data.Migrations.WishList;

public sealed class WishListMigrationChainTests
{
    [Fact]
    public void MigrationChain_Success()
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

        var migrations = new IMigration<WishListApp.Data.Models.WishList>[]
        {
            new V01_WishListMigration()
        };

        foreach (var migration in migrations)
        {
            var result = migration.Migrate(document);
            Assert.True(result.IsT0);
        }

        Assert.Equal(shareId, document["_id"].AsObjectId);
        Assert.Equal(wishListId, document["WishListId"].AsObjectId);
        Assert.Equal("MyName", document["Name"].AsString);
        Assert.Equal("MyAccessKey", document["AccessKey"].AsString);
        Assert.Equal(1, document["Version"].AsInt64);
    }
}
