using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using ContentPatcherMaker.ViewModels;
using ContentPatcherMaker.Views;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ContentPatcherMaker;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = default!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainWindowViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // ViewModels
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<WorkflowViewModel>();

        // 基础服务
        services.AddSingleton<Services.Abstractions.IAppLogger, Services.Logging.SimpleConsoleLogger>();
        services.AddSingleton<Services.Abstractions.IExceptionHandler, Services.ErrorHandling.GlobalExceptionHandler>();
        services.AddSingleton<Services.Abstractions.IAppConfiguration, Services.Configuration.InMemoryAppConfiguration>();
        services.AddSingleton<Services.Abstractions.ILocalizationService, Services.Localization.AvaloniaLocalizationService>();
        services.AddSingleton<Services.Abstractions.IPreviewService, Services.Preview.JsonPreviewService>();
        services.AddSingleton<Services.Abstractions.IParameterEditorFactory, Services.Plugins.ParameterEditorFactory>();

        // TODO: 注册 i18n、文档扫描等服务
        services.AddSingleton<Services.Abstractions.IDocumentScanner, Services.Docs.MarkdownDocumentScanner>();
    }
}