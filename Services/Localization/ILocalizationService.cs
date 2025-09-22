using System.Collections.Generic;

namespace ContentPatcherMaker.Services.Localization;

public interface ILocalizationService
{
    IReadOnlyList<string> AvailableLanguages { get; }
    string CurrentLanguage { get; }
    void LoadLanguagesFromDirectory(string directoryPath);
    void SetLanguage(string languageCode);
    string T(string key, string? fallback = null);
}

