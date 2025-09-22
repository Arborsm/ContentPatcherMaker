using System.Collections.Generic;

namespace ContentPatcherMaker.Services.Docs;

public record DocEntry(string Title, string Path);

public interface IDocsIndexService
{
    IReadOnlyList<DocEntry> Entries { get; }
    void ScanDirectory(string directoryPath);
}

