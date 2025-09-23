using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ContentPatcherMaker.Core.DataModels;

/// <summary>
/// 角色类型枚举
/// </summary>
public enum CharacterType
{
    /// <summary>
    /// 村民
    /// </summary>
    [Display(Name = "村民", Description = "普通村民")]
    Villager,

    /// <summary>
    /// 可结婚角色
    /// </summary>
    [Display(Name = "可结婚角色", Description = "可以结婚的角色")]
    Marriageable,

    /// <summary>
    /// 特殊角色
    /// </summary>
    [Display(Name = "特殊角色", Description = "特殊角色")]
    Special,

    /// <summary>
    /// 动物
    /// </summary>
    [Display(Name = "动物", Description = "动物角色")]
    Animal,

    /// <summary>
    /// 怪物
    /// </summary>
    [Display(Name = "怪物", Description = "怪物角色")]
    Monster
}

/// <summary>
/// 角色数据模型
/// 基于Content/Characters/schedules/*.json文件
/// </summary>
public record CharacterData
{
    /// <summary>
    /// 角色ID
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 角色名称
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色描述
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 角色类型
    /// </summary>
    [JsonProperty("type")]
    public CharacterType Type { get; set; }

    /// <summary>
    /// 是否可结婚
    /// </summary>
    [JsonProperty("isMarriageable")]
    public bool IsMarriageable { get; set; }

    /// <summary>
    /// 生日（月-日格式，如"spring-15"）
    /// </summary>
    [JsonProperty("birthday")]
    public string? Birthday { get; set; }

    /// <summary>
    /// 最爱物品列表
    /// </summary>
    [JsonProperty("lovedItems")]
    public List<string> LovedItems { get; set; } = new();

    /// <summary>
    /// 喜欢物品列表
    /// </summary>
    [JsonProperty("likedItems")]
    public List<string> LikedItems { get; set; } = new();

    /// <summary>
    /// 中立物品列表
    /// </summary>
    [JsonProperty("neutralItems")]
    public List<string> NeutralItems { get; set; } = new();

    /// <summary>
    /// 不喜欢物品列表
    /// </summary>
    [JsonProperty("dislikedItems")]
    public List<string> DislikedItems { get; set; } = new();

    /// <summary>
    /// 讨厌物品列表
    /// </summary>
    [JsonProperty("hatedItems")]
    public List<string> HatedItems { get; set; } = new();

    /// <summary>
    /// 日程数据
    /// </summary>
    [JsonProperty("schedules")]
    public Dictionary<string, string> Schedules { get; set; } = new();

    /// <summary>
    /// 对话数据
    /// </summary>
    [JsonProperty("dialogue")]
    public Dictionary<string, string> Dialogue { get; set; } = new();

    /// <summary>
    /// 验证角色数据
    /// </summary>
    /// <returns>验证结果</returns>
    public ValidationResult Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Id))
            errors.Add("角色ID不能为空");

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("角色名称不能为空");

        if (IsMarriageable && Type != CharacterType.Marriageable)
            errors.Add("可结婚角色的类型必须是Marriageable");

        return new ValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors
        };
    }
}

/// <summary>
/// 角色数据集合
/// </summary>
public class CharacterDataCollection : DataModelCollection<CharacterData>
{
    private readonly Dictionary<string, CharacterData> _characters = new();

    /// <summary>
    /// 初始化角色数据集合
    /// </summary>
    /// <param name="characters">角色数据列表</param>
    public CharacterDataCollection(IEnumerable<CharacterData> characters)
    {
        foreach (var character in characters)
        {
            if (!string.IsNullOrWhiteSpace(character.Id))
            {
                _characters[character.Id] = character;
            }
        }
    }

    /// <summary>
    /// 获取所有角色
    /// </summary>
    public override IEnumerable<CharacterData> GetAll() => _characters.Values;

    /// <summary>
    /// 根据ID获取角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>角色数据，如果不存在则返回null</returns>
    public override CharacterData? GetById(string id) => _characters.GetValueOrDefault(id);

    /// <summary>
    /// 检查角色是否存在
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>是否存在</returns>
    public override bool Exists(string id) => _characters.ContainsKey(id);

    /// <summary>
    /// 搜索角色
    /// </summary>
    /// <param name="predicate">搜索条件</param>
    /// <returns>匹配的角色列表</returns>
    public override IEnumerable<CharacterData> Search(Func<CharacterData, bool> predicate) => _characters.Values.Where(predicate);

    /// <summary>
    /// 获取可结婚的角色
    /// </summary>
    /// <returns>可结婚的角色列表</returns>
    public IEnumerable<CharacterData> GetMarriageableCharacters() => _characters.Values.Where(c => c.IsMarriageable);

    /// <summary>
    /// 获取村民角色
    /// </summary>
    /// <returns>村民角色列表</returns>
    public IEnumerable<CharacterData> GetVillagerCharacters() => _characters.Values.Where(c => c.Type == CharacterType.Villager);

    /// <summary>
    /// 获取特殊角色
    /// </summary>
    /// <returns>特殊角色列表</returns>
    public IEnumerable<CharacterData> GetSpecialCharacters() => _characters.Values.Where(c => c.Type == CharacterType.Special);

    /// <summary>
    /// 获取动物角色
    /// </summary>
    /// <returns>动物角色列表</returns>
    public IEnumerable<CharacterData> GetAnimalCharacters() => _characters.Values.Where(c => c.Type == CharacterType.Animal);

    /// <summary>
    /// 获取怪物角色
    /// </summary>
    /// <returns>怪物角色列表</returns>
    public IEnumerable<CharacterData> GetMonsterCharacters() => _characters.Values.Where(c => c.Type == CharacterType.Monster);

    /// <summary>
    /// 根据类型获取角色
    /// </summary>
    /// <param name="type">角色类型</param>
    /// <returns>指定类型的角色列表</returns>
    public IEnumerable<CharacterData> GetCharactersByType(CharacterType type) => _characters.Values.Where(c => c.Type == type);

    /// <summary>
    /// 根据生日获取角色
    /// </summary>
    /// <param name="birthday">生日</param>
    /// <returns>指定生日的角色列表</returns>
    public IEnumerable<CharacterData> GetCharactersByBirthday(string birthday) => _characters.Values.Where(c => c.Birthday == birthday);

    /// <summary>
    /// 搜索喜欢特定物品的角色
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <returns>喜欢该物品的角色列表</returns>
    public IEnumerable<CharacterData> GetCharactersWhoLikeItem(string itemName) =>
        _characters.Values.Where(c => 
            c.LovedItems.Contains(itemName) || 
            c.LikedItems.Contains(itemName));

    /// <summary>
    /// 搜索讨厌特定物品的角色
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <returns>讨厌该物品的角色列表</returns>
    public IEnumerable<CharacterData> GetCharactersWhoHateItem(string itemName) =>
        _characters.Values.Where(c => 
            c.DislikedItems.Contains(itemName) || 
            c.HatedItems.Contains(itemName));
}
