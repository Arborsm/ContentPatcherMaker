namespace ContentPatcherMaker.Models.Validation;

public sealed class ValidationError
{
    public required string FieldPath { get; init; }
    public required string Message { get; init; }
    public string? Code { get; init; }
}

