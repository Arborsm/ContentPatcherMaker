using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ContentPatcherMaker.Core.Models
{
    /// <summary>
    /// ContentPatcher内容包的主要模型
    /// </summary>
    public class ContentPack
    {
        /// <summary>
        /// ContentPatcher格式版本
        /// </summary>
        [Required]
        [JsonPropertyName("Format")]
        public string Format { get; set; } = "2.8.0";

        /// <summary>
        /// 补丁列表
        /// </summary>
        [JsonPropertyName("Changes")]
        public List<Patch> Changes { get; set; } = new();

        /// <summary>
        /// 配置模式定义
        /// </summary>
        [JsonPropertyName("ConfigSchema")]
        public Dictionary<string, ConfigSchemaField>? ConfigSchema { get; set; }

        /// <summary>
        /// 自定义地点定义
        /// </summary>
        [JsonPropertyName("CustomLocations")]
        public List<CustomLocation>? CustomLocations { get; set; }

        /// <summary>
        /// 动态令牌定义
        /// </summary>
        [JsonPropertyName("DynamicTokens")]
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
        [JsonPropertyName("Action")]
        public abstract string Action { get; }

        /// <summary>
        /// 目标素材名称
        /// </summary>
        [Required]
        [JsonPropertyName("Target")]
        public string Target { get; set; } = string.Empty;

        /// <summary>
        /// 条件表达式
        /// </summary>
        [JsonPropertyName("When")]
        public Dictionary<string, string>? When { get; set; }

        /// <summary>
        /// 日志名称
        /// </summary>
        [JsonPropertyName("LogName")]
        public string? LogName { get; set; }

        /// <summary>
        /// 更新频率
        /// </summary>
        [JsonPropertyName("Update")]
        public string? Update { get; set; }

        /// <summary>
        /// 局部令牌
        /// </summary>
        [JsonPropertyName("LocalTokens")]
        public Dictionary<string, string>? LocalTokens { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        [JsonPropertyName("Priority")]
        public string? Priority { get; set; }

        /// <summary>
        /// 目标语言环境
        /// </summary>
        [JsonPropertyName("TargetLocale")]
        public string? TargetLocale { get; set; }
    }

    /// <summary>
    /// Load操作补丁
    /// </summary>
    public class LoadPatch : Patch
    {
        public override string Action => "Load";

        /// <summary>
        /// 源文件路径
        /// </summary>
        [Required]
        [JsonPropertyName("FromFile")]
        public string FromFile { get; set; } = string.Empty;
    }

    /// <summary>
    /// EditData操作补丁
    /// </summary>
    public class EditDataPatch : Patch
    {
        public override string Action => "EditData";

        /// <summary>
        /// 要编辑的字段
        /// </summary>
        [JsonPropertyName("Fields")]
        public Dictionary<string, object>? Fields { get; set; }

        /// <summary>
        /// 要添加/替换/删除的条目
        /// </summary>
        [JsonPropertyName("Entries")]
        public Dictionary<string, object>? Entries { get; set; }

        /// <summary>
        /// 移动列表条目
        /// </summary>
        [JsonPropertyName("MoveEntries")]
        public List<MoveEntry>? MoveEntries { get; set; }

        /// <summary>
        /// 文本操作
        /// </summary>
        [JsonPropertyName("TextOperations")]
        public List<TextOperation>? TextOperations { get; set; }

        /// <summary>
        /// 目标字段
        /// </summary>
        [JsonPropertyName("TargetField")]
        public List<string>? TargetField { get; set; }
    }

    /// <summary>
    /// EditImage操作补丁
    /// </summary>
    public class EditImagePatch : Patch
    {
        public override string Action => "EditImage";

        /// <summary>
        /// 源文件路径
        /// </summary>
        [Required]
        [JsonPropertyName("FromFile")]
        public string FromFile { get; set; } = string.Empty;

        /// <summary>
        /// 源区域
        /// </summary>
        [JsonPropertyName("FromArea")]
        public Area? FromArea { get; set; }

        /// <summary>
        /// 目标区域
        /// </summary>
        [JsonPropertyName("ToArea")]
        public Area? ToArea { get; set; }

        /// <summary>
        /// 补丁模式
        /// </summary>
        [JsonPropertyName("PatchMode")]
        public string? PatchMode { get; set; }
    }

    /// <summary>
    /// EditMap操作补丁
    /// </summary>
    public class EditMapPatch : Patch
    {
        public override string Action => "EditMap";

        /// <summary>
        /// 源文件路径
        /// </summary>
        [JsonPropertyName("FromFile")]
        public string? FromFile { get; set; }

        /// <summary>
        /// 源区域
        /// </summary>
        [JsonPropertyName("FromArea")]
        public Area? FromArea { get; set; }

        /// <summary>
        /// 目标区域
        /// </summary>
        [JsonPropertyName("ToArea")]
        public Area? ToArea { get; set; }

        /// <summary>
        /// 补丁模式
        /// </summary>
        [JsonPropertyName("PatchMode")]
        public string? PatchMode { get; set; }

        /// <summary>
        /// 地图属性
        /// </summary>
        [JsonPropertyName("MapProperties")]
        public Dictionary<string, string>? MapProperties { get; set; }

        /// <summary>
        /// 添加NPC传送点
        /// </summary>
        [JsonPropertyName("AddNpcWarps")]
        public List<string>? AddNpcWarps { get; set; }

        /// <summary>
        /// 添加传送点
        /// </summary>
        [JsonPropertyName("AddWarps")]
        public List<string>? AddWarps { get; set; }

        /// <summary>
        /// 地图图块
        /// </summary>
        [JsonPropertyName("MapTiles")]
        public List<MapTile>? MapTiles { get; set; }

        /// <summary>
        /// 文本操作
        /// </summary>
        [JsonPropertyName("TextOperations")]
        public List<TextOperation>? TextOperations { get; set; }
    }

    /// <summary>
    /// Include操作补丁
    /// </summary>
    public class IncludePatch : Patch
    {
        public override string Action => "Include";

        /// <summary>
        /// 源文件路径
        /// </summary>
        [Required]
        [JsonPropertyName("FromFile")]
        public string FromFile { get; set; } = string.Empty;
    }

    /// <summary>
    /// 区域定义
    /// </summary>
    public class Area
    {
        [JsonPropertyName("X")]
        public int X { get; set; }

        [JsonPropertyName("Y")]
        public int Y { get; set; }

        [JsonPropertyName("Width")]
        public int Width { get; set; }

        [JsonPropertyName("Height")]
        public int Height { get; set; }
    }

    /// <summary>
    /// 移动条目定义
    /// </summary>
    public class MoveEntry
    {
        [JsonPropertyName("ID")]
        public string ID { get; set; } = string.Empty;

        [JsonPropertyName("BeforeID")]
        public string? BeforeID { get; set; }

        [JsonPropertyName("AfterID")]
        public string? AfterID { get; set; }

        [JsonPropertyName("ToPosition")]
        public string? ToPosition { get; set; }
    }

    /// <summary>
    /// 文本操作定义
    /// </summary>
    public class TextOperation
    {
        [JsonPropertyName("Operation")]
        public string Operation { get; set; } = string.Empty;

        [JsonPropertyName("Target")]
        public List<string> Target { get; set; } = new();

        [JsonPropertyName("Value")]
        public string Value { get; set; } = string.Empty;

        [JsonPropertyName("Delimiter")]
        public string? Delimiter { get; set; }
    }

    /// <summary>
    /// 地图图块定义
    /// </summary>
    public class MapTile
    {
        [JsonPropertyName("Layer")]
        public string Layer { get; set; } = string.Empty;

        [JsonPropertyName("Position")]
        public Area Position { get; set; } = new();

        [JsonPropertyName("SetTilesheet")]
        public string? SetTilesheet { get; set; }

        [JsonPropertyName("SetIndex")]
        public string? SetIndex { get; set; }

        [JsonPropertyName("SetProperties")]
        public Dictionary<string, string>? SetProperties { get; set; }

        [JsonPropertyName("Remove")]
        public bool Remove { get; set; }
    }

    /// <summary>
    /// 配置模式字段定义
    /// </summary>
    public class ConfigSchemaField
    {
        [JsonPropertyName("AllowValues")]
        public string? AllowValues { get; set; }

        [JsonPropertyName("Default")]
        public object? Default { get; set; }

        [JsonPropertyName("Description")]
        public string? Description { get; set; }
    }

    /// <summary>
    /// 自定义地点定义
    /// </summary>
    public class CustomLocation
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("FromMapFile")]
        public string? FromMapFile { get; set; }

        [JsonPropertyName("FromMapData")]
        public string? FromMapData { get; set; }

        [JsonPropertyName("WarpTo")]
        public string? WarpTo { get; set; }

        [JsonPropertyName("WarpToX")]
        public int? WarpToX { get; set; }

        [JsonPropertyName("WarpToY")]
        public int? WarpToY { get; set; }

        [JsonPropertyName("WarpToDirection")]
        public int? WarpToDirection { get; set; }

        [JsonPropertyName("WarpFrom")]
        public string? WarpFrom { get; set; }

        [JsonPropertyName("WarpFromX")]
        public int? WarpFromX { get; set; }

        [JsonPropertyName("WarpFromY")]
        public int? WarpFromY { get; set; }

        [JsonPropertyName("WarpFromDirection")]
        public int? WarpFromDirection { get; set; }

        [JsonPropertyName("MapProperties")]
        public Dictionary<string, string>? MapProperties { get; set; }
    }

    /// <summary>
    /// 动态令牌定义
    /// </summary>
    public class DynamicToken
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("Value")]
        public string Value { get; set; } = string.Empty;

        [JsonPropertyName("When")]
        public Dictionary<string, string>? When { get; set; }
    }
}

