using ContentPatcherMaker.Core.Models;

namespace ContentPatcherMaker.Services.Abstractions;

public interface IPreviewService
{
    string GetPreviewJson(ContentPack pack);
}

