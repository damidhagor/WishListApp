using MongoDB.Bson;
using MongoDB.Driver;
using WishListApp.Data.Repositories;
using WishListApp.Tests.Fixtures.MongoDb;

namespace WishListApp.Tests.Data;

[Collection("MongoDb")]
public sealed class WishListRepositoryTests(MongoDbFixture mongoDbFixture)
{
    private readonly IMongoDatabase _database = mongoDbFixture.GetMongoClient().GetDatabase(Guid.NewGuid().ToString());
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    [Fact]
    public async Task Add_New()
    {
        var repository = new WishListRepository(_database);
        var name = "Name";
        var ownerId = "OwnerId";

        var list = await repository.Add(name, ownerId, _cancellationToken);

        Assert.NotNull(list);
        Assert.NotEqual(ObjectId.Empty, list.Id);
        Assert.Equal(name, list.Name);
        Assert.Equal(ownerId, list.OwnerId);
        Assert.Empty(list.Items);
    }

    [Fact]
    public async Task Add_AllowDuplicate()
    {
        var repository = new WishListRepository(_database);
        var name = "Name";
        var ownerId = "OwnerId";

        var list1 = await repository.Add(name, ownerId, _cancellationToken);
        var list2 = await repository.Add(name, ownerId, _cancellationToken);

        Assert.NotNull(list1);
        Assert.NotNull(list2);
        Assert.NotEqual(list1.Id, list2.Id);
    }

    [Fact]
    public async Task GetById_Found()
    {
        var repository = new WishListRepository(_database);

        var name1 = "Name1";
        var name2 = "Name2";

        var list1 = await repository.Add(name1, "OwnerId", _cancellationToken);
        var list2 = await repository.Add(name2, "OwnerId", _cancellationToken);

        var foundList = await repository.GetById(list1.Id, _cancellationToken);

        Assert.NotNull(foundList);
        Assert.Equal(list1.Id, foundList.Id);
    }

    [Fact]
    public async Task GetById_NotFound()
    {
        var repository = new WishListRepository(_database);

        var list = await repository.Add("Name", "OwnerId", _cancellationToken);

        var foundList = await repository.GetById(ObjectId.GenerateNewId(), _cancellationToken);

        Assert.Null(foundList);
    }

    [Fact]
    public async Task GetByOwnerId_Found()
    {
        var repository = new WishListRepository(_database);

        var ownerId1 = "OwnerId1";
        var ownerId2 = "OwnerId2";

        var list1 = await repository.Add("Name1", ownerId1, _cancellationToken);
        var list2 = await repository.Add("Name2", ownerId1, _cancellationToken);
        var list3 = await repository.Add("Name2", ownerId2, _cancellationToken);

        var foundLists = await repository.GetByOwnerId(ownerId1, _cancellationToken);

        Assert.NotEmpty(foundLists);
        Assert.Equal(2, foundLists.Count);
        var foundList1 = foundLists[0];
        var foundList2 = foundLists[1];
        Assert.Equal(list1.Id, foundList1.Id);
        Assert.Equal(list2.Id, foundList2.Id);
    }

    [Fact]
    public async Task GetByOwnerId_NotFound()
    {
        var repository = new WishListRepository(_database);

        var ownerId = "OwnerId";

        var list = await repository.Add("Name", ownerId, _cancellationToken);

        var foundLists = await repository.GetByOwnerId("NoOwnerId", _cancellationToken);

        Assert.Empty(foundLists);
    }

    [Fact]
    public async Task GetByItemId_Found()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var list1 = await listRepository.Add("Name1", "OwnerId", _cancellationToken);
        var list2 = await listRepository.Add("Name2", "OwnerId", _cancellationToken);

        var itemId1 = await itemRepository.Add(list1.Id, "myurl1.com", _cancellationToken);
        var itemId2 = await itemRepository.Add(list2.Id, "myurl2.com", _cancellationToken);

        var foundList = await listRepository.GetByItemId(itemId1.Value, _cancellationToken);

        Assert.NotNull(foundList);
        Assert.Equal(list1.Id, foundList.Id);
        var foundItem = Assert.Single(foundList.Items);
        Assert.Equal(itemId1, foundItem.Id);
    }

    [Fact]
    public async Task GetByItemId_NotFound()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var list = await listRepository.Add("Name", "OwnerId", _cancellationToken);

        await itemRepository.Add(list.Id, "myurl.com", _cancellationToken);

        var foundList = await listRepository.GetByItemId(ObjectId.GenerateNewId(), _cancellationToken);

        Assert.Null(foundList);
    }

    [Fact]
    public async Task Rename_Found()
    {
        var repository = new WishListRepository(_database);
        var oldName = "Name";
        var newName = "Name1";

        var list = await repository.Add(oldName, "OwnerId", _cancellationToken);

        Assert.NotNull(list);
        Assert.Equal(oldName, list.Name);

        var renamedList = await repository.Rename(list.Id, newName, _cancellationToken);

        Assert.NotNull(renamedList);
        Assert.Equal(newName, renamedList.Name);
    }

    [Fact]
    public async Task Rename_NotFound()
    {
        var repository = new WishListRepository(_database);

        var oldName = "Name1";
        var newName = "Name2";

        var list = await repository.Add(oldName, "OwnerId", _cancellationToken);

        Assert.NotNull(list);
        Assert.Equal(oldName, list.Name);

        var renamedList = await repository.Rename(ObjectId.GenerateNewId(), newName, _cancellationToken);

        Assert.Null(renamedList);
    }

    [Fact]
    public async Task Delete_Found()
    {
        var repository = new WishListRepository(_database);

        var list1 = await repository.Add("Name", "OwnerId", _cancellationToken);
        var list2 = await repository.Add("Name", "OwnerId", _cancellationToken);

        Assert.NotNull(list1);
        Assert.NotNull(list2);

        var deleted = await repository.Delete(list1.Id, _cancellationToken);
        var foundList1 = await repository.GetById(list1.Id, _cancellationToken);
        var foundList2 = await repository.GetById(list2.Id, _cancellationToken);

        Assert.True(deleted);
        Assert.Null(foundList1);
        Assert.NotNull(foundList2);
        Assert.Equal(list2.Id, foundList2.Id);
    }

    [Fact]
    public async Task Delete_NotFound()
    {
        var repository = new WishListRepository(_database);

        var list = await repository.Add("Name", "OwnerId", _cancellationToken);

        Assert.NotNull(list);

        var deleted = await repository.Delete(ObjectId.GenerateNewId(), _cancellationToken);

        Assert.False(deleted);
    }
}
