using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ContentPatcherMaker.Services.Abstractions;
using ContentPatcherMaker.Core.Models;
using ContentPatcherMaker.Core.Serialization;
using ContentPatcherMaker.Core.Validation;

namespace ContentPatcherMaker.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IAppLogger _logger;
    private readonly IDocumentScanner _scanner;
    private readonly ILocalizationService _localization;
    private readonly IPreviewService _preview;

    [ObservableProperty]
    private string greeting = "Welcome to Avalonia!";

    [ObservableProperty]
    private string currentLanguage = "zh-CN";

    [ObservableProperty]
    private IDictionary<string, string> knowledgeIndex = new Dictionary<string, string>();

    [ObservableProperty]
    private WorkflowViewModel workflow = default!;

    public MainWindowViewModel(IAppLogger logger, IDocumentScanner scanner, IAppConfiguration config, ILocalizationService localization, IPreviewService preview)
    {
        _logger = logger;
        _scanner = scanner;
        _localization = localization;
        _preview = preview;
        CurrentLanguage = config.CurrentLanguage ?? "zh-CN";
        Initialize();
    }

    private void Initialize()
    {
        try
        {
            Greeting = CurrentLanguage.StartsWith("zh", StringComparison.OrdinalIgnoreCase)
                ? "欢迎使用 ContentPatcherMaker!"
                : "Welcome to ContentPatcherMaker!";

            KnowledgeIndex = (IDictionary<string, string>)_scanner.BuildKnowledgeIndex("/workspace/md");
            Workflow = App.Services.GetRequiredService<WorkflowViewModel>();
        }
        catch (Exception ex)
        {
            _logger.Error("初始化失败", ex);
        }
    }

    // 导出与预览已迁移到工作流VM
}