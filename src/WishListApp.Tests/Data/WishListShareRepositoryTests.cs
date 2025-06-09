using MongoDB.Bson;
using MongoDB.Driver;
using WishListApp.Data.Repositories;
using WishListApp.Tests.Fixtures.MongoDb;

namespace WishListApp.Tests.Data;

[Collection("MongoDb")]
public sealed class WishListShareRepositoryTests(MongoDbFixture mongoDbFixture)
{
    private readonly IMongoDatabase _database = mongoDbFixture.GetMongoClient().GetDatabase(Guid.NewGuid().ToString());
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    [Fact]
    public async Task Add_New()
    {
        var repository = new WishListShareRepository(_database);

        var listId = ObjectId.GenerateNewId();
        var name = "Share";
        var accessKey = "AccessKey";

        var share = await repository.Add(listId, name, accessKey, _cancellationToken);

        Assert.NotNull(share);
        Assert.NotEqual(ObjectId.Empty, share.Id);
        Assert.Equal(listId, share.WishListId);
        Assert.Equal(name, share.Name);
        Assert.Equal(accessKey, share.AccessKey);
    }

    [Fact]
    public async Task Add_AllowDuplicate()
    {
        var repository = new WishListShareRepository(_database);

        var listId = ObjectId.GenerateNewId();
        var name = "Share";
        var accessKey = "AccessKey";

        var share1 = await repository.Add(listId, name, accessKey, _cancellationToken);
        var share2 = await repository.Add(listId, name, accessKey, _cancellationToken);

        Assert.NotNull(share1);
        Assert.NotNull(share2);
        Assert.NotEqual(share1.Id, share2.Id);
    }

    [Fact]
    public async Task GetById_Found()
    {
        var repository = new WishListShareRepository(_database);

        var share1 = await repository.Add(ObjectId.GenerateNewId(), "Name1", "AccessKey1", _cancellationToken);
        var share2 = await repository.Add(ObjectId.GenerateNewId(), "Name2", "AccessKey2", _cancellationToken);

        var foundShare = await repository.GetById(share1.Id, _cancellationToken);

        Assert.NotNull(foundShare);
        Assert.Equal(share1.Id, foundShare.Id);
    }

    [Fact]
    public async Task GetById_NotFound()
    {
        var repository = new WishListShareRepository(_database);

        var share = await repository.Add(ObjectId.GenerateNewId(), "Name1", "AccessKey1", _cancellationToken);

        var foundShare = await repository.GetById(ObjectId.GenerateNewId(), _cancellationToken);

        Assert.Null(foundShare);
    }

    [Fact]
    public async Task GetByAccessKey_Found()
    {
        var repository = new WishListShareRepository(_database);

        var accessKey1 = "AccessKey1";
        var accessKey2 = "AccessKey2";

        var share1 = await repository.Add(ObjectId.GenerateNewId(), "Name1", accessKey1, _cancellationToken);
        var share2 = await repository.Add(ObjectId.GenerateNewId(), "Name2", accessKey2, _cancellationToken);

        var foundShare = await repository.GetByAccessKey(accessKey1, _cancellationToken);

        Assert.NotNull(foundShare);
        Assert.Equal(share1.Id, foundShare.Id);
        Assert.Equal(accessKey1, foundShare.AccessKey);
    }

    [Fact]
    public async Task GetByAccessKey_NotFound()
    {
        var repository = new WishListShareRepository(_database);

        var accessKey = "AccessKey";

        var share = await repository.Add(ObjectId.GenerateNewId(), "Name1", accessKey, _cancellationToken);

        var foundShare = await repository.GetByAccessKey("NoAccessKey", _cancellationToken);

        Assert.Null(foundShare);
    }

    [Fact]
    public async Task GetByWishListId_Found()
    {
        var repository = new WishListShareRepository(_database);

        var listId1 = ObjectId.GenerateNewId();
        var listId2 = ObjectId.GenerateNewId();

        var share1 = await repository.Add(listId1, "Name1", "AccessKey1", _cancellationToken);
        var share2 = await repository.Add(listId1, "Name2", "AccessKey2", _cancellationToken);
        var share3 = await repository.Add(listId2, "Name2", "AccessKey2", _cancellationToken);

        var foundShares = await repository.GetByWishListId(listId1, _cancellationToken);

        Assert.NotEmpty(foundShares);
        Assert.Equal(2, foundShares.Count);
        var foundShare1 = foundShares[0];
        var foundShare2 = foundShares[1];
        Assert.Equal(share1.Id, foundShare1.Id);
        Assert.Equal(share2.Id, foundShare2.Id);
    }

    [Fact]
    public async Task GetByWishListId_NotFound()
    {
        var repository = new WishListShareRepository(_database);

        var listId = ObjectId.GenerateNewId();

        var share = await repository.Add(listId, "Name", "AccessKey", _cancellationToken);

        var foundShares = await repository.GetByWishListId(ObjectId.GenerateNewId(), _cancellationToken);

        Assert.Empty(foundShares);
    }

    [Fact]
    public async Task Delete_Found()
    {
        var repository = new WishListShareRepository(_database);

        var share1 = await repository.Add(ObjectId.GenerateNewId(), "Name", "AccessKey", _cancellationToken);
        var share2 = await repository.Add(ObjectId.GenerateNewId(), "Name", "AccessKey", _cancellationToken);

        var deleted = await repository.Delete(share1.Id, _cancellationToken);
        var foundShare1 = await repository.GetById(share1.Id, _cancellationToken);
        var foundShare2 = await repository.GetById(share2.Id, _cancellationToken);

        Assert.True(deleted);
        Assert.Null(foundShare1);
        Assert.NotNull(foundShare2);
    }

    [Fact]
    public async Task Delete_NotFound()
    {
        var repository = new WishListShareRepository(_database);

        var share = await repository.Add(ObjectId.GenerateNewId(), "Name", "AccessKey", _cancellationToken);

        var deleted = await repository.Delete(ObjectId.GenerateNewId(), _cancellationToken);
        var foundShare = await repository.GetById(share.Id, _cancellationToken);

        Assert.False(deleted);
        Assert.NotNull(foundShare);
    }
}
