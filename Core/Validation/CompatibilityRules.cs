using System.Text.RegularExpressions;
using ContentPatcherMaker.Core.Models;
using ContentPatcherMaker.Models.Validation;

namespace ContentPatcherMaker.Core.Validation;

public static class CompatibilityRules
{
    private static readonly Regex UniqueIdRegex = new("^[A-Za-z0-9_.-]+\\.[A-Za-z0-9_.-]+$", RegexOptions.Compiled);

    public static void Apply(ContentPack pack, ValidationResult result)
    {
        // UniqueID: recommend Namespace.ModName format
        if (!UniqueIdRegex.IsMatch(pack.Manifest.UniqueID))
        {
            result.Errors.Add(new ValidationError
            {
                FieldPath = "Manifest.UniqueID",
                Message = "UniqueID 建议为 Namespace.ModName 形式（仅字母数字._-）"
            });
        }

        // ContentPatcher target
        if (!string.Equals(pack.Manifest.ContentPackFor, "Pathoschild.ContentPatcher", System.StringComparison.Ordinal))
        {
            result.Errors.Add(new ValidationError
            {
                FieldPath = "Manifest.ContentPackFor",
                Message = "ContentPackFor 必须为 Pathoschild.ContentPatcher"
            });
        }

        // Patches: basic semantic checks
        for (int i = 0; i < pack.Changes.Count; i++)
        {
            var p = pack.Changes[i];
            if (string.Equals(p.Action, "EditData", System.StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(p.Target))
            {
                result.Errors.Add(new ValidationError
                {
                    FieldPath = $"Changes[{i}].Target",
                    Message = "EditData 需要指定 Target"
                });
            }
        }
    }
}

