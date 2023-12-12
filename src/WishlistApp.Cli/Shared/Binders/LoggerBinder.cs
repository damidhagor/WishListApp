using System.CommandLine.Binding;
using Microsoft.Extensions.Logging;

namespace WishlistApp.Cli.Shared.Binders;

internal sealed class LoggerBinder : BinderBase<ILogger>
{
    protected override ILogger GetBoundValue(BindingContext bindingContext)
        => CreateLogger(bindingContext);

    private static ILogger CreateLogger(BindingContext bindingContext)
    {
        var commandName = bindingContext.ParseResult.CommandResult.Command.Name;

        using ILoggerFactory loggerFactory = LoggerFactory.Create(
            builder => builder.AddConsole());

        return loggerFactory.CreateLogger(commandName);
    }
}
