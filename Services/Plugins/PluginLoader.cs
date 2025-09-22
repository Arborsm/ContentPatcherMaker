using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ContentPatcherMaker.Services.Plugins;

public class PluginLoader : IPluginRegistry
{
    private readonly Dictionary<string, Type> _parameterEditors = new();
    public IReadOnlyDictionary<string, Type> ParameterEditors => _parameterEditors;

    public void RegisterParameterEditor(string typeKey, Type editorControlType)
    {
        _parameterEditors[typeKey] = editorControlType;
    }

    public IReadOnlyList<IPlugin> LoadPlugins(string pluginsDirectory)
    {
        var result = new List<IPlugin>();

        if (!Directory.Exists(pluginsDirectory))
        {
            Directory.CreateDirectory(pluginsDirectory);
            return result;
        }

        foreach (var dll in Directory.EnumerateFiles(pluginsDirectory, "*.dll", SearchOption.TopDirectoryOnly))
        {
            try
            {
                var asm = Assembly.LoadFrom(dll);
                var types = asm.GetTypes()
                    .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);
                foreach (var t in types)
                {
                    if (Activator.CreateInstance(t) is IPlugin plugin)
                    {
                        plugin.Register(this);
                        result.Add(plugin);
                    }
                }
            }
            catch
            {
                // ignore bad assemblies
            }
        }

        return result;
    }
}

