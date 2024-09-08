using System.Collections.Immutable;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using WishListApp.SourceGeneration.LocalizationServiceGenerator.Models;

namespace WishListApp.SourceGeneration.LocalizationServiceGenerator.Providers;

internal static class AdditionalTextProviderExtensions
{
    public static IncrementalValueProvider<ImmutableArray<Resource>> GetResources(this IncrementalValuesProvider<AdditionalText> provider)
        => provider
            .Where(t => t.Path.EndsWith(".resx"))
            .Select((text, cancellationToken) =>
            {
                try
                {
                    var xmlText = text.GetText()?.ToString();
                    if (xmlText is null)
                    {
                        return new Resource(text.Path, [], "File could not be read.");
                    }

                    var names = XDocument.Parse(xmlText)
                        .Descendants("data")
                        .Attributes("name")
                        .Select(a => a.Value)
                        .ToArray();

                    return new Resource(text.Path, names, null);
                }
                catch (Exception ex)
                {
                    return new Resource(text.Path, [], ex.Message);
                }
            })
            .Collect();
}
