using CommunityToolkit.Mvvm.Input;
using ContentPatcherMaker.Services.Abstractions;

namespace ContentPatcherMaker.ViewModels;

public partial class MainWindowViewModel
{
    private readonly ILocalizationService _localization;

    partial void OnCurrentLanguageChanged(string value)
    {
        _localization.SetCulture(value);
    }
}

