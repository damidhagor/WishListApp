namespace WishListApp.Data.Repositories;

public sealed class WishListRepository(IMongoClient mongoClient) : IWishListRepository
{
    private readonly IMongoCollection<WishList> _collection = mongoClient
        .GetDatabase(MongoDBConstants.DatabaseName)
        .GetCollection<WishList>(MongoDBConstants.WishListsCollectionName);

    public async Task<WishList> Add(string name, string ownerId, CancellationToken cancellationToken)
    {
        var wishList = new WishList(ObjectId.Empty, ownerId, name, []);
        await _collection.InsertOneAsync(wishList, null, cancellationToken);
        return wishList;
    }

    public async Task<WishList?> GetById(ObjectId wishListId, CancellationToken cancellationToken)
    {
        return await _collection.Find(w => w.Id == wishListId, null).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<WishList>> GetByOwner(string ownerId, CancellationToken cancellationToken)
    {
        return await _collection.Find(w => w.OwnerId == ownerId, null).ToListAsync(cancellationToken);
    }

    public async Task<WishList?> GetByItemId(ObjectId itemId, CancellationToken cancellationToken)
    {
        return await _collection.Find(w => w.Items.Any(i => i.Id == itemId), null).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<WishList?> Rename(ObjectId wishListId, string name, CancellationToken cancellationToken)
    {
        return await _collection.FindOneAndUpdateAsync(
            w => w.Id == wishListId,
            Builders<WishList>.Update.Set(w => w.Name, name),
            new() { ReturnDocument = ReturnDocument.After },
            cancellationToken);
    }

    public async Task<bool> Delete(ObjectId wishListId, CancellationToken cancellationToken)
    {
        var result = await _collection.DeleteOneAsync(w => w.Id == wishListId, cancellationToken);
        return result.DeletedCount > 0;
    }
}
