using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ContentPatcherMaker.Core.DataModels;

/// <summary>
/// 农场数据模型
/// 基于Content/Data/AdditionalFarms.json文件
/// </summary>
public record FarmData : IDataModel
{
    /// <summary>
    /// 农场ID
    /// </summary>
    [JsonProperty("Id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// 农场名称
    /// </summary>
    [JsonProperty("Name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// 工具提示字符串路径
    /// </summary>
    [JsonProperty("TooltipStringPath")]
    public string? TooltipStringPath { get; init; }

    /// <summary>
    /// 地图名称
    /// </summary>
    [JsonProperty("MapName")]
    public string? MapName { get; init; }

    /// <summary>
    /// 图标纹理
    /// </summary>
    [JsonProperty("IconTexture")]
    public string? IconTexture { get; init; }

    /// <summary>
    /// 世界地图纹理
    /// </summary>
    [JsonProperty("WorldMapTexture")]
    public string? WorldMapTexture { get; init; }

    /// <summary>
    /// 是否默认生成怪物
    /// </summary>
    [JsonProperty("SpawnMonstersByDefault")]
    public bool SpawnMonstersByDefault { get; init; }

    /// <summary>
    /// 模组数据
    /// </summary>
    [JsonProperty("ModData")]
    public object? ModData { get; init; }

    /// <summary>
    /// 自定义字段
    /// </summary>
    [JsonProperty("CustomFields")]
    public object? CustomFields { get; init; }

    /// <summary>
    /// 农场描述
    /// </summary>
    public string? Description => TooltipStringPath;

    /// <summary>
    /// 验证农场数据
    /// </summary>
    /// <returns>验证结果</returns>
    public ValidationResult Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Id))
            errors.Add("农场ID不能为空");

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("农场名称不能为空");

        if (string.IsNullOrWhiteSpace(MapName))
            errors.Add("地图名称不能为空");

        return new ValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors
        };
    }
}

/// <summary>
/// 农场数据集合
/// </summary>
public class FarmDataCollection : IDataModelCollection<FarmData>
{
    private readonly Dictionary<string, FarmData> _farms = new();

    /// <summary>
    /// 初始化农场数据集合
    /// </summary>
    /// <param name="farms">农场数据列表</param>
    public FarmDataCollection(IEnumerable<FarmData> farms)
    {
        foreach (var farm in farms)
        {
            if (!string.IsNullOrWhiteSpace(farm.Id))
            {
                _farms[farm.Id] = farm;
            }
        }
    }

    /// <summary>
    /// 获取所有农场
    /// </summary>
    public IEnumerable<FarmData> GetAll() => _farms.Values;

    /// <summary>
    /// 根据ID获取农场
    /// </summary>
    /// <param name="id">农场ID</param>
    /// <returns>农场数据，如果不存在则返回null</returns>
    public FarmData? GetById(string id)
    {
        return _farms.TryGetValue(id, out var farm) ? farm : null;
    }

    /// <summary>
    /// 检查农场是否存在
    /// </summary>
    /// <param name="id">农场ID</param>
    /// <returns>是否存在</returns>
    public bool Exists(string id) => _farms.ContainsKey(id);

    /// <summary>
    /// 搜索农场
    /// </summary>
    /// <param name="predicate">搜索条件</param>
    /// <returns>匹配的农场列表</returns>
    public IEnumerable<FarmData> Search(Func<FarmData, bool> predicate)
    {
        return _farms.Values.Where(predicate);
    }

    /// <summary>
    /// 获取默认生成怪物的农场
    /// </summary>
    /// <returns>默认生成怪物的农场列表</returns>
    public IEnumerable<FarmData> GetFarmsWithMonsters()
    {
        return _farms.Values.Where(f => f.SpawnMonstersByDefault);
    }

    /// <summary>
    /// 获取不生成怪物的农场
    /// </summary>
    /// <returns>不生成怪物的农场列表</returns>
    public IEnumerable<FarmData> GetFarmsWithoutMonsters()
    {
        return _farms.Values.Where(f => !f.SpawnMonstersByDefault);
    }

    /// <summary>
    /// 根据地图名称获取农场
    /// </summary>
    /// <param name="mapName">地图名称</param>
    /// <returns>匹配的农场列表</returns>
    public IEnumerable<FarmData> GetFarmsByMapName(string mapName)
    {
        return _farms.Values.Where(f => f.MapName == mapName);
    }
}
