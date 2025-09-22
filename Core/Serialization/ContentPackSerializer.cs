using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ContentPatcherMaker.Core.Models;

namespace ContentPatcherMaker.Core.Serialization;

public static class ContentPackSerializer
{
    public static string ToManifestJson(Manifest manifest)
        => JsonConvert.SerializeObject(manifest, Formatting.Indented);

    public static string ToContentJson(IEnumerable<Patch> patches)
    {
        var jarr = JArray.FromObject(patches);
        var root = new JObject
        {
            ["Changes"] = jarr
        };
        return root.ToString(Formatting.Indented);
    }

    public static void SaveToDirectory(ContentPack pack, string directory)
    {
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, "manifest.json"), ToManifestJson(pack.Manifest));
        File.WriteAllText(Path.Combine(directory, "content.json"), ToContentJson(pack.Changes));
    }
}

