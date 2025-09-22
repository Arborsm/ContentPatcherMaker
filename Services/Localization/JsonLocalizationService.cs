using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ContentPatcherMaker.Services.Localization;

public class JsonLocalizationService : ILocalizationService
{
    private readonly Dictionary<string, Dictionary<string, string>> _languageToKeyValues = new();
    private string _currentLanguage = "zh-CN";

    public IReadOnlyList<string> AvailableLanguages => _languageToKeyValues.Keys.OrderBy(k => k).ToList();
    public string CurrentLanguage => _currentLanguage;

    public void LoadLanguagesFromDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var jsonFiles = Directory.EnumerateFiles(directoryPath, "*.json", SearchOption.TopDirectoryOnly);
        foreach (var file in jsonFiles)
        {
            try
            {
                var langCode = Path.GetFileNameWithoutExtension(file);
                var raw = File.ReadAllText(file);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(raw) ?? new Dictionary<string, string>();
                _languageToKeyValues[langCode] = dict;
            }
            catch
            {
                // ignore invalid json files but keep going
            }
        }

        if (!_languageToKeyValues.ContainsKey(_currentLanguage) && _languageToKeyValues.Count > 0)
        {
            _currentLanguage = _languageToKeyValues.Keys.First();
        }
    }

    public void SetLanguage(string languageCode)
    {
        if (!_languageToKeyValues.ContainsKey(languageCode))
        {
            throw new ArgumentException($"Language not loaded: {languageCode}");
        }
        _currentLanguage = languageCode;
    }

    public string T(string key, string? fallback = null)
    {
        if (_languageToKeyValues.TryGetValue(_currentLanguage, out var dict))
        {
            if (dict.TryGetValue(key, out var value))
            {
                return value;
            }
        }
        return fallback ?? key;
    }
}

