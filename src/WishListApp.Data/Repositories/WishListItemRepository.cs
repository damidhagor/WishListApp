namespace WishListApp.Data.Repositories;

public sealed class WishListItemRepository(IMongoClient mongoClient, string? databaseName = null) : IWishListItemRepository
{
    private readonly IMongoCollection<WishList> _collection = mongoClient
        .GetDatabase(databaseName ?? MongoDBConstants.DatabaseName)
        .GetCollection<WishList>(MongoDBConstants.WishListsCollectionName);

    public async Task<ObjectId?> Add(ObjectId wishListId, string url, CancellationToken cancellationToken)
    {
        var item = new WishListItem(ObjectId.GenerateNewId(), url, null, null, null, null, null, null, 1, "", 0, []);

        var list = await _collection.FindOneAndUpdateAsync(
            w => w.Id == wishListId,
            Builders<WishList>.Update.Push(w => w.Items, item),
            new() { ReturnDocument = ReturnDocument.After },
            cancellationToken);

        return list?.Items?.FirstOrDefault(i => i.Id == item.Id)?.Id;
    }

    public async Task<WishList?> Delete(ObjectId wishListId, ObjectId itemId, CancellationToken cancellationToken)
    {
        return await _collection.FindOneAndUpdateAsync(
            w => w.Id == wishListId,
            Builders<WishList>.Update.PullFilter(w => w.Items, i => i.Id == itemId),
            new() { ReturnDocument = ReturnDocument.After },
            cancellationToken);
    }

    public async Task<WishList?> DeletePurchasedItems(ObjectId wishListId, CancellationToken cancellationToken)
    {
        return await _collection.FindOneAndUpdateAsync(
            w => w.Id == wishListId,
            Builders<WishList>.Update.PullFilter(w => w.Items, i => i.Purchases.Sum(p => p.Quantity) >= i.Quantity),
            new() { ReturnDocument = ReturnDocument.After },
            cancellationToken);
    }

    public async Task<WishList?> Update(ObjectId wishListId, WishListItem item, CancellationToken cancellationToken)
    {
        return await _collection.FindOneAndUpdateAsync(
            w => w.Id == wishListId && w.Items.Any(i => i.Id == item.Id),
            Builders<WishList>.Update
                .Set(w => w.Items.FirstMatchingElement().ImageUrl, item.ImageUrl)
                .Set(w => w.Items.FirstMatchingElement().SiteName, item.SiteName)
                .Set(w => w.Items.FirstMatchingElement().Name, item.Name)
                .Set(w => w.Items.FirstMatchingElement().Description, item.Description)
                .Set(w => w.Items.FirstMatchingElement().Price, item.Price)
                .Set(w => w.Items.FirstMatchingElement().Currency, item.Currency)
                .Set(w => w.Items.FirstMatchingElement().Quantity, item.Quantity)
                .Set(w => w.Items.FirstMatchingElement().Note, item.Note)
                .Set(w => w.Items.FirstMatchingElement().Priority, item.Priority),
            new() { ReturnDocument = ReturnDocument.After },
            cancellationToken);
    }

    public async Task<WishList?> UpdatePriority(ObjectId wishListId, ObjectId itemId, int priority, CancellationToken cancellationToken)
    {
        return await _collection.FindOneAndUpdateAsync(
            w => w.Id == wishListId && w.Items.Any(i => i.Id == itemId),
            Builders<WishList>.Update.Set(w => w.Items.FirstMatchingElement().Priority, priority),
            new() { ReturnDocument = ReturnDocument.After },
            cancellationToken);
    }

    public async Task<WishList?> UpdatePurchaseQuantity(ObjectId wishListId, ObjectId itemId, ObjectId shareId, int quantity, CancellationToken cancellationToken)
    {
        return await _collection.FindOneAndUpdateAsync(
            w => w.Id == wishListId,
            new EmptyPipelineDefinition<WishList>()
                .AppendStage<WishList, WishList, WishList>(
                $$"""
                {
                  $set: {
                    "{{nameof(WishList.Items)}}": {
                      $map: {
                        input: "${{nameof(WishList.Items)}}",
                        as: "item",
                        in: {
                          $cond: [
                            {
                              $eq: [ "$$item._id", ObjectId("{{itemId}}") ]
                            },
                            {
                              $mergeObjects: [
                                "$$item",
                                {
                                  "{{nameof(WishListItem.Purchases)}}": {
                                    $cond: [
                                      {
                                        $eq: [ {{quantity}}, 0 ]
                                      },
                                      {
                                        $filter: {
                                          input: "$$item.{{nameof(WishListItem.Purchases)}}",
                                          as: "purchase",
                                          cond: { $ne: [ "$$purchase.{{nameof(WishListItemPurchase.ShareId)}}", ObjectId("{{shareId}}") ] }
                                        }
                                      },
                                      {
                                        $cond: [
                                          {
                                            $in: [ ObjectId("{{shareId}}"), "$$item.{{nameof(WishListItem.Purchases)}}.{{nameof(WishListItemPurchase.ShareId)}}" ]
                                          },
                                          {
                                            $map: {
                                              input: "$$item.{{nameof(WishListItem.Purchases)}}",
                                              as: "purchase",
                                              in: {
                                                $mergeObjects: [
                                                  "$$purchase",
                                                  {
                                                    $cond: [
                                                      { $eq: [ "$$purchase.{{nameof(WishListItemPurchase.ShareId)}}", ObjectId("{{shareId}}") ] },
                                                      { "{{nameof(WishListItemPurchase.Quantity)}}": {{quantity}} },
                                                      {}
                                                    ]
                                                  }
                                                ]
                                              }
                                            }
                                          },
                                          {
                                            $concatArrays: [
                                              "$$item.{{nameof(WishListItem.Purchases)}}",
                                              [ { "{{nameof(WishListItemPurchase.ShareId)}}": ObjectId("{{shareId}}"), "{{nameof(WishListItemPurchase.Quantity)}}": {{quantity}} } ]
                                            ]
                                          }
                                        ]
                                      }
                                    ]
                                  }
                                }
                              ]
                            },
                            {
                              $mergeObjects: [ "$$item", {} ]
                            }
                          ]
                        }
                      }
                    }
                  }
                }
                """),
            new() { ReturnDocument = ReturnDocument.After },
            cancellationToken);
    }

    public async Task<WishList?> ResetPurchases(ObjectId wishListId, ObjectId itemId, CancellationToken cancellationToken)
    {
        return await _collection.FindOneAndUpdateAsync(
            w => w.Id == wishListId && w.Items.Any(i => i.Id == itemId),
            Builders<WishList>.Update.Set(w => w.Items.FirstMatchingElement().Purchases, []),
            new() { ReturnDocument = ReturnDocument.After },
            cancellationToken);
    }

    public async Task<ObjectId?> MoveToWishList(ObjectId listId, ObjectId itemId, ObjectId newListId, CancellationToken cancellationToken)
    {
        var item = await _collection.Find(w => w.Id == listId && w.Items.Any(i => i.Id == itemId))
            .Project(w => w.Items.FirstOrDefault(i => i.Id == itemId))
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
        {
            return null;
        }

        item = item with
        {
            Id = ObjectId.GenerateNewId(),
            Purchases = []
        };

        var result = await _collection.FindOneAndUpdateAsync(
            w => w.Id == newListId,
            Builders<WishList>.Update.Push(w => w.Items, item),
            null,
            cancellationToken);

        if (result is null)
        {
            return null;
        }

        result = await _collection.FindOneAndUpdateAsync(
            w => w.Id == listId,
            Builders<WishList>.Update.PullFilter(w => w.Items, i => i.Id == itemId),
            new() { ReturnDocument = ReturnDocument.After },
            cancellationToken);

        return result is not null ? item.Id : null;
    }
}
