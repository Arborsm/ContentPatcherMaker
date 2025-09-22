using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ContentPatcherMaker.Core.Models;
using ContentPatcherMaker.Core.Validation;
using ContentPatcherMaker.Models.Validation;
using ContentPatcherMaker.Services.Abstractions;

namespace ContentPatcherMaker.ViewModels;

public partial class WorkflowViewModel : ViewModelBase
{
    private readonly IAppLogger _logger;
    private readonly IPreviewService _preview;

    [ObservableProperty]
    private ContentPack currentPack = new() { Manifest = new Manifest() };

    public ObservableCollection<Patch> Patches { get; } = new();

    [ObservableProperty]
    private Patch? selectedPatch;

    public ObservableCollection<FieldEntry> FieldEntries { get; } = new();

    public ObservableCollection<ValidationError> ValidationErrors { get; } = new();

    [ObservableProperty]
    private string livePreviewJson = string.Empty;

    [ObservableProperty]
    private int selectedTabIndex;

    public WorkflowViewModel(IAppLogger logger, IPreviewService preview)
    {
        _logger = logger;
        _preview = preview;

        // Seed sample
        CurrentPack.Manifest.Name = "My Content Pack";
        CurrentPack.Manifest.Author = "Author";
        CurrentPack.Manifest.UniqueID = "YourName.ContentPack";
        CurrentPack.Manifest.Description = "A sample pack";
        CurrentPack.Manifest.ContentPackFor.UniqueID = "Pathoschild.ContentPatcher";

        // Bind list
        foreach (var p in CurrentPack.Changes)
            Patches.Add(p);

        RefreshAll();
    }

    partial void OnSelectedPatchChanged(Patch? value)
    {
        SyncFieldEntriesFromPatch();
    }

    [RelayCommand]
    private void AddPatch()
    {
        var p = new Patch
        {
            Action = "EditData",
            Target = "Data/Example"
        };
        Patches.Add(p);
        CurrentPack.Changes.Add(p);
        SelectedPatch = p;
        RefreshAll();
    }

    [RelayCommand]
    private void RemoveSelectedPatch()
    {
        if (SelectedPatch is null) return;
        CurrentPack.Changes.Remove(SelectedPatch);
        Patches.Remove(SelectedPatch);
        SelectedPatch = null;
        RefreshAll();
    }

    [RelayCommand]
    private void ApplyFieldEntries()
    {
        if (SelectedPatch is null) return;
        SelectedPatch.Fields = FieldEntries
            .Where(e => !string.IsNullOrWhiteSpace(e.Key))
            .GroupBy(e => e.Key)
            .ToDictionary(g => g.Key, g => (object?)g.Last().Value);
        RefreshAll();
    }

    private void SyncFieldEntriesFromPatch()
    {
        FieldEntries.Clear();
        if (SelectedPatch?.Fields is null) return;
        foreach (var kv in SelectedPatch.Fields)
        {
            FieldEntries.Add(new FieldEntry { Key = kv.Key, Value = kv.Value?.ToString() });
        }
    }

    [RelayCommand]
    private void AddFieldEntry()
    {
        FieldEntries.Add(new FieldEntry { Key = string.Empty, Value = string.Empty });
    }

    [RelayCommand]
    private void RemoveFieldEntry(FieldEntry? entry)
    {
        if (entry is null) return;
        FieldEntries.Remove(entry);
    }

    private void RefreshAll()
    {
        RefreshValidation();
        RefreshPreview();
    }

    private void RefreshValidation()
    {
        ValidationErrors.Clear();
        var result = ContentPackValidator.Validate(CurrentPack);
        foreach (var e in result.Errors)
        {
            ValidationErrors.Add(e);
        }
    }

    private void RefreshPreview()
    {
        try
        {
            LivePreviewJson = _preview.GetPreviewJson(CurrentPack);
        }
        catch (Exception ex)
        {
            _logger.Error("预览生成失败", ex);
        }
    }
}

public sealed class FieldEntry : ObservableObject
{
    private string key = string.Empty;
    public string Key
    {
        get => key; set => SetProperty(ref key, value);
    }

    private string? value;
    public string? Value
    {
        get => this.value; set => SetProperty(ref this.value, value);
    }
}

