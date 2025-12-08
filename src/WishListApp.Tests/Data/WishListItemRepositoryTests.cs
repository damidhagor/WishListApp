using MongoDB.Bson;
using MongoDB.Driver;
using WishListApp.Data.Repositories;
using WishListApp.Tests.Fixtures.MongoDb;

namespace WishListApp.Tests.Data;

[Collection(typeof(MongoDbCollection))]
public sealed class WishListItemRepositoryTests(MongoDbFixture mongoDbFixture)
{
    private readonly IMongoDatabase _database = mongoDbFixture.GetMongoClient().GetDatabase(Guid.NewGuid().ToString());
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    [Fact]
    public async Task Add_New()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var url = "https://example.com";

        var list1 = await listRepository.Add("Name1", "OwnerId", _cancellationToken);
        var list2 = await listRepository.Add("Name2", "OwnerId", _cancellationToken);

        var itemId = await itemRepository.Add(list1.Id, url, _cancellationToken);

        list1 = await listRepository.GetById(list1.Id, _cancellationToken);
        list2 = await listRepository.GetById(list2.Id, _cancellationToken);

        Assert.NotNull(list1);
        Assert.NotNull(list2);

        var item = Assert.Single(list1.Items);
        Assert.Equal(itemId, item.Id);
        Assert.Equal(url, item.Url);
        Assert.Null(item.ImageUrl);
        Assert.Null(item.SiteName);
        Assert.Null(item.Name);
        Assert.Null(item.Description);
        Assert.Null(item.Price);
        Assert.Null(item.Currency);
        Assert.Equal("", item.Note);
        Assert.Equal(0, item.Priority);
        Assert.Null(item.Purchaser);

        Assert.Empty(list2.Items);
    }

    [Fact]
    public async Task Add_AllowDuplicate()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var url = "https://example.com";

        var list = await listRepository.Add("Name", "OwnerId", _cancellationToken);

        var itemId1 = await itemRepository.Add(list.Id, url, _cancellationToken);
        var itemId2 = await itemRepository.Add(list.Id, url, _cancellationToken);

        list = await listRepository.GetById(list.Id, _cancellationToken);

        Assert.NotNull(list);
        Assert.Equal(2, list.Items.Length);
        var item1 = list.Items[0];
        Assert.Equal(itemId1, item1.Id);
        Assert.Equal(url, item1.Url);
        var item2 = list.Items[1];
        Assert.Equal(itemId2, item2.Id);
        Assert.Equal(url, item2.Url);
    }

    [Fact]
    public async Task Add_NotFound()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var url = "https://example.com";

        var list = await listRepository.Add("Name", "OwnerId", _cancellationToken);

        var itemId = await itemRepository.Add(ObjectId.GenerateNewId(), url, _cancellationToken);

        Assert.Null(itemId);
    }

    [Fact]
    public async Task Delete_Found()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var list1 = await listRepository.Add("Name1", "OwnerId", _cancellationToken);
        var list2 = await listRepository.Add("Name2", "OwnerId", _cancellationToken);

        var itemId1 = await itemRepository.Add(list1.Id, "https://example1.com", _cancellationToken);
        var itemId2 = await itemRepository.Add(list1.Id, "https://example2.com", _cancellationToken);
        var itemId3 = await itemRepository.Add(list2.Id, "https://example3.com", _cancellationToken);

        var result = await itemRepository.Delete(list1.Id, itemId2.Value, _cancellationToken);

        list1 = await listRepository.GetById(list1.Id, _cancellationToken);
        list2 = await listRepository.GetById(list2.Id, _cancellationToken);

        Assert.NotNull(result);
        Assert.NotNull(list1);
        Assert.NotNull(list2);
        Assert.Single(list1.Items);
        Assert.Equal(itemId1.Value, list1.Items[0].Id);
        Assert.Single(list2.Items);
        Assert.Equal(itemId3.Value, list2.Items[0].Id);
    }

    [Fact]
    public async Task Delete_WishListNotFound()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var list = await listRepository.Add("Name", "OwnerId", _cancellationToken);
        var itemId = await itemRepository.Add(list.Id, "https://example.com", _cancellationToken);

        var result = await itemRepository.Delete(ObjectId.GenerateNewId(), itemId.Value, _cancellationToken);

        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_ItemNotFound()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var list = await listRepository.Add("Name", "OwnerId", _cancellationToken);
        var itemId = await itemRepository.Add(list.Id, "https://example.com", _cancellationToken);

        var result = await itemRepository.Delete(list.Id, ObjectId.GenerateNewId(), _cancellationToken);

        Assert.NotNull(result);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task DeletePurchasedItems_SingleQuantity()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var list1 = await listRepository.Add("Name1", "OwnerId", _cancellationToken);
        var list2 = await listRepository.Add("Name2", "OwnerId", _cancellationToken);

        var itemId1 = await itemRepository.Add(list1.Id, "https://example1.com", _cancellationToken);
        var itemId2 = await itemRepository.Add(list1.Id, "https://example2.com", _cancellationToken);
        var itemId3 = await itemRepository.Add(list1.Id, "https://example3.com", _cancellationToken);
        var itemId4 = await itemRepository.Add(list2.Id, "https://example4.com", _cancellationToken);
        var itemId5 = await itemRepository.Add(list2.Id, "https://example5.com", _cancellationToken);

        await itemRepository.UpdatePurchaser(list1.Id, itemId1.Value, ObjectId.GenerateNewId(), _cancellationToken);
        await itemRepository.UpdatePurchaser(list1.Id, itemId3.Value, ObjectId.GenerateNewId(), _cancellationToken);
        await itemRepository.UpdatePurchaser(list2.Id, itemId5.Value, ObjectId.GenerateNewId(), _cancellationToken);

        var result = await itemRepository.DeletePurchasedItems(list1.Id, _cancellationToken);
        list1 = await listRepository.GetById(list1.Id, _cancellationToken);
        list2 = await listRepository.GetById(list2.Id, _cancellationToken);

        Assert.NotNull(result);
        Assert.NotNull(list1);
        Assert.NotNull(list2);

        Assert.Single(list1.Items);
        Assert.Equal(itemId2.Value, list1.Items[0].Id);

        Assert.Equal(2, list2.Items.Length);
    }

    [Fact]
    public async Task DeletePurchasedItems_WishListNotFound()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var list = await listRepository.Add("Name1", "OwnerId", _cancellationToken);

        var itemId = await itemRepository.Add(list.Id, "https://example.com", _cancellationToken);

        await itemRepository.UpdatePurchaser(list.Id, itemId.Value, ObjectId.GenerateNewId(), _cancellationToken);

        var result = await itemRepository.DeletePurchasedItems(ObjectId.GenerateNewId(), _cancellationToken);

        Assert.Null(result);
    }

    [Fact]
    public async Task Update_Found()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var newImageUrl = "https://newexample.com";
        var newSiteName = "SiteName2";
        var newName = "Name2";
        var newDescription = "Description2";
        var newPrice = 1.23m;
        var newCurrency = "USD";
        var newNote = "Note2";
        var newPriority = 3;
        var newPurchaser = ObjectId.GenerateNewId();

        var list1 = await listRepository.Add("Name1", "OwnerId", _cancellationToken);
        var list2 = await listRepository.Add("Name2", "OwnerId", _cancellationToken);

        var itemId1 = await itemRepository.Add(list1.Id, "https://example1.com", _cancellationToken);
        var itemId2 = await itemRepository.Add(list1.Id, "https://example2.com", _cancellationToken);
        var itemId3 = await itemRepository.Add(list2.Id, "https://example3.com", _cancellationToken);

        list1 = await listRepository.GetById(list1.Id, _cancellationToken);
        Assert.NotNull(list1);

        var item1 = list1.Items.First(i => i.Id == itemId1);

        var result = await itemRepository.Update(
            list1.Id,
            item1 with
            {
                ImageUrl = newImageUrl,
                SiteName = newSiteName,
                Name = newName,
                Description = newDescription,
                Price = newPrice,
                Currency = newCurrency,
                Note = newNote,
                Priority = newPriority,
                Purchaser = newPurchaser
            },
            _cancellationToken);

        list1 = await listRepository.GetById(list1.Id, _cancellationToken);
        list2 = await listRepository.GetById(list2.Id, _cancellationToken);

        Assert.NotNull(result);
        Assert.NotNull(list1);
        Assert.NotNull(list2);

        item1 = list1.Items.First(i => i.Id == itemId1);
        Assert.Equal("https://example1.com", item1.Url);
        Assert.Equal(newImageUrl, item1.ImageUrl);
        Assert.Equal(newSiteName, item1.SiteName);
        Assert.Equal(newName, item1.Name);
        Assert.Equal(newDescription, item1.Description);
        Assert.Equal(newPrice, item1.Price);
        Assert.Equal(newCurrency, item1.Currency);
        Assert.Equal(newNote, item1.Note);
        Assert.Equal(newPriority, item1.Priority);
        Assert.Equal(newPurchaser, item1.Purchaser);

        var item2 = list1.Items.First(i => i.Id == itemId2);
        Assert.Null(item2.Name);

        var item3 = list2.Items.First(i => i.Id == itemId3);
        Assert.Null(item3.Name);
    }

    [Fact]
    public async Task Update_WishListNotFound()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var list = await listRepository.Add("Name1", "OwnerId", _cancellationToken);

        var itemId = await itemRepository.Add(list.Id, "https://example.com", _cancellationToken);

        list = await listRepository.GetById(list.Id, _cancellationToken);

        Assert.NotNull(list);

        var updatedItem = list.Items.First(i => i.Id == itemId) with { Name = "Name2" };

        var result = await itemRepository.Update(ObjectId.GenerateNewId(), updatedItem, _cancellationToken);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdatePriority_Found()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var newPriority = 3;

        var list1 = await listRepository.Add("Name1", "OwnerId", _cancellationToken);
        var list2 = await listRepository.Add("Name2", "OwnerId", _cancellationToken);

        var itemId1 = await itemRepository.Add(list1.Id, "https://example1.com", _cancellationToken);
        var itemId2 = await itemRepository.Add(list1.Id, "https://example2.com", _cancellationToken);
        var itemId3 = await itemRepository.Add(list2.Id, "https://example3.com", _cancellationToken);

        var result = await itemRepository.UpdatePriority(list1.Id, itemId2.Value, newPriority, _cancellationToken);

        list1 = await listRepository.GetById(list1.Id, _cancellationToken);
        list2 = await listRepository.GetById(list2.Id, _cancellationToken);

        Assert.NotNull(result);
        Assert.NotNull(list1);
        Assert.NotNull(list2);

        var item1 = list1.Items.First(i => i.Id == itemId1);
        Assert.Equal(0, item1.Priority);

        var item2 = list1.Items.First(i => i.Id == itemId2);
        Assert.Equal(newPriority, item2.Priority);

        var item3 = list2.Items.First(i => i.Id == itemId3);
        Assert.Equal(0, item3.Priority);
    }

    [Fact]
    public async Task UpdatePriority_WishListNotFound()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var list = await listRepository.Add("Name1", "OwnerId", _cancellationToken);

        var itemId = await itemRepository.Add(list.Id, "https://example.com", _cancellationToken);

        var result = await itemRepository.UpdatePriority(ObjectId.GenerateNewId(), itemId.Value, 3, _cancellationToken);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdatePurchaser_Set()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var shareId = ObjectId.GenerateNewId();

        var list1 = await listRepository.Add("Name1", "OwnerId", _cancellationToken);
        var list2 = await listRepository.Add("Name2", "OwnerId", _cancellationToken);

        var itemId1 = await itemRepository.Add(list1.Id, "https://example1.com", _cancellationToken);
        var itemId2 = await itemRepository.Add(list1.Id, "https://example2.com", _cancellationToken);
        var itemId3 = await itemRepository.Add(list2.Id, "https://example3.com", _cancellationToken);

        var result = await itemRepository.UpdatePurchaser(list1.Id, itemId2.Value, shareId, _cancellationToken);

        list1 = await listRepository.GetById(list1.Id, _cancellationToken);
        list2 = await listRepository.GetById(list2.Id, _cancellationToken);

        Assert.NotNull(result);
        Assert.NotNull(list1);
        Assert.NotNull(list2);

        var item1 = list1.Items.First(i => i.Id == itemId1);
        Assert.Null(item1.Purchaser);

        var item2 = list1.Items.First(i => i.Id == itemId2);
        Assert.Equal(shareId, item2.Purchaser);

        var item3 = list2.Items.First(i => i.Id == itemId3);
        Assert.Null(item3.Purchaser);
    }

    [Fact]
    public async Task UpdatePurchaser_Reset()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var shareId = ObjectId.GenerateNewId();

        var list1 = await listRepository.Add("Name1", "OwnerId", _cancellationToken);
        var list2 = await listRepository.Add("Name2", "OwnerId", _cancellationToken);

        var itemId1 = await itemRepository.Add(list1.Id, "https://example1.com", _cancellationToken);
        var itemId2 = await itemRepository.Add(list1.Id, "https://example2.com", _cancellationToken);
        var itemId3 = await itemRepository.Add(list2.Id, "https://example3.com", _cancellationToken);

        var result = await itemRepository.UpdatePurchaser(list1.Id, itemId2.Value, shareId, _cancellationToken);

        await itemRepository.UpdatePurchaser(list1.Id, itemId2.Value, null, _cancellationToken);

        list1 = await listRepository.GetById(list1.Id, _cancellationToken);
        list2 = await listRepository.GetById(list2.Id, _cancellationToken);

        Assert.NotNull(result);
        Assert.NotNull(list1);
        Assert.NotNull(list2);

        var item1 = result.Items.First(i => i.Id == itemId1);
        Assert.Null(item1.Purchaser);

        var item2 = list1.Items.First(i => i.Id == itemId1);
        Assert.Null(item2.Purchaser);

        var item3 = list1.Items.First(i => i.Id == itemId2);
        Assert.Null(item3.Purchaser);

        var item4 = list2.Items.First(i => i.Id == itemId3);
        Assert.Null(item4.Purchaser);
    }

    [Fact]
    public async Task UpdatePurchaser_WishListNotFound()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var shareId = ObjectId.GenerateNewId();

        var list = await listRepository.Add("Name", "OwnerId", _cancellationToken);
        var itemId = await itemRepository.Add(list.Id, "https://example.com", _cancellationToken);

        var result = await itemRepository.UpdatePurchaser(ObjectId.GenerateNewId(), itemId.Value, shareId, _cancellationToken);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdatePurchaser_ItemNotFound()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var shareId = ObjectId.GenerateNewId();

        var list = await listRepository.Add("Name", "OwnerId", _cancellationToken);
        await itemRepository.Add(list.Id, "https://example.com", _cancellationToken);

        var result = await itemRepository.UpdatePurchaser(list.Id, ObjectId.GenerateNewId(), shareId, _cancellationToken);

        Assert.Null(result);
    }

    [Fact]
    public async Task MoveToWishList_Moved()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var list1 = await listRepository.Add("Name1", "OwnerId", _cancellationToken);
        var list2 = await listRepository.Add("Name2", "OwnerId", _cancellationToken);

        var itemId1 = await itemRepository.Add(list1.Id, "https://example1.com", _cancellationToken);
        var itemId2 = await itemRepository.Add(list1.Id, "https://example2.com", _cancellationToken);
        var itemId3 = await itemRepository.Add(list2.Id, "https://example3.com", _cancellationToken);

        await itemRepository.UpdatePurchaser(list1.Id, itemId1.Value, ObjectId.GenerateNewId(), _cancellationToken);
        await itemRepository.UpdatePurchaser(list1.Id, itemId2.Value, ObjectId.GenerateNewId(), _cancellationToken);
        await itemRepository.UpdatePurchaser(list2.Id, itemId3.Value, ObjectId.GenerateNewId(), _cancellationToken);

        var newItemId1 = await itemRepository.MoveToWishList(list1.Id, itemId1.Value, list2.Id, _cancellationToken);

        list1 = await listRepository.GetById(list1.Id, _cancellationToken);
        list2 = await listRepository.GetById(list2.Id, _cancellationToken);

        Assert.NotNull(newItemId1);
        Assert.NotNull(list1);
        Assert.NotNull(list2);

        var item2 = Assert.Single(list1.Items);
        Assert.Equal(itemId2, item2.Id);
        Assert.NotNull(item2.Purchaser);

        Assert.Equal(2, list2.Items.Length);

        var item1 = list2.Items.First(i => i.Id == newItemId1);
        Assert.Equal(newItemId1, item1.Id);
        Assert.Equal("https://example1.com", item1.Url);
        Assert.Null(item1.Purchaser);

        var item3 = list2.Items.First(i => i.Id == itemId3);
        Assert.Equal(itemId3, item3.Id);
        Assert.NotNull(item3.Purchaser);
    }

    [Fact]
    public async Task MoveToWishList_OldWishListNotFound()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var list1 = await listRepository.Add("Name1", "OwnerId", _cancellationToken);
        var list2 = await listRepository.Add("Name2", "OwnerId", _cancellationToken);

        var itemId1 = await itemRepository.Add(list1.Id, "https://example1.com", _cancellationToken);

        var newItemId1 = await itemRepository.MoveToWishList(ObjectId.GenerateNewId(), itemId1.Value, list2.Id, _cancellationToken);

        list1 = await listRepository.GetById(list1.Id, _cancellationToken);
        list2 = await listRepository.GetById(list2.Id, _cancellationToken);

        Assert.Null(newItemId1);
        Assert.NotNull(list1);
        Assert.NotNull(list2);

        Assert.Single(list1.Items);
        Assert.Empty(list2.Items);
    }

    [Fact]
    public async Task MoveToWishList_NewWishListNotFound()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var list1 = await listRepository.Add("Name1", "OwnerId", _cancellationToken);
        var list2 = await listRepository.Add("Name2", "OwnerId", _cancellationToken);

        var itemId1 = await itemRepository.Add(list1.Id, "https://example1.com", _cancellationToken);

        var newItemId1 = await itemRepository.MoveToWishList(list1.Id, itemId1.Value, ObjectId.GenerateNewId(), _cancellationToken);

        list1 = await listRepository.GetById(list1.Id, _cancellationToken);
        list2 = await listRepository.GetById(list2.Id, _cancellationToken);

        Assert.Null(newItemId1);
        Assert.NotNull(list1);
        Assert.NotNull(list2);

        Assert.Single(list1.Items);
        Assert.Empty(list2.Items);
    }

    [Fact]
    public async Task MoveToWishList_ItemNotFound()
    {
        var listRepository = new WishListRepository(_database);
        var itemRepository = new WishListItemRepository(_database);

        var list1 = await listRepository.Add("Name1", "OwnerId", _cancellationToken);
        var list2 = await listRepository.Add("Name2", "OwnerId", _cancellationToken);

        var itemId1 = await itemRepository.Add(list1.Id, "https://example1.com", _cancellationToken);

        var newItemId1 = await itemRepository.MoveToWishList(list1.Id, ObjectId.GenerateNewId(), list2.Id, _cancellationToken);

        list1 = await listRepository.GetById(list1.Id, _cancellationToken);
        list2 = await listRepository.GetById(list2.Id, _cancellationToken);

        Assert.Null(newItemId1);
        Assert.NotNull(list1);
        Assert.NotNull(list2);

        Assert.Single(list1.Items);
        Assert.Empty(list2.Items);
    }
}
