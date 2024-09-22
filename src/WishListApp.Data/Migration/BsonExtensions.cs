using WishListApp.Data.Migration.Results;

namespace WishListApp.Data.Migration;

internal static class BsonExtensions
{
    public static OneOf<ObjectId, ElementNotFound, InvalidDocument> GetObjectIdValue(this BsonValue value, string name)
    {
        var fieldValueResult = value.GetValueFromDocument(name);

        if (!fieldValueResult.TryPickT0(out var fieldValue, out var errorResults))
        {
            return errorResults.TryPickT0(out var notFound, out var invalidDocument)
                ? notFound
                : invalidDocument;
        }

        return fieldValue.IsObjectId
            ? fieldValue.AsObjectId
            : new InvalidDocument($"'{name}' field must be an ObjectId.");
    }

    public static OneOf<long, ElementNotFound, InvalidDocument> GetInt64Value(this BsonValue value, string name)
    {
        var fieldValueResult = value.GetValueFromDocument(name);

        if (!fieldValueResult.TryPickT0(out var fieldValue, out var errorResults))
        {
            return errorResults.TryPickT0(out var notFound, out var invalidDocument)
                ? notFound
                : invalidDocument;
        }

        return fieldValue.IsInt64
            ? fieldValue.AsInt64
            : new InvalidDocument($"'{name}' field must be an Int64.");
    }

    public static OneOf<BsonArray, ElementNotFound, InvalidDocument> GetArrayValue(this BsonValue value, string name)
    {
        var fieldValueResult = value.GetValueFromDocument(name);

        if (!fieldValueResult.TryPickT0(out var fieldValue, out var errorResults))
        {
            return errorResults.TryPickT0(out var notFound, out var invalidDocument)
                ? notFound
                : invalidDocument;
        }

        return fieldValue.IsBsonArray
            ? fieldValue.AsBsonArray
            : new InvalidDocument($"'{name}' field must be an Array.");
    }

    public static OneOf<BsonValue, ElementNotFound, InvalidDocument> GetValueFromDocument(this BsonValue value, string name)
    {
        if (!value.IsBsonDocument)
        {
            return new InvalidDocument($"'{name}' field must be an element of a document.");
        }

        return !value.AsBsonDocument.TryGetValue(name, out var fieldValue)
            ? new ElementNotFound(name)
            : fieldValue;
    }
}
