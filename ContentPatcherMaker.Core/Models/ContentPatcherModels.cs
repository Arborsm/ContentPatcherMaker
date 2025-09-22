using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using ContentPatcherMaker.Core.DataModels;

namespace ContentPatcherMaker.Core.Models;

/// <summary>
/// ContentPatcher内容包的主要模型
/// </summary>
public class ContentPack
{
    /// <summary>
    /// ContentPatcher格式版本
    /// </summary>
    [Required]
    [JsonProperty("Format")]
    public string Format { get; set; } = "2.8.0";

    /// <summary>
    /// 补丁列表
    /// </summary>
    [JsonProperty("Changes")]
    public List<Patch> Changes { get; set; } = [];

    /// <summary>
    /// 配置模式定义
    /// </summary>
    [JsonProperty("ConfigSchema")]
    public Dictionary<string, ConfigSchemaField>? ConfigSchema { get; set; }

    /// <summary>
    /// 自定义地点定义
    /// </summary>
    [JsonProperty("CustomLocations")]
    public List<CustomLocation>? CustomLocations { get; set; }

    /// <summary>
    /// 动态令牌定义
    /// </summary>
    [JsonProperty("DynamicTokens")]
    public List<DynamicToken>? DynamicTokens { get; set; }
}

/// <summary>
/// 补丁基类
/// </summary>
public abstract class Patch
{
    /// <summary>
    /// 操作类型
    /// </summary>
    [Required]
    [JsonProperty("Action")]
    public abstract PatchActionType Action { get; }

    /// <summary>
    /// 目标素材名称
    /// </summary>
    [Required]
    [JsonProperty("Target")]
    public string Target { get; set; } = string.Empty;

    /// <summary>
    /// 条件表达式
    /// </summary>
    [JsonProperty("When")]
    public Dictionary<string, string>? When { get; set; }

    /// <summary>
    /// 日志名称
    /// </summary>
    [JsonProperty("LogName")]
    public string? LogName { get; set; }

    /// <summary>
    /// 更新频率
    /// </summary>
    [JsonProperty("Update")]
    public PatchUpdateFrequency? Update { get; set; }

    /// <summary>
    /// 局部令牌
    /// </summary>
    [JsonProperty("LocalTokens")]
    public Dictionary<string, string>? LocalTokens { get; set; }

    /// <summary>
    /// 优先级
    /// </summary>
    [JsonProperty("Priority")]
    public PatchPriority? Priority { get; set; }

    /// <summary>
    /// 目标语言环境
    /// </summary>
    [JsonProperty("TargetLocale")]
    public string? TargetLocale { get; set; }
}

/// <summary>
/// Load操作补丁
/// </summary>
public class LoadPatch : Patch
{
    /// <summary>
    /// 获取补丁操作类型
    /// </summary>
    public override PatchActionType Action => PatchActionType.Load;

    /// <summary>
    /// 源文件路径
    /// </summary>
    [Required]
    [JsonProperty("FromFile")]
    public string FromFile { get; set; } = string.Empty;
}

/// <summary>
/// EditData操作补丁
/// </summary>
public class EditDataPatch : Patch
{
    /// <summary>
    /// 获取补丁操作类型
    /// </summary>
    public override PatchActionType Action => PatchActionType.EditData;

    /// <summary>
    /// 要编辑的字段
    /// </summary>
    [JsonProperty("Fields")]
    public Dictionary<string, object>? Fields { get; set; }

    /// <summary>
    /// 要添加/替换/删除的条目
    /// </summary>
    [JsonProperty("Entries")]
    public Dictionary<string, object>? Entries { get; set; }

    /// <summary>
    /// 移动列表条目
    /// </summary>
    [JsonProperty("MoveEntries")]
    public List<MoveEntry>? MoveEntries { get; set; }

    /// <summary>
    /// 文本操作
    /// </summary>
    [JsonProperty("TextOperations")]
    public List<TextOperation>? TextOperations { get; set; }

    /// <summary>
    /// 目标字段
    /// </summary>
    [JsonProperty("TargetField")]
    public List<string>? TargetField { get; set; }
}

/// <summary>
/// EditImage操作补丁
/// </summary>
public class EditImagePatch : Patch
{
    /// <summary>
    /// 获取补丁操作类型
    /// </summary>
    public override PatchActionType Action => PatchActionType.EditImage;

    /// <summary>
    /// 源文件路径
    /// </summary>
    [Required]
    [JsonProperty("FromFile")]
    public string FromFile { get; set; } = string.Empty;

    /// <summary>
    /// 源区域
    /// </summary>
    [JsonProperty("FromArea")]
    public Area? FromArea { get; set; }

    /// <summary>
    /// 目标区域
    /// </summary>
    [JsonProperty("ToArea")]
    public Area? ToArea { get; set; }

    /// <summary>
    /// 补丁模式
    /// </summary>
    [JsonProperty("PatchMode")]
    public PatchMode? PatchMode { get; set; }
}

/// <summary>
/// EditMap操作补丁
/// </summary>
public class EditMapPatch : Patch
{
    /// <summary>
    /// 获取补丁操作类型
    /// </summary>
    public override PatchActionType Action => PatchActionType.EditMap;

    /// <summary>
    /// 源文件路径
    /// </summary>
    [JsonProperty("FromFile")]
    public string? FromFile { get; set; }

    /// <summary>
    /// 源区域
    /// </summary>
    [JsonProperty("FromArea")]
    public Area? FromArea { get; set; }

    /// <summary>
    /// 目标区域
    /// </summary>
    [JsonProperty("ToArea")]
    public Area? ToArea { get; set; }

    /// <summary>
    /// 补丁模式
    /// </summary>
    [JsonProperty("PatchMode")]
    public PatchMode? PatchMode { get; set; }

    /// <summary>
    /// 地图属性
    /// </summary>
    [JsonProperty("MapProperties")]
    public Dictionary<string, string>? MapProperties { get; set; }

    /// <summary>
    /// 添加NPC传送点
    /// </summary>
    [JsonProperty("AddNpcWarps")]
    public List<string>? AddNpcWarps { get; set; }

    /// <summary>
    /// 添加传送点
    /// </summary>
    [JsonProperty("AddWarps")]
    public List<string>? AddWarps { get; set; }

    /// <summary>
    /// 地图图块
    /// </summary>
    [JsonProperty("MapTiles")]
    public List<MapTile>? MapTiles { get; set; }

    /// <summary>
    /// 文本操作
    /// </summary>
    [JsonProperty("TextOperations")]
    public List<TextOperation>? TextOperations { get; set; }
}

/// <summary>
/// Include操作补丁
/// </summary>
public class IncludePatch : Patch
{
    /// <summary>
    /// 获取补丁操作类型
    /// </summary>
    public override PatchActionType Action => PatchActionType.Include;

    /// <summary>
    /// 源文件路径
    /// </summary>
    [Required]
    [JsonProperty("FromFile")]
    public string FromFile { get; set; } = string.Empty;
}

/// <summary>
/// 区域定义
/// </summary>
public class Area
{
    /// <summary>
    /// X坐标
    /// </summary>
    [JsonProperty("X")]
    public int X { get; set; }

    /// <summary>
    /// Y坐标
    /// </summary>
    [JsonProperty("Y")]
    public int Y { get; set; }

    /// <summary>
    /// 宽度
    /// </summary>
    [JsonProperty("Width")]
    public int Width { get; set; }

    /// <summary>
    /// 高度
    /// </summary>
    [JsonProperty("Height")]
    public int Height { get; set; }
}

/// <summary>
/// 移动条目定义
/// </summary>
public class MoveEntry
{
    /// <summary>
    /// 条目ID
    /// </summary>
    [JsonProperty("ID")]
    public string ID { get; set; } = string.Empty;

    /// <summary>
    /// 在指定ID之前插入
    /// </summary>
    [JsonProperty("BeforeID")]
    public string? BeforeID { get; set; }

    /// <summary>
    /// 在指定ID之后插入
    /// </summary>
    [JsonProperty("AfterID")]
    public string? AfterID { get; set; }

    /// <summary>
    /// 移动到指定位置
    /// </summary>
    [JsonProperty("ToPosition")]
    public string? ToPosition { get; set; }
}

/// <summary>
/// 文本操作定义
/// </summary>
public class TextOperation
{
    /// <summary>
    /// 操作类型
    /// </summary>
    [JsonProperty("Operation")]
    public TextOperationType Operation { get; set; } = TextOperationType.Replace;

    /// <summary>
    /// 目标字段列表
    /// </summary>
    [JsonProperty("Target")]
    public List<string> Target { get; set; } = [];

    /// <summary>
    /// 操作值
    /// </summary>
    [JsonProperty("Value")]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 分隔符
    /// </summary>
    [JsonProperty("Delimiter")]
    public string? Delimiter { get; set; }
}

/// <summary>
/// 地图图块定义
/// </summary>
public class MapTile
{
    /// <summary>
    /// 图层名称
    /// </summary>
    [JsonProperty("Layer")]
    public MapLayer Layer { get; set; } = MapLayer.Background;

    /// <summary>
    /// 位置信息
    /// </summary>
    [JsonProperty("Position")]
    public Area Position { get; set; } = new();

    /// <summary>
    /// 设置图块表
    /// </summary>
    [JsonProperty("SetTilesheet")]
    public string? SetTilesheet { get; set; }

    /// <summary>
    /// 设置图块索引
    /// </summary>
    [JsonProperty("SetIndex")]
    public string? SetIndex { get; set; }

    /// <summary>
    /// 设置属性
    /// </summary>
    [JsonProperty("SetProperties")]
    public Dictionary<string, string>? SetProperties { get; set; }

    /// <summary>
    /// 是否移除
    /// </summary>
    [JsonProperty("Remove")]
    public bool Remove { get; set; }
}

/// <summary>
/// 配置模式字段定义
/// </summary>
public class ConfigSchemaField
{
    /// <summary>
    /// 允许的值
    /// </summary>
    [JsonProperty("AllowValues")]
    public string? AllowValues { get; set; }

    /// <summary>
    /// 默认值
    /// </summary>
    [JsonProperty("Default")]
    public object? Default { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [JsonProperty("Description")]
    public string? Description { get; set; }
}

/// <summary>
/// 自定义地点定义
/// </summary>
public class CustomLocation
{
    /// <summary>
    /// 地点名称
    /// </summary>
    [JsonProperty("Name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 从地图文件加载
    /// </summary>
    [JsonProperty("FromMapFile")]
    public string? FromMapFile { get; set; }

    /// <summary>
    /// 从地图数据加载
    /// </summary>
    [JsonProperty("FromMapData")]
    public string? FromMapData { get; set; }

    /// <summary>
    /// 传送到目标地点
    /// </summary>
    [JsonProperty("WarpTo")]
    public string? WarpTo { get; set; }

    /// <summary>
    /// 传送到目标X坐标
    /// </summary>
    [JsonProperty("WarpToX")]
    public int? WarpToX { get; set; }

    /// <summary>
    /// 传送到目标Y坐标
    /// </summary>
    [JsonProperty("WarpToY")]
    public int? WarpToY { get; set; }

    /// <summary>
    /// 传送到目标方向
    /// </summary>
    [JsonProperty("WarpToDirection")]
    public int? WarpToDirection { get; set; }

    /// <summary>
    /// 从源地点传送
    /// </summary>
    [JsonProperty("WarpFrom")]
    public string? WarpFrom { get; set; }

    /// <summary>
    /// 从源X坐标传送
    /// </summary>
    [JsonProperty("WarpFromX")]
    public int? WarpFromX { get; set; }

    /// <summary>
    /// 从源Y坐标传送
    /// </summary>
    [JsonProperty("WarpFromY")]
    public int? WarpFromY { get; set; }

    /// <summary>
    /// 从源方向传送
    /// </summary>
    [JsonProperty("WarpFromDirection")]
    public int? WarpFromDirection { get; set; }

    /// <summary>
    /// 地图属性
    /// </summary>
    [JsonProperty("MapProperties")]
    public Dictionary<string, string>? MapProperties { get; set; }
}

/// <summary>
/// 动态令牌定义
/// </summary>
public class DynamicToken
{
    /// <summary>
    /// 令牌名称
    /// </summary>
    [JsonProperty("Name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 令牌值
    /// </summary>
    [JsonProperty("Value")]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 条件字典
    /// </summary>
    [JsonProperty("When")]
    public Dictionary<string, string>? When { get; set; }
}