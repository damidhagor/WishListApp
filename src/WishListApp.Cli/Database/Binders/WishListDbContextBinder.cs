using System.CommandLine;
using System.CommandLine.Binding;
using Microsoft.EntityFrameworkCore;
using WishListApp.Data.Sql;

namespace WishListApp.Cli.Database.Binders;

internal sealed class WishListDbContextBinder(Option<string> connectionStringOption)
    : BinderBase<WishListDbContext>
{
    private readonly Option<string> _connectionStringOption = connectionStringOption;

    protected override WishListDbContext GetBoundValue(BindingContext bindingContext)
        => CreateDbContext(bindingContext);

    private WishListDbContext CreateDbContext(BindingContext bindingContext)
    {
        var connectionString = bindingContext.ParseResult.GetValueForOption(_connectionStringOption);

        var optionsBuilder = new DbContextOptionsBuilder<WishListDbContext>()
            .UseNpgsql(connectionString);

        return new WishListDbContext(optionsBuilder.Options);
    }
}
