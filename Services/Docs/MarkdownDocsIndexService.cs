using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ContentPatcherMaker.Services.Docs;

public class MarkdownDocsIndexService : IDocsIndexService
{
    private readonly List<DocEntry> _entries = new();
    public IReadOnlyList<DocEntry> Entries => _entries;

    public void ScanDirectory(string directoryPath)
    {
        _entries.Clear();
        if (!Directory.Exists(directoryPath))
        {
            return;
        }

        var files = Directory.EnumerateFiles(directoryPath, "*.md", SearchOption.AllDirectories)
            .OrderBy(p => p)
            .ToList();
        foreach (var file in files)
        {
            var title = Path.GetFileNameWithoutExtension(file);
            _entries.Add(new DocEntry(title, file));
        }
    }
}

