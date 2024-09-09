using System.Collections.Immutable;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using WishListApp.SourceGeneration.LocalizationResourceAnalyzer.Models;

namespace WishListApp.SourceGeneration.LocalizationResourceAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class LocalizationResourceAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => [Rules.EntryNotExisting, Rules.EmptyName, Rules.EmptyValue];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterAdditionalFileAction(context =>
        {
            if (!context.AdditionalFile.Path.EndsWith(".resx", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var resource = new Resource(context.AdditionalFile.Path, context.AdditionalFile);
            var otherResources = context.Options.AdditionalFiles
                .Where(f => f.Path.EndsWith(".resx", StringComparison.OrdinalIgnoreCase))
                .Select(f => new Resource(f.Path, f))
                .Where(r => r.FullPath != resource.FullPath)
                .ToImmutableArray();

            var resourceValuesByName = GetValuesFromResource(context, resource);
            foreach (var otherResource in otherResources)
            {
                var otherResourceValuesByName = GetValuesFromResource(context, otherResource);
                foreach (var nameAndValue in resourceValuesByName)
                {
                    if (!otherResourceValuesByName.ContainsKey(nameAndValue.Key))
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                Rules.EntryNotExisting,
                                Location.None,
                                nameAndValue.Value.Name,
                                otherResource.FullPath));
                    }
                }
            }
        });
    }

    private ImmutableDictionary<string, (string Name, string Value)> GetValuesFromResource(
        AdditionalFileAnalysisContext context,
        Resource resource)
    {
        return XDocument
            .Load(resource.FullPath, LoadOptions.SetLineInfo)
            .Descendants("data")
            .Select(d => (d.Attribute("name")?.Value, d.Element("value")?.Value))
            .Where(nameAndValue =>
            {
                var (name, value) = nameAndValue;
                if (string.IsNullOrEmpty(name))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            Rules.EmptyName,
                            Location.None));
                    return false;
                }
                else if (string.IsNullOrEmpty(value))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            Rules.EmptyValue,
                            Location.None,
                            name));
                    return false;
                }

                return true;
            })
            .Select(nameAndValue => (nameAndValue.Item1!, nameAndValue.Item2!))
            .GroupBy(nameAndValue => nameAndValue.Item1)
            .Where(valueByName => valueByName.Count() == 1)
            .ToImmutableDictionary(valueByName => valueByName.Key, valueByName => valueByName.First());
    }
}
