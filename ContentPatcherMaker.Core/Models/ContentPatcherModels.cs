using System.ComponentModel.DataAnnotations;
using ContentPatcherMaker.Core.DataModels;
using Newtonsoft.Json;

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
    [JsonProperty(nameof(Format))]
    public string Format { get; set; } = "2.8.0";

    /// <summary>
    /// 补丁列表
    /// </summary>
    [JsonProperty(nameof(Changes))]
    public List<Patch> Changes { get; set; } = [];

    /// <summary>
    /// 配置模式定义
    /// </summary>
    [JsonProperty(nameof(ConfigSchema))]
    public Dictionary<string, ConfigSchemaField>? ConfigSchema { get; set; }

    /// <summary>
    /// 自定义地点定义
    /// </summary>
    [JsonProperty(nameof(CustomLocations))]
    public List<CustomLocation>? CustomLocations { get; set; }

    /// <summary>
    /// 动态令牌定义
    /// </summary>
    [JsonProperty(nameof(DynamicTokens))]
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
    [JsonProperty(nameof(Action))]
    public abstract PatchActionType Action { get; }

    /// <summary>
    /// 目标素材名称
    /// </summary>
    [Required]
    [JsonProperty(nameof(Target))]
    public string Target { get; set; } = string.Empty;

    /// <summary>
    /// 条件表达式
    /// </summary>
    [JsonProperty(nameof(When))]
    public Dictionary<string, string>? When { get; set; }

    /// <summary>
    /// 日志名称
    /// </summary>
    [JsonProperty(nameof(LogName))]
    public string? LogName { get; set; }

    /// <summary>
    /// 更新频率
    /// </summary>
    [JsonProperty(nameof(Update))]
    public PatchUpdateFrequency? Update { get; set; }

    /// <summary>
    /// 局部令牌
    /// </summary>
    [JsonProperty(nameof(LocalTokens))]
    public Dictionary<string, string>? LocalTokens { get; set; }

    /// <summary>
    /// 优先级
    /// </summary>
    [JsonProperty(nameof(Priority))]
    public PatchPriority? Priority { get; set; }

    /// <summary>
    /// 目标语言环境
    /// </summary>
    [JsonProperty(nameof(TargetLocale))]
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
    [JsonProperty(nameof(FromFile))]
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
    [JsonProperty(nameof(Fields))]
    public Dictionary<string, object>? Fields { get; set; }

    /// <summary>
    /// 要添加/替换/删除的条目
    /// </summary>
    [JsonProperty(nameof(Entries))]
    public Dictionary<string, object>? Entries { get; set; }

    /// <summary>
    /// 移动列表条目
    /// </summary>
    [JsonProperty(nameof(MoveEntries))]
    public List<MoveEntry>? MoveEntries { get; set; }

    /// <summary>
    /// 文本操作
    /// </summary>
    [JsonProperty(nameof(TextOperations))]
    public List<TextOperation>? TextOperations { get; set; }

    /// <summary>
    /// 目标字段
    /// </summary>
    [JsonProperty(nameof(TargetField))]
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
    [JsonProperty(nameof(FromFile))]
    public string FromFile { get; set; } = string.Empty;

    /// <summary>
    /// 源区域
    /// </summary>
    [JsonProperty(nameof(FromArea))]
    public Area? FromArea { get; set; }

    /// <summary>
    /// 目标区域
    /// </summary>
    [JsonProperty(nameof(ToArea))]
    public Area? ToArea { get; set; }

    /// <summary>
    /// 补丁模式
    /// </summary>
    [JsonProperty(nameof(PatchMode))]
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
    [JsonProperty(nameof(FromFile))]
    public string? FromFile { get; set; }

    /// <summary>
    /// 源区域
    /// </summary>
    [JsonProperty(nameof(FromArea))]
    public Area? FromArea { get; set; }

    /// <summary>
    /// 目标区域
    /// </summary>
    [JsonProperty(nameof(ToArea))]
    public Area? ToArea { get; set; }

    /// <summary>
    /// 补丁模式
    /// </summary>
    [JsonProperty(nameof(PatchMode))]
    public PatchMode? PatchMode { get; set; }

    /// <summary>
    /// 地图属性
    /// </summary>
    [JsonProperty(nameof(MapProperties))]
    public Dictionary<string, string>? MapProperties { get; set; }

    /// <summary>
    /// 添加NPC传送点
    /// </summary>
    [JsonProperty(nameof(AddNpcWarps))]
    public List<string>? AddNpcWarps { get; set; }

    /// <summary>
    /// 添加传送点
    /// </summary>
    [JsonProperty(nameof(AddWarps))]
    public List<string>? AddWarps { get; set; }

    /// <summary>
    /// 地图图块
    /// </summary>
    [JsonProperty(nameof(MapTiles))]
    public List<MapTile>? MapTiles { get; set; }

    /// <summary>
    /// 文本操作
    /// </summary>
    [JsonProperty(nameof(TextOperations))]
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
    [JsonProperty(nameof(FromFile))]
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
    [JsonProperty(nameof(X))]
    public int X { get; set; }

    /// <summary>
    /// Y坐标
    /// </summary>
    [JsonProperty(nameof(Y))]
    public int Y { get; set; }

    /// <summary>
    /// 宽度
    /// </summary>
    [JsonProperty(nameof(Width))]
    public int Width { get; set; }

    /// <summary>
    /// 高度
    /// </summary>
    [JsonProperty(nameof(Height))]
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
    [JsonProperty(nameof(ID))]
    public string ID { get; set; } = string.Empty;

    /// <summary>
    /// 在指定ID之前插入
    /// </summary>
    [JsonProperty(nameof(BeforeID))]
    public string? BeforeID { get; set; }

    /// <summary>
    /// 在指定ID之后插入
    /// </summary>
    [JsonProperty(nameof(AfterID))]
    public string? AfterID { get; set; }

    /// <summary>
    /// 移动到指定位置
    /// </summary>
    [JsonProperty(nameof(ToPosition))]
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
    [JsonProperty(nameof(Operation))]
    public TextOperationType Operation { get; set; } = TextOperationType.Replace;

    /// <summary>
    /// 目标字段列表
    /// </summary>
    [JsonProperty(nameof(Target))]
    public List<string> Target { get; set; } = [];

    /// <summary>
    /// 操作值
    /// </summary>
    [JsonProperty(nameof(Value))]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 分隔符
    /// </summary>
    [JsonProperty(nameof(Delimiter))]
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
    [JsonProperty(nameof(Layer))]
    public MapLayer Layer { get; set; } = MapLayer.Background;

    /// <summary>
    /// 位置信息
    /// </summary>
    [JsonProperty(nameof(Position))]
    public Area Position { get; set; } = new();

    /// <summary>
    /// 设置图块表
    /// </summary>
    [JsonProperty(nameof(SetTilesheet))]
    public string? SetTilesheet { get; set; }

    /// <summary>
    /// 设置图块索引
    /// </summary>
    [JsonProperty(nameof(SetIndex))]
    public string? SetIndex { get; set; }

    /// <summary>
    /// 设置属性
    /// </summary>
    [JsonProperty(nameof(SetProperties))]
    public Dictionary<string, string>? SetProperties { get; set; }

    /// <summary>
    /// 是否移除
    /// </summary>
    [JsonProperty(nameof(Remove))]
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
    [JsonProperty(nameof(AllowValues))]
    public string? AllowValues { get; set; }

    /// <summary>
    /// 默认值
    /// </summary>
    [JsonProperty(nameof(Default))]
    public object? Default { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [JsonProperty(nameof(Description))]
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
    [JsonProperty(nameof(Name))]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 从地图文件加载
    /// </summary>
    [JsonProperty(nameof(FromMapFile))]
    public string? FromMapFile { get; set; }

    /// <summary>
    /// 从地图数据加载
    /// </summary>
    [JsonProperty(nameof(FromMapData))]
    public string? FromMapData { get; set; }

    /// <summary>
    /// 传送到目标地点
    /// </summary>
    [JsonProperty(nameof(WarpTo))]
    public string? WarpTo { get; set; }

    /// <summary>
    /// 传送到目标X坐标
    /// </summary>
    [JsonProperty(nameof(WarpToX))]
    public int? WarpToX { get; set; }

    /// <summary>
    /// 传送到目标Y坐标
    /// </summary>
    [JsonProperty(nameof(WarpToY))]
    public int? WarpToY { get; set; }

    /// <summary>
    /// 传送到目标方向
    /// </summary>
    [JsonProperty(nameof(WarpToDirection))]
    public int? WarpToDirection { get; set; }

    /// <summary>
    /// 从源地点传送
    /// </summary>
    [JsonProperty(nameof(WarpFrom))]
    public string? WarpFrom { get; set; }

    /// <summary>
    /// 从源X坐标传送
    /// </summary>
    [JsonProperty(nameof(WarpFromX))]
    public int? WarpFromX { get; set; }

    /// <summary>
    /// 从源Y坐标传送
    /// </summary>
    [JsonProperty(nameof(WarpFromY))]
    public int? WarpFromY { get; set; }

    /// <summary>
    /// 从源方向传送
    /// </summary>
    [JsonProperty(nameof(WarpFromDirection))]
    public int? WarpFromDirection { get; set; }

    /// <summary>
    /// 地图属性
    /// </summary>
    [JsonProperty(nameof(MapProperties))]
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
    [JsonProperty(nameof(Name))]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 令牌值
    /// </summary>
    [JsonProperty(nameof(Value))]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 条件字典
    /// </summary>
    [JsonProperty(nameof(When))]
    public Dictionary<string, string>? When { get; set; }
}