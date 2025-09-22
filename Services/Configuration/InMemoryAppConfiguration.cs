using ContentPatcherMaker.Services.Abstractions;

namespace ContentPatcherMaker.Services.Configuration;

public sealed class InMemoryAppConfiguration : IAppConfiguration
{
    public string? CurrentLanguage { get; set; } = "zh-CN";
}

