using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ContentPatcherMaker.Core.Models;
using ContentPatcherMaker.Models.Validation;

namespace ContentPatcherMaker.Core.Validation;

public static class ContentPackValidator
{
    public static ValidationResult Validate(ContentPack pack)
    {
        var result = new ValidationResult();

        ValidateObject(pack.Manifest, "Manifest", result.Errors);

        if (pack.Changes.Count == 0)
        {
            result.Errors.Add(new ValidationError { FieldPath = "Changes", Message = "至少需要一个 Patch" });
        }

        for (int i = 0; i < pack.Changes.Count; i++)
        {
            ValidateObject(pack.Changes[i], $"Changes[{i}]", result.Errors);
        }

        // 兼容性规则
        CompatibilityRules.Apply(pack, result);

        return result;
    }

    private static void ValidateObject(object obj, string path, List<ValidationError> errors)
    {
        var ctx = new ValidationContext(obj);
        var list = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        if (!Validator.TryValidateObject(obj, ctx, list, true))
        {
            foreach (var e in list)
            {
                var member = e.MemberNames is not null ? string.Join(',', e.MemberNames) : string.Empty;
                errors.Add(new ValidationError { FieldPath = string.IsNullOrEmpty(member) ? path : $"{path}.{member}", Message = e.ErrorMessage ?? "无效值" });
            }
        }
    }
}

