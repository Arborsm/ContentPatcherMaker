using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace ContentPatcherMaker.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (Application.Current is ContentPatcherMaker.App app)
            {
                var handler = ContentPatcherMaker.App.Services.GetService(typeof(ContentPatcherMaker.Services.Abstractions.IExceptionHandler)) as ContentPatcherMaker.Services.Abstractions.IExceptionHandler;
                handler?.Handle(e.ExceptionObject as Exception ?? new Exception("Unknown exception"), "AppDomain.UnhandledException");
            }
        };
    }
}