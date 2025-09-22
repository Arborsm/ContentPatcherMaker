using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ContentPatcherMaker.Core.DataModels;

/// <summary>
/// 成就数据模型
/// 基于Content/Data/Achievements.json文件
/// </summary>
public record AchievementData : IDataModel
{
    /// <summary>
    /// 成就ID
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// 成就名称
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// 成就描述
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; init; }

    /// <summary>
    /// 是否隐藏
    /// </summary>
    [JsonProperty("isHidden")]
    public bool IsHidden { get; init; }

    /// <summary>
    /// 前置成就ID
    /// </summary>
    [JsonProperty("prerequisiteId")]
    public int PrerequisiteId { get; init; }

    /// <summary>
    /// 图标ID
    /// </summary>
    [JsonProperty("iconId")]
    public int IconId { get; init; }

    /// <summary>
    /// 验证成就数据
    /// </summary>
    /// <returns>验证结果</returns>
    public ValidationResult Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Id))
            errors.Add("成就ID不能为空");

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("成就名称不能为空");

        if (PrerequisiteId < -1)
            errors.Add("前置成就ID不能小于-1");

        if (IconId < 0)
            errors.Add("图标ID不能为负数");

        return new ValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors
        };
    }
}

/// <summary>
/// 成就数据集合
/// </summary>
public class AchievementDataCollection : IDataModelCollection<AchievementData>
{
    private readonly Dictionary<string, AchievementData> _achievements = new();

    /// <summary>
    /// 初始化成就数据集合
    /// </summary>
    /// <param name="achievements">成就数据列表</param>
    public AchievementDataCollection(IEnumerable<AchievementData> achievements)
    {
        foreach (var achievement in achievements)
        {
            if (!string.IsNullOrWhiteSpace(achievement.Id))
            {
                _achievements[achievement.Id] = achievement;
            }
        }
    }

    /// <summary>
    /// 获取所有成就
    /// </summary>
    public IEnumerable<AchievementData> GetAll() => _achievements.Values;

    /// <summary>
    /// 根据ID获取成就
    /// </summary>
    /// <param name="id">成就ID</param>
    /// <returns>成就数据，如果不存在则返回null</returns>
    public AchievementData? GetById(string id)
    {
        return _achievements.TryGetValue(id, out var achievement) ? achievement : null;
    }

    /// <summary>
    /// 检查成就是否存在
    /// </summary>
    /// <param name="id">成就ID</param>
    /// <returns>是否存在</returns>
    public bool Exists(string id) => _achievements.ContainsKey(id);

    /// <summary>
    /// 搜索成就
    /// </summary>
    /// <param name="predicate">搜索条件</param>
    /// <returns>匹配的成就列表</returns>
    public IEnumerable<AchievementData> Search(Func<AchievementData, bool> predicate)
    {
        return _achievements.Values.Where(predicate);
    }

    /// <summary>
    /// 获取可获得的成就（非隐藏）
    /// </summary>
    /// <returns>可获得的成就列表</returns>
    public IEnumerable<AchievementData> GetVisibleAchievements()
    {
        return _achievements.Values.Where(a => !a.IsHidden);
    }

    /// <summary>
    /// 获取隐藏的成就
    /// </summary>
    /// <returns>隐藏的成就列表</returns>
    public IEnumerable<AchievementData> GetHiddenAchievements()
    {
        return _achievements.Values.Where(a => a.IsHidden);
    }

    /// <summary>
    /// 根据前置成就获取成就
    /// </summary>
    /// <param name="prerequisiteId">前置成就ID</param>
    /// <returns>依赖该前置成就的成就列表</returns>
    public IEnumerable<AchievementData> GetAchievementsByPrerequisite(int prerequisiteId)
    {
        return _achievements.Values.Where(a => a.PrerequisiteId == prerequisiteId);
    }

    /// <summary>
    /// 获取顶级成就（无前置成就）
    /// </summary>
    /// <returns>顶级成就列表</returns>
    public IEnumerable<AchievementData> GetTopLevelAchievements()
    {
        return _achievements.Values.Where(a => a.PrerequisiteId == -1);
    }
}
