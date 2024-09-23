using MongoDB.Bson;
using WishListApp.Data.Migration.Migrations.WishList;

namespace WishListApp.Tests.Data.Migrations.WishList;

public sealed class V01_WishListMigrationTests
{
    [Fact]
    public void V00_To_V01_Migrated()
    {
        var wishListId = new ObjectId();
        var itemId1 = new ObjectId();
        var itemId2 = new ObjectId();
        var shareId = new ObjectId();

        var document = new BsonDocument
        {
            { "_id", wishListId },
            { "OwnerId", "MyOwner" },
            { "Name", "MyName" },
            { "Items",
                new BsonArray([
                    new BsonDocument
                    {
                        { "_id", itemId1 },
                        { "Url", "MyUrl1" },
                        { "ImageUrl", "MyImageUrl1" },
                        { "SiteName", "MySiteName1" },
                        { "Name", "MyName1" },
                        { "Description", "MyDescription1" },
                        { "Price", 1.23 },
                        { "Currency", "€" },
                        { "Note", "MyNote1" },
                        { "Priority", 1 },
                        { "Quantity", 2 },
                        { "Purchases",
                            new BsonArray([
                                new BsonDocument
                                {
                                    { "ShareId", shareId },
                                    { "quantity", 1 }
                                }
                                ])
                        }
                    },
                    new BsonDocument
                    {
                        { "_id", itemId2 },
                        { "Url", "MyUrl2" },
                        { "ImageUrl", "MyImageUrl2" },
                        { "SiteName", "MySiteName2" },
                        { "Name", "MyName2" },
                        { "Description", "MyDescription2" },
                        { "Price", 3.45 },
                        { "Currency", "$" },
                        { "Note", "MyNote2" },
                        { "Priority", 2 },
                        { "Quantity", 3 },
                        { "Purchases", new BsonArray() }
                    }
                ])
            }
        };

        var migration = new V01_WishListMigration();

        var result = migration.Migrate(document);

        Assert.True(result.IsT0);

        Assert.Equal(wishListId, document["_id"].AsObjectId);
        Assert.Equal("MyOwner", document["OwnerId"].AsString);
        Assert.Equal("MyName", document["Name"].AsString);

        Assert.Equal(2, document["Items"].AsBsonArray.Count);

        var item1 = document["Items"].AsBsonArray[0].AsBsonDocument;
        Assert.Equal(itemId1, item1["_id"].AsObjectId);
        Assert.Equal("MyUrl1", item1["Url"].AsString);
        Assert.Equal("MyImageUrl1", item1["ImageUrl"].AsString);
        Assert.Equal("MySiteName1", item1["SiteName"].AsString);
        Assert.Equal("MyName1", item1["Name"].AsString);
        Assert.Equal("MyDescription1", item1["Description"].AsString);
        Assert.Equal(1.23, item1["Price"].AsDouble);
        Assert.Equal("€", item1["Currency"].AsString);
        Assert.Equal("MyNote1", item1["Note"].AsString);
        Assert.Equal(1, item1["Priority"].AsInt32);
        Assert.Equal(shareId, item1["Purchaser"].AsObjectId);
        Assert.False(item1.Contains("Quantity"));
        Assert.False(item1.Contains("Purchases"));

        var item2 = document["Items"].AsBsonArray[1].AsBsonDocument;
        Assert.Equal(itemId2, item2["_id"].AsObjectId);
        Assert.Equal("MyUrl2", item2["Url"].AsString);
        Assert.Equal("MyImageUrl2", item2["ImageUrl"].AsString);
        Assert.Equal("MySiteName2", item2["SiteName"].AsString);
        Assert.Equal("MyName2", item2["Name"].AsString);
        Assert.Equal("MyDescription2", item2["Description"].AsString);
        Assert.Equal(3.45, item2["Price"].AsDouble);
        Assert.Equal("$", item2["Currency"].AsString);
        Assert.Equal("MyNote2", item2["Note"].AsString);
        Assert.Equal(2, item2["Priority"].AsInt32);
        Assert.False(item2.Contains("Purchaser"));
        Assert.False(item2.Contains("Quantity"));
        Assert.False(item2.Contains("Purchases"));

        Assert.Equal(1, document["Version"].AsInt64);
    }

    [Fact]
    public void V00_To_V01_Migrated_Items_NotFound()
    {
        var document = new BsonDocument
        {
            { "_id", new ObjectId() }
        };

        var migration = new V01_WishListMigration();

        var result = migration.Migrate(document);

        Assert.True(result.IsT0);
        Assert.Equal(1, document["Version"].AsInt64);
    }

    [Fact]
    public void V00_To_V01_Migrated_Items_Empty()
    {
        var document = new BsonDocument
        {
            { "_id", new ObjectId() },
            { "Items", new BsonArray() }
        };

        var migration = new V01_WishListMigration();

        var result = migration.Migrate(document);

        Assert.True(result.IsT0);
        Assert.Equal(1, document["Version"].AsInt64);
    }

    [Fact]
    public void V00_To_V01_Migrated_Purchases_NotFound()
    {
        var document = new BsonDocument
        {
            { "_id", new ObjectId() },
            { "Items",
                new BsonArray([
                    new BsonDocument
                    {
                        { "_id", new ObjectId() },
                        { "Url", "MyUrl1" }
                    }
                ])
            }
        };

        var migration = new V01_WishListMigration();

        var result = migration.Migrate(document);

        Assert.True(result.IsT0);
        Assert.Equal(1, document["Version"].AsInt64);
    }

    [Fact]
    public void V00_To_V01_Migrated_Purchases_Empty()
    {
        var document = new BsonDocument
        {
            { "_id", new ObjectId() },
            { "Items",
                new BsonArray([
                    new BsonDocument
                    {
                        { "_id", new ObjectId() },
                        { "Url", "MyUrl1" },
                        { "Purchases", new BsonArray() }
                    }
                ])
            }
        };
        var migration = new V01_WishListMigration();
        var result = migration.Migrate(document);

        Assert.True(result.IsT0);
        Assert.Equal(1, document["Version"].AsInt64);
    }

    [Theory]
    [InlineData(1u)]
    [InlineData(2u)]
    public void V00_To_V01_InvalidSourceVersion(uint version)
    {
        var wishListId = new ObjectId();

        var document = new BsonDocument
        {
            { "_id", wishListId },
            { "OwnerId", "MyOwner" },
            { "Name", "MyName" },
            { "Items", new BsonArray() },
            { "Version", version }
        };

        var migration = new V01_WishListMigration();

        var result = migration.Migrate(document);

        Assert.True(result.IsT1);

        Assert.Equal(wishListId, document["_id"].AsObjectId);
        Assert.Equal("MyOwner", document["OwnerId"].AsString);
        Assert.Equal("MyName", document["Name"].AsString);
        Assert.Empty(document["Items"].AsBsonArray);
        Assert.Equal(version, document["Version"].AsInt64);
    }

    [Fact]
    public void V00_To_V01_InvalidDocument_Version_WrongType()
    {
        var document = new BsonDocument
        {
            { "_id", new ObjectId() },
            { "Version", "0" }
        };

        var migration = new V01_WishListMigration();

        var result = migration.Migrate(document);

        Assert.True(result.IsT2);
        Assert.Equal("'Version' field must be an Int64.", result.AsT2.Error);
    }

    [Fact]
    public void V00_To_V01_InvalidDocument_Items_WrongType()
    {
        var document = new BsonDocument
        {
            { "_id", new ObjectId() },
            { "Items", "MyItems" }
        };

        var migration = new V01_WishListMigration();

        var result = migration.Migrate(document);

        Assert.True(result.IsT2);
        Assert.Equal("'Items' field must be an Array.", result.AsT2.Error);
    }

    [Fact]
    public void V00_To_V01_InvalidDocument_Item_NotBsonDocument()
    {
        var document = new BsonDocument
        {
            { "_id", new ObjectId() },
            { "Items", new BsonArray([ BsonValue.Create("MyItem") ]) }
        };

        var migration = new V01_WishListMigration();

        var result = migration.Migrate(document);

        Assert.True(result.IsT2);
        Assert.Equal("Item must be a BsonDocument.", result.AsT2.Error);
    }

    [Fact]
    public void V00_To_V01_InvalidDocument_Purchases_WrongType()
    {
        var document = new BsonDocument
        {
            { "_id", new ObjectId() },
            { "Items",
                new BsonArray([
                    new BsonDocument
                    {
                        { "_id", new ObjectId() },
                        { "Purchases", "MyPurchases" }
                    }
                ])
            }
        };

        var migration = new V01_WishListMigration();

        var result = migration.Migrate(document);

        Assert.True(result.IsT2);
        Assert.Equal("'Purchases' field must be an Array.", result.AsT2.Error);
    }

    [Fact]
    public void V00_To_V01_InvalidDocument_Purchases_ShareId_NotFound()
    {
        var document = new BsonDocument
        {
            { "_id", new ObjectId() },
            { "Items",
                new BsonArray([
                    new BsonDocument
                    {
                        { "_id", new ObjectId() },
                        { "Purchases",
                            new BsonArray([
                                new BsonDocument
                                {
                                    { "Quantity", 1 }
                                }
                            ])
                        }
                    }
                ])
            }
        };

        var migration = new V01_WishListMigration();

        var result = migration.Migrate(document);

        Assert.True(result.IsT2);
        Assert.Equal("Purchase must have a field 'ShareId'.", result.AsT2.Error);
    }

    [Fact]
    public void V00_To_V01_InvalidDocument_Purchases_ShareId_WrongType()
    {
        var document = new BsonDocument
        {
            { "_id", new ObjectId() },
            { "Items",
                new BsonArray([
                    new BsonDocument
                    {
                        { "_id", new ObjectId() },
                        { "Purchases",
                            new BsonArray([
                                new BsonDocument
                                {
                                    { "ShareId", "MyShareId" }
                                }
                            ])
                        }
                    }
                ])
            }
        };

        var migration = new V01_WishListMigration();

        var result = migration.Migrate(document);

        Assert.True(result.IsT2);
        Assert.Equal("'ShareId' field must be an ObjectId.", result.AsT2.Error);
    }
}
