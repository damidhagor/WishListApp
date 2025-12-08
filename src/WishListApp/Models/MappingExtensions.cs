using MongoDB.Bson;

namespace WishListApp.Models;

internal static class MappingExtensions
{
    extension(Data.Models.WishList wishList)
    {
        public WishList ToModel() => new(wishList.Id, wishList.OwnerId, wishList.Name, [.. wishList.Items.ToModels(wishList.Id)]);
    }

    extension(IEnumerable<Data.Models.WishList> wishLists)
    {
        public IEnumerable<WishList> ToModels() => wishLists.Select(w => w.ToModel());
    }

    extension(Data.Models.WishListItem item)
    {
        public WishListItem ToModel(ObjectId wishListId)
            => new(
                item.Id,
                wishListId,
                item.Url,
                item.ImageUrl,
                item.SiteName,
                item.Name,
                item.Description,
                item.Price,
                item.Currency,
                item.Note,
                item.Priority,
                item.Purchaser);
    }

    extension(IEnumerable<Data.Models.WishListItem> items)
    {
        public IEnumerable<WishListItem> ToModels(ObjectId wishListId) => items.Select(i => i.ToModel(wishListId));
    }

    extension(WishListItem item)
    {
        public Data.Models.WishListItem ToDataModel()
            => new(
                item.Id,
                item.Url,
                item.ImageUrl,
                item.SiteName,
                item.Name,
                item.Description,
                item.Price,
                item.Currency,
                item.Note,
                item.Priority,
                item.Purchaser);
    }

    extension(IEnumerable<WishListItem> items)
    {
        public IEnumerable<Data.Models.WishListItem> ToDataModels() => items.Select(i => i.ToDataModel());
    }

    extension(Data.Models.WishListShare share)
    {
        public WishListShare ToModel() => new(share.Id, share.WishListId, share.Name, share.AccessKey);
    }

    extension(IEnumerable<Data.Models.WishListShare> shares)
    {
        public IEnumerable<WishListShare> ToModels() => shares.Select(s => s.ToModel());
    }
}
