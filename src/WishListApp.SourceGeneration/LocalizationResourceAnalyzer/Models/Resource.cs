using Microsoft.CodeAnalysis;

namespace WishListApp.SourceGeneration.LocalizationResourceAnalyzer.Models;

internal sealed record Resource(
    string FullPath,
    string Path,
    string Filename,
    string BaseFilename,
    AdditionalText Text)
{
    public Resource(string fullPath, AdditionalText text)
        : this("", "", "", "", text)
    {
        FullPath = fullPath;
        Path = System.IO.Path.GetDirectoryName(fullPath) ?? throw new InvalidOperationException();
        Filename = System.IO.Path.GetFileName(fullPath);

        var dotIndex = Filename.IndexOf('.');
        BaseFilename = dotIndex == -1 ? Filename : Filename.Substring(0, dotIndex);
    }
}
