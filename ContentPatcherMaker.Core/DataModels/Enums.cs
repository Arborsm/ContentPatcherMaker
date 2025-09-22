using System.ComponentModel.DataAnnotations;

namespace ContentPatcherMaker.Core.DataModels;

/// <summary>
/// 补丁操作类型枚举
/// </summary>
public enum PatchActionType
{
    /// <summary>
    /// 加载操作
    /// </summary>
    [Display(Name = "加载", Description = "从文件加载内容")]
    Load,

    /// <summary>
    /// 编辑数据操作
    /// </summary>
    [Display(Name = "编辑数据", Description = "编辑游戏数据")]
    EditData,

    /// <summary>
    /// 编辑图像操作
    /// </summary>
    [Display(Name = "编辑图像", Description = "编辑图像资源")]
    EditImage,

    /// <summary>
    /// 编辑地图操作
    /// </summary>
    [Display(Name = "编辑地图", Description = "编辑地图数据")]
    EditMap,

    /// <summary>
    /// 包含操作
    /// </summary>
    [Display(Name = "包含", Description = "包含其他补丁文件")]
    Include
}

/// <summary>
/// 补丁更新频率枚举
/// </summary>
public enum PatchUpdateFrequency
{
    /// <summary>
    /// 从不更新
    /// </summary>
    [Display(Name = "从不", Description = "补丁从不更新")]
    Never,

    /// <summary>
    /// 游戏启动时更新
    /// </summary>
    [Display(Name = "游戏启动", Description = "游戏启动时更新")]
    OnGameStart,

    /// <summary>
    /// 每天更新
    /// </summary>
    [Display(Name = "每天", Description = "每天更新")]
    Daily,

    /// <summary>
    /// 每时更新
    /// </summary>
    [Display(Name = "每时", Description = "每小时更新")]
    Hourly,

    /// <summary>
    /// 每分钟更新
    /// </summary>
    [Display(Name = "每分钟", Description = "每分钟更新")]
    Minutely,

    /// <summary>
    /// 实时更新
    /// </summary>
    [Display(Name = "实时", Description = "实时更新")]
    Realtime
}

/// <summary>
/// 补丁优先级枚举
/// </summary>
public enum PatchPriority
{
    /// <summary>
    /// 最低优先级
    /// </summary>
    [Display(Name = "最低", Description = "最低优先级")]
    Lowest = -1000,

    /// <summary>
    /// 低优先级
    /// </summary>
    [Display(Name = "低", Description = "低优先级")]
    Low = -100,

    /// <summary>
    /// 正常优先级
    /// </summary>
    [Display(Name = "正常", Description = "正常优先级")]
    Normal = 0,

    /// <summary>
    /// 高优先级
    /// </summary>
    [Display(Name = "高", Description = "高优先级")]
    High = 100,

    /// <summary>
    /// 最高优先级
    /// </summary>
    [Display(Name = "最高", Description = "最高优先级")]
    Highest = 1000
}

/// <summary>
/// 补丁模式枚举
/// </summary>
public enum PatchMode
{
    /// <summary>
    /// 替换模式
    /// </summary>
    [Display(Name = "替换", Description = "完全替换目标内容")]
    Replace,

    /// <summary>
    /// 覆盖模式
    /// </summary>
    [Display(Name = "覆盖", Description = "覆盖目标内容")]
    Overlay,

    /// <summary>
    /// 添加模式
    /// </summary>
    [Display(Name = "添加", Description = "添加到目标内容")]
    Add,

    /// <summary>
    /// 移除模式
    /// </summary>
    [Display(Name = "移除", Description = "从目标内容中移除")]
    Remove,

    /// <summary>
    /// 合并模式
    /// </summary>
    [Display(Name = "合并", Description = "与目标内容合并")]
    Merge
}

/// <summary>
/// 文本操作类型枚举
/// </summary>
public enum TextOperationType
{
    /// <summary>
    /// 替换操作
    /// </summary>
    [Display(Name = "替换", Description = "替换文本")]
    Replace,

    /// <summary>
    /// 添加操作
    /// </summary>
    [Display(Name = "添加", Description = "添加文本")]
    Add,

    /// <summary>
    /// 移除操作
    /// </summary>
    [Display(Name = "移除", Description = "移除文本")]
    Remove,

    /// <summary>
    /// 插入操作
    /// </summary>
    [Display(Name = "插入", Description = "插入文本")]
    Insert,

    /// <summary>
    /// 追加操作
    /// </summary>
    [Display(Name = "追加", Description = "追加文本")]
    Append,

    /// <summary>
    /// 前置操作
    /// </summary>
    [Display(Name = "前置", Description = "前置文本")]
    Prepend
}

/// <summary>
/// 地图图层枚举
/// </summary>
public enum MapLayer
{
    /// <summary>
    /// 背景图层
    /// </summary>
    [Display(Name = "背景", Description = "背景图层")]
    Background,

    /// <summary>
    /// 建筑图层
    /// </summary>
    [Display(Name = "建筑", Description = "建筑图层")]
    Buildings,

    /// <summary>
    /// 路径图层
    /// </summary>
    [Display(Name = "路径", Description = "路径图层")]
    Paths,

    /// <summary>
    /// 前景图层
    /// </summary>
    [Display(Name = "前景", Description = "前景图层")]
    Foreground,

    /// <summary>
    /// 始终在前图层
    /// </summary>
    [Display(Name = "始终在前", Description = "始终在前图层")]
    AlwaysFront
}

/// <summary>
/// 季节枚举
/// </summary>
public enum Season
{
    /// <summary>
    /// 春季
    /// </summary>
    [Display(Name = "春季", Description = "春季")]
    Spring,

    /// <summary>
    /// 夏季
    /// </summary>
    [Display(Name = "夏季", Description = "夏季")]
    Summer,

    /// <summary>
    /// 秋季
    /// </summary>
    [Display(Name = "秋季", Description = "秋季")]
    Fall,

    /// <summary>
    /// 冬季
    /// </summary>
    [Display(Name = "冬季", Description = "冬季")]
    Winter
}

/// <summary>
/// 天气枚举
/// </summary>
public enum Weather
{
    /// <summary>
    /// 晴天
    /// </summary>
    [Display(Name = "晴天", Description = "晴天")]
    Sunny,

    /// <summary>
    /// 雨天
    /// </summary>
    [Display(Name = "雨天", Description = "雨天")]
    Rainy,

    /// <summary>
    /// 暴风雨
    /// </summary>
    [Display(Name = "暴风雨", Description = "暴风雨")]
    Stormy,

    /// <summary>
    /// 雪天
    /// </summary>
    [Display(Name = "雪天", Description = "雪天")]
    Snowy,

    /// <summary>
    /// 绿雨
    /// </summary>
    [Display(Name = "绿雨", Description = "绿雨")]
    GreenRain
}

/// <summary>
/// 时间枚举
/// </summary>
public enum TimeOfDay
{
    /// <summary>
    /// 早晨
    /// </summary>
    [Display(Name = "早晨", Description = "早晨")]
    Morning,

    /// <summary>
    /// 上午
    /// </summary>
    [Display(Name = "上午", Description = "上午")]
    Forenoon,

    /// <summary>
    /// 中午
    /// </summary>
    [Display(Name = "中午", Description = "中午")]
    Noon,

    /// <summary>
    /// 下午
    /// </summary>
    [Display(Name = "下午", Description = "下午")]
    Afternoon,

    /// <summary>
    /// 傍晚
    /// </summary>
    [Display(Name = "傍晚", Description = "傍晚")]
    Evening,

    /// <summary>
    /// 夜晚
    /// </summary>
    [Display(Name = "夜晚", Description = "夜晚")]
    Night
}
