using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ContentPatcherMaker.Core.Models;

namespace ContentPatcherMaker.Services.Validation;

public class ContentPatcherValidationService : IValidationService
{
    private readonly ContentPatcherPackage _package;

    public ContentPatcherValidationService(ContentPatcherPackage package)
    {
        _package = package;
    }

    public IEnumerable<ValidationIssue> ValidateAll()
    {
        var issues = new List<ValidationIssue>();

        // Validate manifest
        var context = new ValidationContext(_package.Manifest);
        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        if (!Validator.TryValidateObject(_package.Manifest, context, results, true))
        {
            foreach (var r in results)
            {
                issues.Add(new ValidationIssue("ManifestValidation", r.ErrorMessage ?? "Manifest error", string.Join(',', r.MemberNames), true));
            }
        }

        // Validate changes basic fields
        for (var i = 0; i < _package.Changes.Count; i++)
        {
            var ch = _package.Changes[i];
            if (string.IsNullOrWhiteSpace(ch.Action))
            {
                issues.Add(new ValidationIssue("ChangeMissingAction", "Change action is required", $"Changes[{i}].Action", true));
            }
        }

        // Rudimentary compatibility checks (example)
        if (string.IsNullOrWhiteSpace(_package.Manifest.MinimumApiVersion))
        {
            issues.Add(new ValidationIssue("MissingMinimumApiVersion", "MinimumApiVersion not set - compatibility unknown"));
        }

        return issues;
    }
}

