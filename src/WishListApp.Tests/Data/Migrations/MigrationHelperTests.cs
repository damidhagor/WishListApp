using MongoDB.Bson;
using WishListApp.Data.Migration.Migrations;

namespace WishListApp.Tests.Data.Migrations;

public sealed class MigrationHelperTests
{
    [Fact]
    public void ValidateVersion_Success()
    {
        var document = new BsonDocument
        {
            { "name", "John Doe" },
            { "Version", 1u }
        };

        var result = document.ValidateVersion(1u);

        Assert.True(result.IsT0);
    }

    [Fact]
    public void ValidateVersion_VersionNotFound_Success()
    {
        var document = new BsonDocument
        {
            { "name", "John Doe" }
        };

        var result = document.ValidateVersion(0u);

        Assert.True(result.IsT0);
    }

    [Fact]
    public void ValidateVersion_VersionNotFound_InvalidVersion()
    {
        var document = new BsonDocument
        {
            { "name", "John Doe" }
        };

        var result = document.ValidateVersion(1u);

        Assert.True(result.IsT1);
        Assert.Equal(1u, result.AsT1.SupportedVersion);
        Assert.Equal(0u, result.AsT1.Version);
    }

    [Theory]
    [InlineData(0u)]
    [InlineData(2u)]
    public void ValidateVersion_InvalidVersion(uint supportedVersion)
    {
        var document = new BsonDocument
        {
            { "name", "John Doe" },
            { "Version", 1u }
        };

        var result = document.ValidateVersion(supportedVersion);

        Assert.True(result.IsT1);
        Assert.Equal(supportedVersion, result.AsT1.SupportedVersion);
        Assert.Equal(1u, result.AsT1.Version);
    }

    [Fact]
    public void ValidateVersion_InvalidDocument_WrongType()
    {
        var document = new BsonDocument
        {
            { "name", "John Doe" },
            { "Version", "1" }
        };

        var result = document.ValidateVersion(1u);

        Assert.True(result.IsT2);
        Assert.Equal("'Version' field must be an Int64.", result.AsT2.Error);
    }

    [Fact]
    public void ValidateVersion_InvalidDocument_NotPositive()
    {
        var document = new BsonDocument
        {
            { "name", "John Doe" },
            { "Version", -1L }
        };

        var result = document.ValidateVersion(1u);

        Assert.True(result.IsT2);
        Assert.Equal("'Version' field must be a positive integer.", result.AsT2.Error);
    }

    [Fact]
    public void SetVersion_SetNewField()
    {
        var document = new BsonDocument
        {
            { "name", "John Doe" }
        };

        document.SetVersion(1u);

        Assert.True(document.Contains("Version"));
        Assert.True(document["Version"].IsInt64);
        Assert.Equal(1, document["Version"].AsInt64);
    }

    [Fact]
    public void SetVersion_UpdateExistingField()
    {
        var document = new BsonDocument
        {
            { "name", "John Doe" },
            { "Version", 1L }
        };

        document.SetVersion(3u);

        Assert.True(document.Contains("Version"));
        Assert.True(document["Version"].IsInt64);
        Assert.Equal(3, document["Version"].AsInt64);
    }
}
