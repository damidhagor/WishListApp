using MongoDB.Bson;
using WishListApp.Data.Migration;

namespace WishListApp.Tests.Data.Migrations;

public sealed class BsonExtensionTests
{
    [Fact]
    public void GetValueFromDocument_Success()
    {
        var document = new BsonDocument
        {
            { "name", "John Doe" },
            { "age", 30 }
        };

        var result = document.GetValueFromDocument("name");

        Assert.True(result.IsT0);
        Assert.True(result.AsT0.IsString);
        Assert.Equal("John Doe", result.AsT0.AsString);
    }

    [Fact]
    public void GetValueFromDocument_InvalidDocument()
    {
        var bsonValue = new BsonInt32(30);

        var result = bsonValue.GetValueFromDocument("name");

        Assert.True(result.IsT2);
        Assert.Equal("'name' field must be an element of a document.", result.AsT2.Error);
    }

    [Fact]
    public void GetValueFromDocument_ElementNotFound()
    {
        var document = new BsonDocument
        {
            { "name", "John Doe" },
            { "age", 30 }
        };

        var result = document.GetValueFromDocument("address");

        Assert.True(result.IsT1);
        Assert.Equal("address", result.AsT1.Name);
    }

    [Fact]
    public void GetObjectIdValue_Success()
    {
        var objectId = ObjectId.GenerateNewId();
        var document = new BsonDocument
        {
            { "_id", objectId },
            { "name", "John Doe" }
        };

        var result = document.GetObjectIdValue("_id");

        Assert.True(result.IsT0);
        Assert.Equal(objectId, result.AsT0);
    }

    [Fact]
    public void GetObjectIdValue_InvalidDocument_NotADocument()
    {
        var bsonValue = new BsonInt32(30);

        var result = bsonValue.GetObjectIdValue("_id");

        Assert.True(result.IsT2);
        Assert.Equal("'_id' field must be an element of a document.", result.AsT2.Error);
    }

    [Fact]
    public void GetObjectIdValue_InvalidDocument_WrongType()
    {
        var document = new BsonDocument
        {
            { "_id", "123" }
        };

        var result = document.GetObjectIdValue("_id");

        Assert.True(result.IsT2);
        Assert.Equal("'_id' field must be an ObjectId.", result.AsT2.Error);
    }

    [Fact]
    public void GetObjectIdValue_ElementNotFound()
    {
        var document = new BsonDocument
        {
            { "name", "John Doe" }
        };

        var result = document.GetObjectIdValue("_id");

        Assert.True(result.IsT1);
        Assert.Equal("_id", result.AsT1.Name);
    }

    [Fact]
    public void GetInt64Value_Success()
    {
        var document = new BsonDocument
        {
            { "_id", ObjectId.GenerateNewId() },
            { "age", 30L }
        };

        var result = document.GetInt64Value("age");

        Assert.True(result.IsT0);
        Assert.Equal(30, result.AsT0);
    }

    [Fact]
    public void GetInt64Value_InvalidDocument_NotADocument()
    {
        var bsonValue = new BsonInt32(30);

        var result = bsonValue.GetInt64Value("age");

        Assert.True(result.IsT2);
        Assert.Equal("'age' field must be an element of a document.", result.AsT2.Error);
    }

    [Fact]
    public void GetInt64Value_InvalidDocument_WrongType()
    {
        var document = new BsonDocument
        {
            { "age", "123" }
        };

        var result = document.GetInt64Value("age");

        Assert.True(result.IsT2);
        Assert.Equal("'age' field must be an Int64.", result.AsT2.Error);
    }

    [Fact]
    public void GetInt64Value_ElementNotFound()
    {
        var document = new BsonDocument
        {
            { "name", "John Doe" }
        };

        var result = document.GetInt64Value("age");

        Assert.True(result.IsT1);
        Assert.Equal("age", result.AsT1.Name);
    }

    [Fact]
    public void GetArrayValue_Success()
    {
        BsonValue[] values = [new BsonInt32(1), new BsonInt32(2), new BsonInt32(3)];
        var document = new BsonDocument
        {
            { "_id", ObjectId.GenerateNewId() },
            { "numbers", new BsonArray(values)}
        };

        var result = document.GetArrayValue("numbers");

        Assert.True(result.IsT0);
        Assert.Equal(values.Length, result.AsT0.Count);
    }

    [Fact]
    public void GetArrayValue_InvalidDocument_NotADocument()
    {
        var bsonValue = new BsonInt32(30);

        var result = bsonValue.GetArrayValue("numbers");

        Assert.True(result.IsT2);
        Assert.Equal("'numbers' field must be an element of a document.", result.AsT2.Error);
    }

    [Fact]
    public void GetArrayValue_InvalidDocument_WrongType()
    {
        var document = new BsonDocument
        {
            { "numbers", "123" }
        };

        var result = document.GetArrayValue("numbers");

        Assert.True(result.IsT2);
        Assert.Equal("'numbers' field must be an Array.", result.AsT2.Error);
    }

    [Fact]
    public void GetArrayValue_ElementNotFound()
    {
        var document = new BsonDocument
        {
            { "name", "John Doe" }
        };

        var result = document.GetArrayValue("numbers");

        Assert.True(result.IsT1);
        Assert.Equal("numbers", result.AsT1.Name);
    }
}
