namespace WishListApp.Data.Repositories;

public sealed class WishListShareRepository(IMongoClient mongoClient) : IWishListShareRepository
{
    private readonly IMongoCollection<WishListShare> _collection = mongoClient
        .GetDatabase("wishlist")
        .GetCollection<WishListShare>("shares");

    public async Task<WishListShare> Add(ObjectId wishListId, string shareName, string shareAccessKey, CancellationToken cancellationToken)
    {
        var share = new WishListShare(ObjectId.Empty, wishListId, shareName, shareAccessKey);
        await _collection.InsertOneAsync(share, null, cancellationToken);
        return share;
    }

    public async Task<WishListShare?> GetById(ObjectId shareId, CancellationToken cancellationToken)
    {
        return await _collection.Find(s => s.Id == shareId, null).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<WishListShare?> GetByAccessKey(string accessKey, CancellationToken cancellationToken)
    {
        return await _collection.Find(s => s.AccessKey == accessKey, null).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<WishListShare>> GetByWishListId(ObjectId wishListId, CancellationToken cancellationToken)
    {
        return await _collection.Find(s => s.WishListId == wishListId, null).ToListAsync(cancellationToken);
    }

    public async Task<bool> Delete(ObjectId shareId, CancellationToken cancellationToken)
    {
        var result = await _collection.DeleteOneAsync(s => s.Id == shareId, cancellationToken);
        return result.DeletedCount > 0;
    }
}
