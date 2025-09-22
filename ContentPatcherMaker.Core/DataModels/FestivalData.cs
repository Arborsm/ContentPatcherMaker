using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ContentPatcherMaker.Core.DataModels;

/// <summary>
/// 节日类型枚举
/// </summary>
public enum FestivalType
{
    /// <summary>
    /// 春季节日
    /// </summary>
    [Display(Name = "春季节日", Description = "春季举行的节日")]
    Spring,

    /// <summary>
    /// 夏季节日
    /// </summary>
    [Display(Name = "夏季节日", Description = "夏季举行的节日")]
    Summer,

    /// <summary>
    /// 秋季节日
    /// </summary>
    [Display(Name = "秋季节日", Description = "秋季举行的节日")]
    Fall,

    /// <summary>
    /// 冬季节日
    /// </summary>
    [Display(Name = "冬季节日", Description = "冬季举行的节日")]
    Winter,

    /// <summary>
    /// 特殊节日
    /// </summary>
    [Display(Name = "特殊节日", Description = "特殊节日")]
    Special
}

/// <summary>
/// 节日数据模型
/// 基于Content/Data/Festivals/*.json文件
/// </summary>
public record FestivalData : IDataModel
{
    /// <summary>
    /// 节日ID
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// 节日名称
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// 节日描述
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; init; }

    /// <summary>
    /// 节日类型
    /// </summary>
    [JsonProperty("type")]
    public FestivalType Type { get; init; }

    /// <summary>
    /// 节日日期（月-日格式，如"spring-13"）
    /// </summary>
    [JsonProperty("date")]
    public string? Date { get; init; }

    /// <summary>
    /// 节日时间
    /// </summary>
    [JsonProperty("time")]
    public string? Time { get; init; }

    /// <summary>
    /// 节日位置
    /// </summary>
    [JsonProperty("location")]
    public string? Location { get; init; }

    /// <summary>
    /// 触发条件
    /// </summary>
    [JsonProperty("conditions")]
    public string? Conditions { get; init; }

    /// <summary>
    /// 节日设置脚本
    /// </summary>
    [JsonProperty("setupScript")]
    public string? SetupScript { get; init; }

    /// <summary>
    /// 节日对话
    /// </summary>
    [JsonProperty("dialogue")]
    public Dictionary<string, string> Dialogue { get; init; } = new();

    /// <summary>
    /// 相关角色
    /// </summary>
    [JsonProperty("characters")]
    public List<string> Characters { get; init; } = new();

    /// <summary>
    /// 节日活动
    /// </summary>
    [JsonProperty("activities")]
    public List<string> Activities { get; init; } = new();

    /// <summary>
    /// 奖励物品
    /// </summary>
    [JsonProperty("rewards")]
    public List<string> Rewards { get; init; } = new();

    /// <summary>
    /// 是否每年重复
    /// </summary>
    [JsonProperty("isYearly")]
    public bool IsYearly { get; init; } = true;

    /// <summary>
    /// 是否可跳过
    /// </summary>
    [JsonProperty("isSkippable")]
    public bool IsSkippable { get; init; } = false;

    /// <summary>
    /// 优先级
    /// </summary>
    [JsonProperty("priority")]
    public int Priority { get; init; }

    /// <summary>
    /// 验证节日数据
    /// </summary>
    /// <returns>验证结果</returns>
    public ValidationResult Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Id))
            errors.Add("节日ID不能为空");

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("节日名称不能为空");

        if (string.IsNullOrWhiteSpace(Date))
            errors.Add("节日日期不能为空");

        if (string.IsNullOrWhiteSpace(Location))
            errors.Add("节日位置不能为空");

        if (Priority < 0)
            errors.Add("优先级不能为负数");

        return new ValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors
        };
    }
}

/// <summary>
/// 节日数据集合
/// </summary>
public class FestivalDataCollection : IDataModelCollection<FestivalData>
{
    private readonly Dictionary<string, FestivalData> _festivals = new();

    /// <summary>
    /// 初始化节日数据集合
    /// </summary>
    /// <param name="festivals">节日数据列表</param>
    public FestivalDataCollection(IEnumerable<FestivalData> festivals)
    {
        foreach (var festival in festivals)
        {
            if (!string.IsNullOrWhiteSpace(festival.Id))
            {
                _festivals[festival.Id] = festival;
            }
        }
    }

    /// <summary>
    /// 获取所有节日
    /// </summary>
    public IEnumerable<FestivalData> GetAll() => _festivals.Values;

    /// <summary>
    /// 根据ID获取节日
    /// </summary>
    /// <param name="id">节日ID</param>
    /// <returns>节日数据，如果不存在则返回null</returns>
    public FestivalData? GetById(string id)
    {
        return _festivals.TryGetValue(id, out var festival) ? festival : null;
    }

    /// <summary>
    /// 检查节日是否存在
    /// </summary>
    /// <param name="id">节日ID</param>
    /// <returns>是否存在</returns>
    public bool Exists(string id) => _festivals.ContainsKey(id);

    /// <summary>
    /// 搜索节日
    /// </summary>
    /// <param name="predicate">搜索条件</param>
    /// <returns>匹配的节日列表</returns>
    public IEnumerable<FestivalData> Search(Func<FestivalData, bool> predicate)
    {
        return _festivals.Values.Where(predicate);
    }

    /// <summary>
    /// 根据类型获取节日
    /// </summary>
    /// <param name="type">节日类型</param>
    /// <returns>指定类型的节日列表</returns>
    public IEnumerable<FestivalData> GetFestivalsByType(FestivalType type)
    {
        return _festivals.Values.Where(f => f.Type == type);
    }

    /// <summary>
    /// 获取春季节日
    /// </summary>
    /// <returns>春季节日列表</returns>
    public IEnumerable<FestivalData> GetSpringFestivals()
    {
        return _festivals.Values.Where(f => f.Type == FestivalType.Spring);
    }

    /// <summary>
    /// 获取夏季节日
    /// </summary>
    /// <returns>夏季节日列表</returns>
    public IEnumerable<FestivalData> GetSummerFestivals()
    {
        return _festivals.Values.Where(f => f.Type == FestivalType.Summer);
    }

    /// <summary>
    /// 获取秋季节日
    /// </summary>
    /// <returns>秋季节日列表</returns>
    public IEnumerable<FestivalData> GetFallFestivals()
    {
        return _festivals.Values.Where(f => f.Type == FestivalType.Fall);
    }

    /// <summary>
    /// 获取冬季节日
    /// </summary>
    /// <returns>冬季节日列表</returns>
    public IEnumerable<FestivalData> GetWinterFestivals()
    {
        return _festivals.Values.Where(f => f.Type == FestivalType.Winter);
    }

    /// <summary>
    /// 获取特殊节日
    /// </summary>
    /// <returns>特殊节日列表</returns>
    public IEnumerable<FestivalData> GetSpecialFestivals()
    {
        return _festivals.Values.Where(f => f.Type == FestivalType.Special);
    }

    /// <summary>
    /// 根据日期获取节日
    /// </summary>
    /// <param name="date">日期</param>
    /// <returns>指定日期的节日列表</returns>
    public IEnumerable<FestivalData> GetFestivalsByDate(string date)
    {
        return _festivals.Values.Where(f => f.Date == date);
    }

    /// <summary>
    /// 根据位置获取节日
    /// </summary>
    /// <param name="location">位置</param>
    /// <returns>在指定位置的节日列表</returns>
    public IEnumerable<FestivalData> GetFestivalsByLocation(string location)
    {
        return _festivals.Values.Where(f => f.Location == location);
    }

    /// <summary>
    /// 获取可跳过的节日
    /// </summary>
    /// <returns>可跳过的节日列表</returns>
    public IEnumerable<FestivalData> GetSkippableFestivals()
    {
        return _festivals.Values.Where(f => f.IsSkippable);
    }

    /// <summary>
    /// 获取不可跳过的节日
    /// </summary>
    /// <returns>不可跳过的节日列表</returns>
    public IEnumerable<FestivalData> GetNonSkippableFestivals()
    {
        return _festivals.Values.Where(f => !f.IsSkippable);
    }

    /// <summary>
    /// 获取每年重复的节日
    /// </summary>
    /// <returns>每年重复的节日列表</returns>
    public IEnumerable<FestivalData> GetYearlyFestivals()
    {
        return _festivals.Values.Where(f => f.IsYearly);
    }

    /// <summary>
    /// 获取不每年重复的节日
    /// </summary>
    /// <returns>不每年重复的节日列表</returns>
    public IEnumerable<FestivalData> GetNonYearlyFestivals()
    {
        return _festivals.Values.Where(f => !f.IsYearly);
    }

    /// <summary>
    /// 根据角色获取节日
    /// </summary>
    /// <param name="characterId">角色ID</param>
    /// <returns>涉及该角色的节日列表</returns>
    public IEnumerable<FestivalData> GetFestivalsByCharacter(string characterId)
    {
        return _festivals.Values.Where(f => f.Characters.Contains(characterId));
    }

    /// <summary>
    /// 根据活动获取节日
    /// </summary>
    /// <param name="activity">活动名称</param>
    /// <returns>包含该活动的节日列表</returns>
    public IEnumerable<FestivalData> GetFestivalsByActivity(string activity)
    {
        return _festivals.Values.Where(f => f.Activities.Contains(activity));
    }

    /// <summary>
    /// 根据优先级获取节日
    /// </summary>
    /// <param name="priority">优先级</param>
    /// <returns>指定优先级的节日列表</returns>
    public IEnumerable<FestivalData> GetFestivalsByPriority(int priority)
    {
        return _festivals.Values.Where(f => f.Priority == priority);
    }

    /// <summary>
    /// 获取按日期排序的节日
    /// </summary>
    /// <returns>按日期排序的节日列表</returns>
    public IEnumerable<FestivalData> GetFestivalsOrderedByDate()
    {
        return _festivals.Values.OrderBy(f => f.Date);
    }
}
