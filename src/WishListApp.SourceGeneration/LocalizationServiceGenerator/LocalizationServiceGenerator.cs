using Microsoft.CodeAnalysis;
using WishListApp.SourceGeneration.LocalizationServiceGenerator.Attributes;
using WishListApp.SourceGeneration.LocalizationServiceGenerator.Generators;
using WishListApp.SourceGeneration.LocalizationServiceGenerator.Providers;

namespace WishListApp.SourceGeneration.LocalizationServiceGenerator;

[Generator]
public sealed class LocalizationServiceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(
            static context => context
                .AddSource(LocalizationServiceAttribute.SourceHint, LocalizationServiceAttribute.Source));

        var resources = context.AdditionalTextsProvider.GetResources();
        var services = context.SyntaxProvider
            .GetServices()
            .AddResources(resources);

        context.RegisterSourceOutput(services, InterfaceGenerator.GenerateLocalizationServiceInterface);
        context.RegisterSourceOutput(services, ClassGenerator.GenerateLocalizationServiceClass);
    }
}
