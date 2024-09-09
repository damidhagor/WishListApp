using Microsoft.CodeAnalysis;

namespace WishListApp.SourceGeneration.LocalizationResourceAnalyzer;

internal static class Rules
{
    private const string _category = "Resource";

    public static readonly DiagnosticDescriptor EntryNotExisting =
        new(
            "LR0001",
            "Resource entry does not exist in other files",
            "The resource entry '{0}' does not exist in resource file '{1}'",
            _category,
            DiagnosticSeverity.Warning,
            true);

    public static readonly DiagnosticDescriptor EmptyName =
        new(
            "LR0002",
            "Resource entry has no name",
            "The resource entry has no name",
            _category,
            DiagnosticSeverity.Error,
            true);

    public static readonly DiagnosticDescriptor EmptyValue =
        new(
            "LR0003",
            "Resource entry has no value",
            "The resource entry '{0}' has no value",
            _category,
            DiagnosticSeverity.Warning,
            true);
}
