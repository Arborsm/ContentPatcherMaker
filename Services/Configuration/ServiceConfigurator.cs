using System;
using System.IO;
using ContentPatcherMaker.Services.Docs;
using ContentPatcherMaker.Services.Localization;
using ContentPatcherMaker.Services.Plugins;
using ContentPatcherMaker.Services.Preview;
using Microsoft.Extensions.DependencyInjection;

namespace ContentPatcherMaker.Services.Configuration;

public static class ServiceConfigurator
{
    public static IServiceProvider Build()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ILocalizationService, JsonLocalizationService>();
        services.AddSingleton<IDocsIndexService, MarkdownDocsIndexService>();
        services.AddSingleton<IPreviewService, ImagePreviewService>();
        services.AddSingleton<PluginLoader>();

        var provider = services.BuildServiceProvider();

        // Initialize services with data directories
        var loc = provider.GetRequiredService<ILocalizationService>();
        var baseDir = AppContext.BaseDirectory;
        var localesDir = Path.Combine(baseDir, "Resources", "Locales");
        loc.LoadLanguagesFromDirectory(localesDir);

        var docs = provider.GetRequiredService<IDocsIndexService>();
        docs.ScanDirectory(Path.Combine(baseDir, "md"));

        var loader = provider.GetRequiredService<PluginLoader>();
        loader.LoadPlugins(Path.Combine(baseDir, "Plugins"));

        return provider;
    }
}

