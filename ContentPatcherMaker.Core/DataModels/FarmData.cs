using StardewValley.GameData;

namespace ContentPatcherMaker.Core.DataModels;

/// <summary>
/// 农场数据集合
/// </summary>
public class FarmDataCollection : DataModelCollection<ModFarmType>
{
    private readonly Dictionary<string, ModFarmType> _farms = new();

    /// <summary>
    /// 初始化农场数据集合
    /// </summary>
    /// <param name="farms">农场数据列表</param>
    public FarmDataCollection(IEnumerable<ModFarmType> farms)
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
    public override IEnumerable<ModFarmType> GetAll() => _farms.Values;

    /// <summary>
    /// 根据ID获取农场
    /// </summary>
    /// <param name="id">农场ID</param>
    /// <returns>农场数据，如果不存在则返回null</returns>
    public override ModFarmType? GetById(string id) => _farms.GetValueOrDefault(id);

    /// <summary>
    /// 检查农场是否存在
    /// </summary>
    /// <param name="id">农场ID</param>
    /// <returns>是否存在</returns>
    public override bool Exists(string id) => _farms.ContainsKey(id);

    /// <summary>
    /// 搜索农场
    /// </summary>
    /// <param name="predicate">搜索条件</param>
    /// <returns>匹配的农场列表</returns>
    public override IEnumerable<ModFarmType> Search(Func<ModFarmType, bool> predicate) => _farms.Values.Where(predicate);

    /// <summary>
    /// 获取默认生成怪物的农场
    /// </summary>
    /// <returns>默认生成怪物的农场列表</returns>
    public IEnumerable<ModFarmType> GetFarmsWithMonsters() => _farms.Values.Where(f => f.SpawnMonstersByDefault);

    /// <summary>
    /// 获取不生成怪物的农场
    /// </summary>
    /// <returns>不生成怪物的农场列表</returns>
    public IEnumerable<ModFarmType> GetFarmsWithoutMonsters() => _farms.Values.Where(f => !f.SpawnMonstersByDefault);

    /// <summary>
    /// 根据地图名称获取农场
    /// </summary>
    /// <param name="mapName">地图名称</param>
    /// <returns>匹配的农场列表</returns>
    public IEnumerable<ModFarmType> GetFarmsByMapName(string mapName) => _farms.Values.Where(f => f.MapName == mapName);
}
