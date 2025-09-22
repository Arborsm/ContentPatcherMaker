using System.Collections.Generic;
using ContentPatcherMaker.Core.Models;
using Newtonsoft.Json;

namespace ContentPatcherMaker.Core.Serialization;

public static class ContentPatcherSerializer
{
    public static string SerializeContentJson(IEnumerable<ContentPatcherChange> changes)
    {
        var obj = new Dictionary<string, object>
        {
            ["Format"] = "1.30.0",
            ["Changes"] = changes
        };
        var json = JsonConvert.SerializeObject(obj, Formatting.Indented,
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        return json;
    }
}

