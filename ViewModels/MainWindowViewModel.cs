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
    private ContentPack currentPack = new() { Manifest = new Manifest() };

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
            CurrentPack.Manifest.Name = "My Content Pack";
            CurrentPack.Manifest.Author = "Author";
            CurrentPack.Manifest.UniqueID = "YourName.ContentPack";
            CurrentPack.Manifest.Description = "A sample pack";
        }
        catch (Exception ex)
        {
            _logger.Error("初始化失败", ex);
        }
    }

    [RelayCommand]
    private void ExportPack()
    {
        var validation = ContentPackValidator.Validate(CurrentPack);
        if (!validation.IsValid)
        {
            _logger.Warn($"验证失败: {validation.Errors.Count} 项");
            return;
        }

        try
        {
            ContentPackSerializer.SaveToDirectory(CurrentPack, "/workspace/output");
            _logger.Info("导出完成：/workspace/output");
        }
        catch (Exception ex)
        {
            _logger.Error("导出失败", ex);
        }
    }

    [ObservableProperty]
    private string livePreviewJson = string.Empty;

    partial void OnCurrentPackChanged(ContentPack value)
    {
        RefreshPreview();
    }

    private void RefreshPreview()
    {
        try
        {
            LivePreviewJson = _preview.GetPreviewJson(CurrentPack);
        }
        catch (System.Exception ex)
        {
            _logger.Error("预览生成失败", ex);
        }
    }
}