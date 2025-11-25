using WishListApp.Data.Migration.Results;

namespace WishListApp.Data.Migration;

internal static class BsonExtensions
{
    extension(BsonValue value)
    {
        public OneOf<ObjectId, ElementNotFound, InvalidDocument> GetObjectIdValue(string name)
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

        public OneOf<long, ElementNotFound, InvalidDocument> GetInt64Value(string name)
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

        public OneOf<BsonArray, ElementNotFound, InvalidDocument> GetArrayValue(string name)
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

        public OneOf<BsonValue, ElementNotFound, InvalidDocument> GetValueFromDocument(string name)
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
}
