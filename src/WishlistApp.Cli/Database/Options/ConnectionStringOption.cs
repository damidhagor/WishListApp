using System.CommandLine;

namespace WishlistApp.Cli.Database.Options;

internal static class ConnectionStringOption
{
    public static Option<string> AddConnectionStringOption(this Command command)
    {
        var option = new Option<string>(
            aliases: ["--connection-string", "-c"],
            description: "Connection string to the Wishlist database")
        {
            IsRequired = true,
        };

        command.AddGlobalOption(option);
        return option;
    }
}