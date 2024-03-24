namespace WishListApp.Data.Repositories;

public sealed class WishListRepository(IMongoClient mongoClient) : IWishListRepository
{
    private readonly IMongoCollection<WishList> _collection = mongoClient
        .GetDatabase(Constants.DatabaseName)
        .GetCollection<WishList>(Constants.WishListsCollectionName);

    public async Task<WishList> CreateWishList(string name, string ownerId, CancellationToken cancellationToken)
    {
        var wishList = new WishList(ObjectId.Empty, ownerId, name, []);
        await _collection.InsertOneAsync(wishList, null, cancellationToken);
        return wishList;
    }

    public async Task<WishList?> GetWishListById(string wishListId, CancellationToken cancellationToken)
    {
        return await _collection.AsQueryable().FirstOrDefaultAsync(w => w.Id == new ObjectId(wishListId), cancellationToken);
    }

    public async Task<List<WishList>> GetWishListsForOwner(string ownerId, CancellationToken cancellationToken)
    {
        return await _collection.AsQueryable().Where(w => w.OwnerId == ownerId).ToListAsync(cancellationToken);
    }

    public async Task<WishList?> RenameWishList(string wishListId, string name, CancellationToken cancellationToken)
    {
        return await _collection.FindOneAndUpdateAsync(
            w => w.Id == new ObjectId(wishListId),
            Builders<WishList>.Update.Set(w => w.Name, name),
            new() { ReturnDocument = ReturnDocument.After },
            cancellationToken);
    }

    public async Task<bool> DeleteWishList(string wishListId, CancellationToken cancellationToken)
    {
        var result = await _collection.DeleteOneAsync(w => w.Id == new ObjectId(wishListId), null, cancellationToken);
        return result.DeletedCount > 0;
    }
}
