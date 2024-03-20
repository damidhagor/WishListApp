using System.CommandLine;
using System.CommandLine.Binding;
using Microsoft.EntityFrameworkCore;
using WishlistApp.Data.Sql;

namespace WishlistApp.Cli.Database.Binders;

internal sealed class WishlistDbContextBinder(Option<string> connectionStringOption)
    : BinderBase<WishlistDbContext>
{
    private readonly Option<string> _connectionStringOption = connectionStringOption;

    protected override WishlistDbContext GetBoundValue(BindingContext bindingContext)
        => CreateDbContext(bindingContext);

    private WishlistDbContext CreateDbContext(BindingContext bindingContext)
    {
        var connectionString = bindingContext.ParseResult.GetValueForOption(_connectionStringOption);

        var optionsBuilder = new DbContextOptionsBuilder<WishlistDbContext>()
            .UseNpgsql(connectionString);

        return new WishlistDbContext(optionsBuilder.Options);
    }
}