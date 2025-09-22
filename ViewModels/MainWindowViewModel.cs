using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ContentPatcherMaker.Core.Models;
using ContentPatcherMaker.Core.Serialization;
using ContentPatcherMaker.Services.Docs;
using ContentPatcherMaker.Services.Localization;
using ContentPatcherMaker.Services.Preview;
using ContentPatcherMaker.Services.Validation;
using ContentPatcherMaker.ViewModels.Workflow;

namespace ContentPatcherMaker.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ILocalizationService _loc;
    public LocalizationIndex LocIndex { get; }
    private readonly IDocsIndexService _docs;
    private readonly IPreviewService _preview;
    private readonly ContentPatcherPackage _package = new();
    private readonly ObservableCollection<ContentPatcherChange> _changes = new();
    public WorkflowEditorViewModel WorkflowVM { get; } = new();

    public MainWindowViewModel(ILocalizationService loc, IDocsIndexService docs, IPreviewService preview)
    {
        _loc = loc;
        _docs = docs;
        _preview = preview;
        _preview.PreviewUpdated += (_, _) => OnPropertyChanged(nameof(PreviewBitmap));

        Languages = new ObservableCollection<string>(_loc.AvailableLanguages);
        LocIndex = new LocalizationIndex(_loc);

        Changes = new ObservableCollection<ContentPatcherChange>(_changes);
        JsonText = ContentPatcherSerializer.SerializeContentJson(Changes);
        JsonTree = JsonTreeBuilder.Build(JsonText);

        DocsEntries = new ObservableCollection<DocEntry>(_docs.Entries);
        WindowTitle = _loc.T("app.title", "ContentPatcher Editor");
        StatusText = _loc.T("status.ready", "Ready");
    }

    public string WindowTitle => _loc.T("app.title", "ContentPatcher Editor");

    public ObservableCollection<string> Languages { get; }

    public string this[string key] => _loc.T(key);

    public string StatusText { get; }

    [ObservableProperty]
    private string? jsonText;

    [ObservableProperty]
    private ObservableCollection<object>? jsonTree;

    [ObservableProperty]
    private ObservableCollection<ContentPatcherChange> changes = new();

    [ObservableProperty]
    private ObservableCollection<DocEntry> docsEntries = new();

    [ObservableProperty]
    private DocEntry? selectedDoc;

    public string SelectedDocText
    {
        get
        {
            if (SelectedDoc is null)
                return string.Empty;
            try
            {
                return File.ReadAllText(SelectedDoc.Path);
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    public Bitmap? PreviewBitmap => _preview.CurrentBitmap;

    public IReadOnlyDictionary<string, string> Loc => new ReadOnlyDictionary<string, string>(
        _loc.AvailableLanguages.ToDictionary(k => k, v => v));

    [RelayCommand]
    private void AddChange()
    {
        var change = new ContentPatcherChange { Action = "EditImage" };
        _changes.Add(change);
        Changes = new ObservableCollection<ContentPatcherChange>(_changes);
        JsonText = ContentPatcherSerializer.SerializeContentJson(Changes);
        JsonTree = JsonTreeBuilder.Build(JsonText);
    }

    [RelayCommand]
    private void RunValidation()
    {
        var v = new ContentPatcherValidationService(new ContentPatcherPackage
        {
            Manifest = _package.Manifest,
            Changes = _changes.ToList()
        });

        ValidationIssues = new ObservableCollection<ValidationIssue>(v.ValidateAll());
    }

    [ObservableProperty]
    private ObservableCollection<ValidationIssue> validationIssues = new();

    public ObservableCollection<string> AvailableLanguages => Languages;

    [RelayCommand]
    private void ChangeLanguage(string lang)
    {
        try
        {
            _loc.SetLanguage(lang);
            OnPropertyChanged(nameof(WindowTitle));
            LocIndex.Refresh();
        }
        catch
        {
            // ignore
        }
    }

    partial void OnSelectedDocChanged(DocEntry? value)
    {
        OnPropertyChanged(nameof(SelectedDocText));
    }

    [RelayCommand]
    private void Open()
    {
        // TODO: implement open
    }

    [RelayCommand]
    private void Save()
    {
        // TODO: implement save
    }

    [RelayCommand]
    private void Export()
    {
        var json = ContentPatcherSerializer.SerializeContentJson(_changes);
        File.WriteAllText("content.json", json);
    }
}