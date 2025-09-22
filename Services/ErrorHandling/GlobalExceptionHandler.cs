using ContentPatcherMaker.Services.Abstractions;

namespace ContentPatcherMaker.Services.ErrorHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IAppLogger _logger;

    public GlobalExceptionHandler(IAppLogger logger)
    {
        _logger = logger;
    }

    public void Handle(System.Exception ex, string? context = null)
    {
        _logger.Error($"Unhandled exception{(context is null ? string.Empty : $" in {context}")}: {ex.Message}", ex);
    }
}

