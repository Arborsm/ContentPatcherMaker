using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ContentPatcherMaker.Core.DataModels.Datas;

/// <summary>
/// 事件类型枚举
/// </summary>
public enum EventType
{
    /// <summary>
    /// 角色事件
    /// </summary>
    [Display(Name = "角色事件", Description = "与角色相关的事件")]
    Character,

    /// <summary>
    /// 节日事件
    /// </summary>
    [Display(Name = "节日事件", Description = "节日相关的事件")]
    Festival,

    /// <summary>
    /// 剧情事件
    /// </summary>
    [Display(Name = "剧情事件", Description = "主要剧情事件")]
    Story,

    /// <summary>
    /// 随机事件
    /// </summary>
    [Display(Name = "随机事件", Description = "随机发生的事件")]
    Random,

    /// <summary>
    /// 特殊事件
    /// </summary>
    [Display(Name = "特殊事件", Description = "特殊条件触发的事件")]
    Special
}

/// <summary>
/// 事件数据模型
/// 基于Content/Data/Events/*.json文件
/// </summary>
public record EventData
{
    /// <summary>
    /// 事件ID
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 事件名称
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 事件描述
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 事件类型
    /// </summary>
    [JsonProperty("type")]
    public EventType Type { get; set; }

    /// <summary>
    /// 事件脚本
    /// </summary>
    [JsonProperty("script")]
    public string? Script { get; set; }

    /// <summary>
    /// 触发条件
    /// </summary>
    [JsonProperty("conditions")]
    public string? Conditions { get; set; }

    /// <summary>
    /// 相关角色
    /// </summary>
    [JsonProperty("characters")]
    public List<string> Characters { get; set; } = [];

    /// <summary>
    /// 事件位置
    /// </summary>
    [JsonProperty("location")]
    public string? Location { get; set; }

    /// <summary>
    /// 事件时间
    /// </summary>
    [JsonProperty("time")]
    public string? Time { get; set; }

    /// <summary>
    /// 是否可重复
    /// </summary>
    [JsonProperty("isRepeatable")]
    public bool IsRepeatable { get; set; }

    /// <summary>
    /// 优先级
    /// </summary>
    [JsonProperty("priority")]
    public int Priority { get; set; }

    /// <summary>
    /// 前置事件
    /// </summary>
    [JsonProperty("prerequisites")]
    public List<string> Prerequisites { get; set; } = [];

    /// <summary>
    /// 后续事件
    /// </summary>
    [JsonProperty("followUpEvents")]
    public List<string> FollowUpEvents { get; set; } = [];

    /// <summary>
    /// 验证事件数据
    /// </summary>
    /// <returns>验证结果</returns>
    public ValidationResult Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Id))
            errors.Add("事件ID不能为空");

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("事件名称不能为空");

        if (string.IsNullOrWhiteSpace(Script))
            errors.Add("事件脚本不能为空");

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
/// 事件数据集合
/// </summary>
public class EventDataCollection : DataModelCollection<EventData>
{
    private readonly Dictionary<string, EventData> _events = new();

    /// <summary>
    /// 初始化事件数据集合
    /// </summary>
    /// <param name="events">事件数据列表</param>
    public EventDataCollection(IEnumerable<EventData> events)
    {
        foreach (var evt in events)
        {
            if (!string.IsNullOrWhiteSpace(evt.Id))
            {
                _events[evt.Id] = evt;
            }
        }
    }

    /// <summary>
    /// 获取所有事件
    /// </summary>
    public override IEnumerable<EventData> GetAll() => _events.Values;

    /// <summary>
    /// 根据ID获取事件
    /// </summary>
    /// <param name="id">事件ID</param>
    /// <returns>事件数据，如果不存在则返回null</returns>
    public override EventData? GetById(string id) => _events.GetValueOrDefault(id);

    /// <summary>
    /// 检查事件是否存在
    /// </summary>
    /// <param name="id">事件ID</param>
    /// <returns>是否存在</returns>
    public override bool Exists(string id) => _events.ContainsKey(id);

    /// <summary>
    /// 搜索事件
    /// </summary>
    /// <param name="predicate">搜索条件</param>
    /// <returns>匹配的事件列表</returns>
    public override IEnumerable<EventData> Search(Func<EventData, bool> predicate) => _events.Values.Where(predicate);

    /// <summary>
    /// 根据类型获取事件
    /// </summary>
    /// <param name="type">事件类型</param>
    /// <returns>指定类型的事件列表</returns>
    public IEnumerable<EventData> GetEventsByType(EventType type) => _events.Values.Where(e => e.Type == type);

    /// <summary>
    /// 获取角色事件
    /// </summary>
    /// <returns>角色事件列表</returns>
    public IEnumerable<EventData> GetCharacterEvents() => _events.Values.Where(e => e.Type == EventType.Character);

    /// <summary>
    /// 获取节日事件
    /// </summary>
    /// <returns>节日事件列表</returns>
    public IEnumerable<EventData> GetFestivalEvents() => _events.Values.Where(e => e.Type == EventType.Festival);

    /// <summary>
    /// 获取剧情事件
    /// </summary>
    /// <returns>剧情事件列表</returns>
    public IEnumerable<EventData> GetStoryEvents() => _events.Values.Where(e => e.Type == EventType.Story);

    /// <summary>
    /// 获取随机事件
    /// </summary>
    /// <returns>随机事件列表</returns>
    public IEnumerable<EventData> GetRandomEvents() => _events.Values.Where(e => e.Type == EventType.Random);

    /// <summary>
    /// 获取特殊事件
    /// </summary>
    /// <returns>特殊事件列表</returns>
    public IEnumerable<EventData> GetSpecialEvents() => _events.Values.Where(e => e.Type == EventType.Special);

    /// <summary>
    /// 获取可重复的事件
    /// </summary>
    /// <returns>可重复的事件列表</returns>
    public IEnumerable<EventData> GetRepeatableEvents() => _events.Values.Where(e => e.IsRepeatable);

    /// <summary>
    /// 获取不可重复的事件
    /// </summary>
    /// <returns>不可重复的事件列表</returns>
    public IEnumerable<EventData> GetNonRepeatableEvents() => _events.Values.Where(e => !e.IsRepeatable);

    /// <summary>
    /// 根据角色获取事件
    /// </summary>
    /// <param name="characterId">角色ID</param>
    /// <returns>涉及该角色的事件列表</returns>
    public IEnumerable<EventData> GetEventsByCharacter(string characterId) => _events.Values.Where(e => e.Characters.Contains(characterId));

    /// <summary>
    /// 根据位置获取事件
    /// </summary>
    /// <param name="location">位置</param>
    /// <returns>在指定位置的事件列表</returns>
    public IEnumerable<EventData> GetEventsByLocation(string location) => _events.Values.Where(e => e.Location == location);

    /// <summary>
    /// 根据优先级获取事件
    /// </summary>
    /// <param name="priority">优先级</param>
    /// <returns>指定优先级的事件列表</returns>
    public IEnumerable<EventData> GetEventsByPriority(int priority) => _events.Values.Where(e => e.Priority == priority);

    /// <summary>
    /// 获取前置事件
    /// </summary>
    /// <param name="eventId">事件ID</param>
    /// <returns>前置事件列表</returns>
    public IEnumerable<EventData> GetPrerequisiteEvents(string eventId)
    {
        var evt = GetById(eventId);
        if (evt == null) return [];

        return evt.Prerequisites
            .Select(prereqId => GetById(prereqId))
            .Where(prereq => prereq != null)!;
    }

    /// <summary>
    /// 获取后续事件
    /// </summary>
    /// <param name="eventId">事件ID</param>
    /// <returns>后续事件列表</returns>
    public IEnumerable<EventData> GetFollowUpEvents(string eventId)
    {
        var evt = GetById(eventId);
        if (evt == null) return [];

        return evt.FollowUpEvents
            .Select(followUpId => GetById(followUpId))
            .Where(followUp => followUp != null)!;
    }
}
