using ContentPatcherMaker.Core.Models;
using ContentPatcherMaker.Core.Serialization;
using ContentPatcherMaker.Services.Abstractions;

namespace ContentPatcherMaker.Services.Preview;

public sealed class JsonPreviewService : IPreviewService
{
    public string GetPreviewJson(ContentPack pack)
    {
        return ContentPackSerializer.ToContentJson(pack.Changes);
    }
}

