using StardewValley.GameData;

namespace ContentPatcherMaker.Core.DataModels.Datas;

/// <summary>
/// 语言数据集合
/// </summary>
public class LanguageDataCollection : DataModelCollection<ModLanguage>
{
    private readonly Dictionary<string, ModLanguage> _languages = new();

    /// <summary>
    /// 初始化语言数据集合
    /// </summary>
    /// <param name="languages">语言数据列表</param>
    public LanguageDataCollection(IEnumerable<ModLanguage> languages)
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
    public override IEnumerable<ModLanguage> GetAll() => _languages.Values;

    /// <summary>
    /// 根据ID获取语言
    /// </summary>
    /// <param name="id">语言ID</param>
    /// <returns>语言数据，如果不存在则返回null</returns>
    public override ModLanguage? GetById(string id) => _languages.GetValueOrDefault(id);

    /// <summary>
    /// 检查语言是否存在
    /// </summary>
    /// <param name="id">语言ID</param>
    /// <returns>是否存在</returns>
    public override bool Exists(string id) => _languages.ContainsKey(id);

    /// <summary>
    /// 搜索语言
    /// </summary>
    /// <param name="predicate">搜索条件</param>
    /// <returns>匹配的语言列表</returns>
    public override IEnumerable<ModLanguage> Search(Func<ModLanguage, bool> predicate) => _languages.Values.Where(predicate);

    /// <summary>
    /// 根据代码获取语言
    /// </summary>
    /// <param name="code">语言代码</param>
    /// <returns>指定代码的语言，如果不存在则返回null</returns>
    public ModLanguage? GetByCode(string code) => _languages.Values.FirstOrDefault(l => l.LanguageCode == code);
}
