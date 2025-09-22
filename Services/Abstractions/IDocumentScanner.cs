using System.Collections.Generic;

namespace ContentPatcherMaker.Services.Abstractions;

public interface IDocumentScanner
{
    IReadOnlyDictionary<string, string> BuildKnowledgeIndex(string mdRootDirectory);
}

