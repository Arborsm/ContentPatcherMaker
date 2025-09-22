using System;

namespace ContentPatcherMaker.Services.Abstractions;

public interface IExceptionHandler
{
    void Handle(Exception ex, string? context = null);
}

