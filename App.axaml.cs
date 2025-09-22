using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using ContentPatcherMaker.ViewModels;
using ContentPatcherMaker.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ContentPatcherMaker.Views;

namespace ContentPatcherMaker;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);
            var provider = ServiceConfigurator.Build();
            desktop.MainWindow = new MainWindow
            {
                DataContext = ActivatorUtilities.CreateInstance<MainWindowViewModel>(provider),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}