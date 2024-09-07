using MongoDB.Bson;
using MongoDB.Driver;
using WishListApp.Data.Repositories;
using WishListApp.Tests.Fixtures.MongoDb;

namespace WishListApp.Tests.Data;

[Collection("MongoDb")]
public sealed class WishListItemRepositoryTests(MongoDbFixture mongoDbFixture)
{
    private readonly IMongoClient _mongoClient = mongoDbFixture.GetMongoClient();
    private readonly string _databaseName = Guid.NewGuid().ToString();

    [Fact]
    public async Task Add_New()
    {
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var url = "https://example.com";

        var list1 = await listRepository.Add("Name1", "OwnerId", default);
        var list2 = await listRepository.Add("Name2", "OwnerId", default);

        var itemId = await itemRepository.Add(list1.Id, url, default);

        list1 = await listRepository.GetById(list1.Id, default);
        list2 = await listRepository.GetById(list2.Id, default);

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
        Assert.Equal(1, item.Quantity);
        Assert.Equal("", item.Note);
        Assert.Equal(0, item.Priority);
        Assert.Null(item.Purchaser);

        Assert.Empty(list2.Items);
    }

    [Fact]
    public async Task Add_AllowDuplicate()
    {
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var url = "https://example.com";

        var list = await listRepository.Add("Name", "OwnerId", default);

        var itemId1 = await itemRepository.Add(list.Id, url, default);
        var itemId2 = await itemRepository.Add(list.Id, url, default);

        list = await listRepository.GetById(list.Id, default);

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
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var url = "https://example.com";

        var list = await listRepository.Add("Name", "OwnerId", default);

        var itemId = await itemRepository.Add(ObjectId.GenerateNewId(), url, default);

        Assert.Null(itemId);
    }

    [Fact]
    public async Task Delete_Found()
    {
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var list1 = await listRepository.Add("Name1", "OwnerId", default);
        var list2 = await listRepository.Add("Name2", "OwnerId", default);

        var itemId1 = await itemRepository.Add(list1.Id, "https://example1.com", default);
        var itemId2 = await itemRepository.Add(list1.Id, "https://example2.com", default);
        var itemId3 = await itemRepository.Add(list2.Id, "https://example3.com", default);

        var result = await itemRepository.Delete(list1.Id, itemId2.Value, default);

        list1 = await listRepository.GetById(list1.Id, default);
        list2 = await listRepository.GetById(list2.Id, default);

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
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var list = await listRepository.Add("Name", "OwnerId", default);
        var itemId = await itemRepository.Add(list.Id, "https://example.com", default);

        var result = await itemRepository.Delete(ObjectId.GenerateNewId(), itemId.Value, default);

        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_ItemNotFound()
    {
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var list = await listRepository.Add("Name", "OwnerId", default);
        var itemId = await itemRepository.Add(list.Id, "https://example.com", default);

        var result = await itemRepository.Delete(list.Id, ObjectId.GenerateNewId(), default);

        Assert.NotNull(result);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task DeletePurchasedItems_SingleQuantity()
    {
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var list1 = await listRepository.Add("Name1", "OwnerId", default);
        var list2 = await listRepository.Add("Name2", "OwnerId", default);

        var itemId1 = await itemRepository.Add(list1.Id, "https://example1.com", default);
        var itemId2 = await itemRepository.Add(list1.Id, "https://example2.com", default);
        var itemId3 = await itemRepository.Add(list1.Id, "https://example3.com", default);
        var itemId4 = await itemRepository.Add(list2.Id, "https://example4.com", default);
        var itemId5 = await itemRepository.Add(list2.Id, "https://example5.com", default);

        await itemRepository.SetPurchaser(list1.Id, itemId1.Value, ObjectId.GenerateNewId(), default);
        await itemRepository.SetPurchaser(list1.Id, itemId3.Value, ObjectId.GenerateNewId(), default);
        await itemRepository.SetPurchaser(list2.Id, itemId5.Value, ObjectId.GenerateNewId(), default);

        var result = await itemRepository.DeletePurchasedItems(list1.Id, default);
        list1 = await listRepository.GetById(list1.Id, default);
        list2 = await listRepository.GetById(list2.Id, default);

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
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var list = await listRepository.Add("Name1", "OwnerId", default);

        var itemId = await itemRepository.Add(list.Id, "https://example.com", default);

        await itemRepository.SetPurchaser(list.Id, itemId.Value, ObjectId.GenerateNewId(), default);

        var result = await itemRepository.DeletePurchasedItems(ObjectId.GenerateNewId(), default);

        Assert.Null(result);
    }

    [Fact]
    public async Task Update_Found()
    {
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var newImageUrl = "https://newexample.com";
        var newSiteName = "SiteName2";
        var newName = "Name2";
        var newDescription = "Description2";
        var newPrice = 1.23m;
        var newCurrency = "USD";
        var newQuantity = 2;
        var newNote = "Note2";
        var newPriority = 3;
        var newPurchaser = ObjectId.GenerateNewId();

        var list1 = await listRepository.Add("Name1", "OwnerId", default);
        var list2 = await listRepository.Add("Name2", "OwnerId", default);

        var itemId1 = await itemRepository.Add(list1.Id, "https://example1.com", default);
        var itemId2 = await itemRepository.Add(list1.Id, "https://example2.com", default);
        var itemId3 = await itemRepository.Add(list2.Id, "https://example3.com", default);

        list1 = await listRepository.GetById(list1.Id, default);
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
                Quantity = newQuantity,
                Note = newNote,
                Priority = newPriority,
                Purchaser = newPurchaser
            },
            default);

        list1 = await listRepository.GetById(list1.Id, default);
        list2 = await listRepository.GetById(list2.Id, default);

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
        Assert.Equal(newQuantity, item1.Quantity);
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
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var list = await listRepository.Add("Name1", "OwnerId", default);

        var itemId = await itemRepository.Add(list.Id, "https://example.com", default);

        list = await listRepository.GetById(list.Id, default);

        Assert.NotNull(list);

        var updatedItem = list.Items.First(i => i.Id == itemId) with { Name = "Name2" };

        var result = await itemRepository.Update(ObjectId.GenerateNewId(), updatedItem, default);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdatePriority_Found()
    {
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var newPriority = 3;

        var list1 = await listRepository.Add("Name1", "OwnerId", default);
        var list2 = await listRepository.Add("Name2", "OwnerId", default);

        var itemId1 = await itemRepository.Add(list1.Id, "https://example1.com", default);
        var itemId2 = await itemRepository.Add(list1.Id, "https://example2.com", default);
        var itemId3 = await itemRepository.Add(list2.Id, "https://example3.com", default);

        var result = await itemRepository.UpdatePriority(list1.Id, itemId2.Value, newPriority, default);

        list1 = await listRepository.GetById(list1.Id, default);
        list2 = await listRepository.GetById(list2.Id, default);

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
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var list = await listRepository.Add("Name1", "OwnerId", default);

        var itemId = await itemRepository.Add(list.Id, "https://example.com", default);

        var result = await itemRepository.UpdatePriority(ObjectId.GenerateNewId(), itemId.Value, 3, default);

        Assert.Null(result);
    }

    [Fact]
    public async Task SetPurchaser_Set()
    {
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var shareId = ObjectId.GenerateNewId();

        var list1 = await listRepository.Add("Name1", "OwnerId", default);
        var list2 = await listRepository.Add("Name2", "OwnerId", default);

        var itemId1 = await itemRepository.Add(list1.Id, "https://example1.com", default);
        var itemId2 = await itemRepository.Add(list1.Id, "https://example2.com", default);
        var itemId3 = await itemRepository.Add(list2.Id, "https://example3.com", default);

        var result = await itemRepository.SetPurchaser(list1.Id, itemId2.Value, shareId, default);

        list1 = await listRepository.GetById(list1.Id, default);
        list2 = await listRepository.GetById(list2.Id, default);

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
    public async Task SetPurchaser_Reset()
    {
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var shareId = ObjectId.GenerateNewId();

        var list1 = await listRepository.Add("Name1", "OwnerId", default);
        var list2 = await listRepository.Add("Name2", "OwnerId", default);

        var itemId1 = await itemRepository.Add(list1.Id, "https://example1.com", default);
        var itemId2 = await itemRepository.Add(list1.Id, "https://example2.com", default);
        var itemId3 = await itemRepository.Add(list2.Id, "https://example3.com", default);

        var result = await itemRepository.SetPurchaser(list1.Id, itemId2.Value, shareId, default);

        await itemRepository.SetPurchaser(list1.Id, itemId2.Value, null, default);

        list1 = await listRepository.GetById(list1.Id, default);
        list2 = await listRepository.GetById(list2.Id, default);

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
    public async Task SetPurchaser_WishListNotFound()
    {
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var shareId = ObjectId.GenerateNewId();

        var list = await listRepository.Add("Name", "OwnerId", default);
        var itemId = await itemRepository.Add(list.Id, "https://example.com", default);

        var result = await itemRepository.SetPurchaser(ObjectId.GenerateNewId(), itemId.Value, shareId, default);

        Assert.Null(result);
    }

    [Fact]
    public async Task SetPurchaser_ItemNotFound()
    {
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var shareId = ObjectId.GenerateNewId();

        var list = await listRepository.Add("Name", "OwnerId", default);
        await itemRepository.Add(list.Id, "https://example.com", default);

        var result = await itemRepository.SetPurchaser(list.Id, ObjectId.GenerateNewId(), shareId, default);

        Assert.Null(result);
    }

    [Fact]
    public async Task MoveToWishList_Moved()
    {
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var list1 = await listRepository.Add("Name1", "OwnerId", default);
        var list2 = await listRepository.Add("Name2", "OwnerId", default);

        var itemId1 = await itemRepository.Add(list1.Id, "https://example1.com", default);
        var itemId2 = await itemRepository.Add(list1.Id, "https://example2.com", default);
        var itemId3 = await itemRepository.Add(list2.Id, "https://example3.com", default);

        await itemRepository.SetPurchaser(list1.Id, itemId1.Value, ObjectId.GenerateNewId(), default);
        await itemRepository.SetPurchaser(list1.Id, itemId2.Value, ObjectId.GenerateNewId(), default);
        await itemRepository.SetPurchaser(list2.Id, itemId3.Value, ObjectId.GenerateNewId(), default);

        var newItemId1 = await itemRepository.MoveToWishList(list1.Id, itemId1.Value, list2.Id, default);

        list1 = await listRepository.GetById(list1.Id, default);
        list2 = await listRepository.GetById(list2.Id, default);

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
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var list1 = await listRepository.Add("Name1", "OwnerId", default);
        var list2 = await listRepository.Add("Name2", "OwnerId", default);

        var itemId1 = await itemRepository.Add(list1.Id, "https://example1.com", default);

        var newItemId1 = await itemRepository.MoveToWishList(ObjectId.GenerateNewId(), itemId1.Value, list2.Id, default);

        list1 = await listRepository.GetById(list1.Id, default);
        list2 = await listRepository.GetById(list2.Id, default);

        Assert.Null(newItemId1);
        Assert.NotNull(list1);
        Assert.NotNull(list2);

        Assert.Single(list1.Items);
        Assert.Empty(list2.Items);
    }

    [Fact]
    public async Task MoveToWishList_NewWishListNotFound()
    {
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var list1 = await listRepository.Add("Name1", "OwnerId", default);
        var list2 = await listRepository.Add("Name2", "OwnerId", default);

        var itemId1 = await itemRepository.Add(list1.Id, "https://example1.com", default);

        var newItemId1 = await itemRepository.MoveToWishList(list1.Id, itemId1.Value, ObjectId.GenerateNewId(), default);

        list1 = await listRepository.GetById(list1.Id, default);
        list2 = await listRepository.GetById(list2.Id, default);

        Assert.Null(newItemId1);
        Assert.NotNull(list1);
        Assert.NotNull(list2);

        Assert.Single(list1.Items);
        Assert.Empty(list2.Items);
    }

    [Fact]
    public async Task MoveToWishList_ItemNotFound()
    {
        var listRepository = new WishListRepository(_mongoClient, _databaseName);
        var itemRepository = new WishListItemRepository(_mongoClient, _databaseName);

        var list1 = await listRepository.Add("Name1", "OwnerId", default);
        var list2 = await listRepository.Add("Name2", "OwnerId", default);

        var itemId1 = await itemRepository.Add(list1.Id, "https://example1.com", default);

        var newItemId1 = await itemRepository.MoveToWishList(list1.Id, ObjectId.GenerateNewId(), list2.Id, default);

        list1 = await listRepository.GetById(list1.Id, default);
        list2 = await listRepository.GetById(list2.Id, default);

        Assert.Null(newItemId1);
        Assert.NotNull(list1);
        Assert.NotNull(list2);

        Assert.Single(list1.Items);
        Assert.Empty(list2.Items);
    }
}
