using System;
using System.Collections.Concurrent;
using ContentPatcherMaker.Services.Abstractions;

namespace ContentPatcherMaker.Services.Plugins;

public sealed class ParameterEditorFactory : IParameterEditorFactory
{
    private readonly ConcurrentDictionary<string, IParameterTypeProvider> _providers = new(StringComparer.OrdinalIgnoreCase);

    public object CreateEditorViewModel(string typeName)
    {
        if (!_providers.TryGetValue(typeName, out var provider))
        {
            throw new InvalidOperationException($"未知的参数类型: {typeName}");
        }
        return Activator.CreateInstance(provider.EditorViewModelType)!;
    }

    public void Register(IParameterTypeProvider provider)
    {
        _providers[provider.TypeName] = provider;
    }
}

