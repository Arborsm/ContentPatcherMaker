using System.Collections.Generic;

namespace ContentPatcherMaker.Services.Validation;

public record ValidationIssue(string Code, string Message, string? Path = null, bool IsError = false);

public interface IValidationService
{
    IEnumerable<ValidationIssue> ValidateAll();
}

