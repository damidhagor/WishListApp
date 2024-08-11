using MongoDB.Bson;
using MongoDB.Driver;
using WishListApp.Data.Repositories;
using WishListApp.Tests.Fixtures.MongoDb;

namespace WishListApp.Tests.Data;

[Collection("MongoDb")]
public sealed class WishListRepositoryTests(MongoDbFixture mongoDbFixture)
{
    private readonly IMongoClient _mongoClient = mongoDbFixture.GetMongoClient();
    private string _databaseName = Guid.NewGuid().ToString();

    [Fact]
    public async Task Add_New()
    {
        var repository = new WishListRepository(_mongoClient, _databaseName);
        var name = "Name";
        var ownerId = "OwnerId";

        var list = await repository.Add(name, ownerId, default);

        Assert.NotEqual(ObjectId.Empty, list.Id);
        Assert.Equal(name, list.Name);
        Assert.Equal(ownerId, list.OwnerId);
        Assert.Empty(list.Items);
    }

    [Fact]
    public async Task Add_AllowDuplicate()
    {
        var repository = new WishListRepository(_mongoClient, _databaseName);
        var name = "Name";
        var ownerId = "OwnerId";

        var list1 = await repository.Add(name, ownerId, default);
        var list2 = await repository.Add(name, ownerId, default);

        Assert.NotEqual(list1.Id, list2.Id);
    }

    [Fact]
    public async Task GetById_Found()
    {
        var repository = new WishListRepository(_mongoClient, _databaseName);

        var name1 = "Name1";
        var name2 = "Name2";

        var list1 = await repository.Add(name1, "OwnerId", default);
        var list2 = await repository.Add(name2, "OwnerId", default);

        var foundList = await repository.GetById(list1.Id, default);

        Assert.NotNull(foundList);
        Assert.Equal(list1.Id, foundList.Id);
        Assert.Equal(name1, foundList.Name);
    }

    [Fact]
    public async Task GetById_NotFound()
    {
        var repository = new WishListRepository(_mongoClient, _databaseName);

        var list = await repository.Add("Name", "OwnerId", default);

        var foundList = await repository.GetById(ObjectId.GenerateNewId(), default);

        Assert.Null(foundList);
    }

    [Fact]
    public async Task GetByOwnerId_Found()
    {
        var repository = new WishListRepository(_mongoClient, _databaseName);

        var ownerId1 = "OwnerId1";
        var ownerId2 = "OwnerId2";

        var list1 = await repository.Add("Name1", ownerId1, default);
        var list2 = await repository.Add("Name2", ownerId2, default);

        var foundLists = await repository.GetByOwnerId(ownerId1, default);

        var foundList = Assert.Single(foundLists);
        Assert.Equal(list1.Id, foundList.Id);
        Assert.Equal(ownerId1, foundList.OwnerId);
    }

    [Fact]
    public async Task GetByOwnerId_NotFound()
    {
        var repository = new WishListRepository(_mongoClient, _databaseName);

        var ownerId = "OwnerId";

        var list = await repository.Add("Name", ownerId, default);

        var foundLists = await repository.GetByOwnerId("NoOwnerId", default);

        Assert.Empty(foundLists);
    }

    [Fact]
    public async Task GetByItemId_Found()
    {
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var list1 = await listRepository.Add("Name1", "OwnerId", default);
        var list2 = await listRepository.Add("Name2", "OwnerId", default);

        var itemId1 = await itemRepository.Add(list1.Id, "myurl1.com", default);
        var itemId2 = await itemRepository.Add(list2.Id, "myurl2.com", default);

        var foundList = await listRepository.GetByItemId(itemId1, default);

        Assert.NotNull(foundList);
        Assert.Equal(list1.Id, foundList.Id);
        var foundItem = Assert.Single(foundList.Items);
        Assert.Equal(itemId1, foundItem.Id);
    }

    [Fact]
    public async Task GetByItemId_NotFound()
    {
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var list = await listRepository.Add("Name", "OwnerId", default);

        await itemRepository.Add(list.Id, "myurl.com", default);

        var foundList = await listRepository.GetByItemId(ObjectId.GenerateNewId(), default);

        Assert.Null(foundList);
    }

    [Fact]
    public async Task Rename_Found()
    {
        var repository = new WishListRepository(_mongoClient, _databaseName);
        var oldName = "Name";
        var newName = "Name1";

        var list = await repository.Add(oldName, "OwnerId", default);

        Assert.NotNull(list);
        Assert.Equal(oldName, list.Name);

        var renamedList = await repository.Rename(list.Id, newName, default);

        Assert.NotNull(renamedList);
        Assert.Equal(newName, renamedList.Name);
    }

    [Fact]
    public async Task Rename_NotFound()
    {
        var repository = new WishListRepository(_mongoClient, _databaseName);

        var oldName = "Name1";
        var newName = "Name2";

        var list = await repository.Add(oldName, "OwnerId", default);

        Assert.NotNull(list);
        Assert.Equal(oldName, list.Name);

        var renamedList = await repository.Rename(ObjectId.GenerateNewId(), newName, default);

        Assert.Null(renamedList);
    }

    [Fact]
    public async Task Delete_Found()
    {
        var repository = new WishListRepository(_mongoClient, _databaseName);

        var list = await repository.Add("Name", "OwnerId", default);

        Assert.NotNull(list);

        var deleted = await repository.Delete(list.Id, default);
        var foundList = await repository.GetById(list.Id, default);

        Assert.True(deleted);
        Assert.Null(foundList);
    }

    [Fact]
    public async Task Delete_NotFound()
    {
        var repository = new WishListRepository(_mongoClient, _databaseName);

        var list = await repository.Add("Name", "OwnerId", default);

        Assert.NotNull(list);

        var deleted = await repository.Delete(ObjectId.GenerateNewId(), default);

        Assert.False(deleted);
    }
}
