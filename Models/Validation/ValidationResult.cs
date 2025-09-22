using System.Collections.Generic;

namespace ContentPatcherMaker.Models.Validation;

public sealed class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<ValidationError> Errors { get; } = new();
}

