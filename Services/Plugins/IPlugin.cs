using System;

namespace ContentPatcherMaker.Services.Plugins;

public interface IPlugin
{
    string Id { get; }
    string Name { get; }
    Version Version { get; }
    void Register(IPluginRegistry registry);
}

public interface IPluginRegistry
{
    void RegisterParameterEditor(string typeKey, Type editorControlType);
}

