using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using NSubstitute;
using WishListApp.Data.Accessors;
using WishListApp.Data.Migration;
using WishListApp.Data.Migration.Migrations;
using WishListApp.Data.Migration.Services;
using WishListApp.Data.Models;

namespace WishListApp.Tests.Data.MigrationExecutor;

public sealed class ValidateMigrationVersionTests
{
    [Fact]
    public void ValidateMigrationVersions_WishList_Success()
    {
        var serviceProvider = new ServiceCollection()
            .AddWishListMigrations()
            .BuildServiceProvider();

        var collection = Substitute.For<IMongoCollection<BsonDocument>>();
        var messenger = Substitute.For<IMessenger>();
        var migrations = serviceProvider.GetServices<IMigration<WishList>>();

        var executor = new MigrationExecutor<WishList>(collection, migrations, messenger);

        var result = MigrationExecutorAccessors<WishList>.ValidateMigrationVersions(executor);

        Assert.True(result.IsT0);
    }

    [Fact]
    public void ValidateMigrationVersions_WishListShare_Success()
    {
        var serviceProvider = new ServiceCollection()
            .AddWishListShareMigrations()
            .BuildServiceProvider();

        var collection = Substitute.For<IMongoCollection<BsonDocument>>();
        var messenger = Substitute.For<IMessenger>();
        var migrations = serviceProvider.GetServices<IMigration<WishListShare>>();

        var executor = new MigrationExecutor<WishListShare>(collection, migrations, messenger);

        var result = MigrationExecutorAccessors<WishListShare>.ValidateMigrationVersions(executor);

        Assert.True(result.IsT0);
    }

    [Fact]
    public void ValidateMigrationVersions_NotStartingAtZero()
    {
        var collection = Substitute.For<IMongoCollection<BsonDocument>>();
        var messenger = Substitute.For<IMessenger>();

        IMigration<WishList>[] migrations = [Substitute.For<IMigration<WishList>>(), Substitute.For<IMigration<WishList>>()];
        migrations[0].SupportedVersion.Returns(1u);
        migrations[1].SupportedVersion.Returns(2u);

        var executor = new MigrationExecutor<WishList>(collection, migrations, messenger);

        var result = MigrationExecutorAccessors<WishList>.ValidateMigrationVersions(executor);

        Assert.True(result.IsT1);
        Assert.Equal(0u, result.AsT1.ExpectedVersion);
        Assert.Equal(1u, result.AsT1.SupportedVersion);
    }

    [Fact]
    public void ValidateMigrationVersions_NotContinuous()
    {
        var collection = Substitute.For<IMongoCollection<BsonDocument>>();
        var messenger = Substitute.For<IMessenger>();

        IMigration<WishList>[] migrations = [Substitute.For<IMigration<WishList>>(), Substitute.For<IMigration<WishList>>()];
        migrations[0].SupportedVersion.Returns(0u);
        migrations[1].SupportedVersion.Returns(2u);

        var executor = new MigrationExecutor<WishList>(collection, migrations, messenger);

        var result = MigrationExecutorAccessors<WishList>.ValidateMigrationVersions(executor);

        Assert.True(result.IsT1);
        Assert.Equal(1u, result.AsT1.ExpectedVersion);
        Assert.Equal(2u, result.AsT1.SupportedVersion);
    }
}
