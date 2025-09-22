using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ContentPatcherMaker.Core.DataModels;

/// <summary>
/// 语言数据模型
/// 基于Content/Data/AdditionalLanguages.json文件
/// </summary>
public record LanguageData : IDataModel
{
    /// <summary>
    /// 语言ID
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// 语言名称
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// 语言描述
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; init; }

    /// <summary>
    /// 语言代码
    /// </summary>
    [JsonProperty("code")]
    public string? Code { get; init; }

    /// <summary>
    /// 本地化代码
    /// </summary>
    [JsonProperty("locale")]
    public string? Locale { get; init; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [JsonProperty("isEnabled")]
    public bool IsEnabled { get; init; } = true;

    /// <summary>
    /// 是否默认语言
    /// </summary>
    [JsonProperty("isDefault")]
    public bool IsDefault { get; init; } = false;

    /// <summary>
    /// 语言文件路径
    /// </summary>
    [JsonProperty("filePath")]
    public string? FilePath { get; init; }

    /// <summary>
    /// 字体文件路径
    /// </summary>
    [JsonProperty("fontPath")]
    public string? FontPath { get; init; }

    /// <summary>
    /// 字符集
    /// </summary>
    [JsonProperty("characterSet")]
    public string? CharacterSet { get; init; }

    /// <summary>
    /// 文本方向
    /// </summary>
    [JsonProperty("textDirection")]
    public string? TextDirection { get; init; } = "ltr";

    /// <summary>
    /// 验证语言数据
    /// </summary>
    /// <returns>验证结果</returns>
    public ValidationResult Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Id))
            errors.Add("语言ID不能为空");

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("语言名称不能为空");

        if (string.IsNullOrWhiteSpace(Code))
            errors.Add("语言代码不能为空");

        if (string.IsNullOrWhiteSpace(Locale))
            errors.Add("本地化代码不能为空");

        return new ValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors
        };
    }
}

/// <summary>
/// 语言数据集合
/// </summary>
public class LanguageDataCollection : IDataModelCollection<LanguageData>
{
    private readonly Dictionary<string, LanguageData> _languages = new();

    /// <summary>
    /// 初始化语言数据集合
    /// </summary>
    /// <param name="languages">语言数据列表</param>
    public LanguageDataCollection(IEnumerable<LanguageData> languages)
    {
        foreach (var language in languages)
        {
            if (!string.IsNullOrWhiteSpace(language.Id))
            {
                _languages[language.Id] = language;
            }
        }
    }

    /// <summary>
    /// 获取所有语言
    /// </summary>
    public IEnumerable<LanguageData> GetAll() => _languages.Values;

    /// <summary>
    /// 根据ID获取语言
    /// </summary>
    /// <param name="id">语言ID</param>
    /// <returns>语言数据，如果不存在则返回null</returns>
    public LanguageData? GetById(string id)
    {
        return _languages.TryGetValue(id, out var language) ? language : null;
    }

    /// <summary>
    /// 检查语言是否存在
    /// </summary>
    /// <param name="id">语言ID</param>
    /// <returns>是否存在</returns>
    public bool Exists(string id) => _languages.ContainsKey(id);

    /// <summary>
    /// 搜索语言
    /// </summary>
    /// <param name="predicate">搜索条件</param>
    /// <returns>匹配的语言列表</returns>
    public IEnumerable<LanguageData> Search(Func<LanguageData, bool> predicate)
    {
        return _languages.Values.Where(predicate);
    }

    /// <summary>
    /// 根据代码获取语言
    /// </summary>
    /// <param name="code">语言代码</param>
    /// <returns>指定代码的语言，如果不存在则返回null</returns>
    public LanguageData? GetByCode(string code)
    {
        return _languages.Values.FirstOrDefault(l => l.Code == code);
    }

    /// <summary>
    /// 根据本地化代码获取语言
    /// </summary>
    /// <param name="locale">本地化代码</param>
    /// <returns>指定本地化代码的语言，如果不存在则返回null</returns>
    public LanguageData? GetByLocale(string locale)
    {
        return _languages.Values.FirstOrDefault(l => l.Locale == locale);
    }

    /// <summary>
    /// 获取启用的语言
    /// </summary>
    /// <returns>启用的语言列表</returns>
    public IEnumerable<LanguageData> GetEnabledLanguages()
    {
        return _languages.Values.Where(l => l.IsEnabled);
    }

    /// <summary>
    /// 获取禁用的语言
    /// </summary>
    /// <returns>禁用的语言列表</returns>
    public IEnumerable<LanguageData> GetDisabledLanguages()
    {
        return _languages.Values.Where(l => !l.IsEnabled);
    }

    /// <summary>
    /// 获取默认语言
    /// </summary>
    /// <returns>默认语言，如果不存在则返回null</returns>
    public LanguageData? GetDefaultLanguage()
    {
        return _languages.Values.FirstOrDefault(l => l.IsDefault);
    }

    /// <summary>
    /// 根据文本方向获取语言
    /// </summary>
    /// <param name="direction">文本方向</param>
    /// <returns>指定文本方向的语言列表</returns>
    public IEnumerable<LanguageData> GetLanguagesByTextDirection(string direction)
    {
        return _languages.Values.Where(l => l.TextDirection == direction);
    }

    /// <summary>
    /// 获取从左到右的语言
    /// </summary>
    /// <returns>从左到右的语言列表</returns>
    public IEnumerable<LanguageData> GetLeftToRightLanguages()
    {
        return _languages.Values.Where(l => l.TextDirection == "ltr");
    }

    /// <summary>
    /// 获取从右到左的语言
    /// </summary>
    /// <returns>从右到左的语言列表</returns>
    public IEnumerable<LanguageData> GetRightToLeftLanguages()
    {
        return _languages.Values.Where(l => l.TextDirection == "rtl");
    }

    /// <summary>
    /// 根据字符集获取语言
    /// </summary>
    /// <param name="characterSet">字符集</param>
    /// <returns>指定字符集的语言列表</returns>
    public IEnumerable<LanguageData> GetLanguagesByCharacterSet(string characterSet)
    {
        return _languages.Values.Where(l => l.CharacterSet == characterSet);
    }

    /// <summary>
    /// 获取有字体文件的语言
    /// </summary>
    /// <returns>有字体文件的语言列表</returns>
    public IEnumerable<LanguageData> GetLanguagesWithFonts()
    {
        return _languages.Values.Where(l => !string.IsNullOrWhiteSpace(l.FontPath));
    }

    /// <summary>
    /// 获取没有字体文件的语言
    /// </summary>
    /// <returns>没有字体文件的语言列表</returns>
    public IEnumerable<LanguageData> GetLanguagesWithoutFonts()
    {
        return _languages.Values.Where(l => string.IsNullOrWhiteSpace(l.FontPath));
    }
}
