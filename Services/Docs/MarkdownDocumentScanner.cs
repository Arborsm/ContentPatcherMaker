using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ContentPatcherMaker.Services.Abstractions;

namespace ContentPatcherMaker.Services.Docs;

public sealed class MarkdownDocumentScanner : IDocumentScanner
{
    private static readonly Regex HeaderRegex = new("^#{1,6}\\s+(?<title>.+)$", RegexOptions.Multiline);

    public IReadOnlyDictionary<string, string> BuildKnowledgeIndex(string mdRootDirectory)
    {
        var index = new Dictionary<string, string>();
        if (!Directory.Exists(mdRootDirectory))
        {
            return index;
        }

        foreach (var file in Directory.EnumerateFiles(mdRootDirectory, "*.md", SearchOption.AllDirectories))
        {
            string content = SafeReadAllText(file);
            string title = ExtractTitle(content) ?? Path.GetFileNameWithoutExtension(file);
            index[title] = content;
        }

        return index;
    }

    private static string? ExtractTitle(string content)
    {
        var match = HeaderRegex.Match(content);
        return match.Success ? match.Groups["title"].Value.Trim() : null;
    }

    private static string SafeReadAllText(string file)
    {
        try
        {
            return File.ReadAllText(file, Encoding.UTF8);
        }
        catch
        {
            return string.Empty;
        }
    }
}

