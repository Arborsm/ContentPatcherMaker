using ContentPatcherMaker.Core.Services;

namespace ContentPatcherMaker.Core.DataModels;

/// <summary>
/// 数据模型集合基类
/// </summary>
public abstract class DataModelCollectionBase
{
    internal DataModelCollectionBase() => DataModelManager.RegisterCollection(this);
}

/// <summary>
/// 数据模型集合接口
/// </summary>
/// <typeparam name="TModel">数据模型类型</typeparam>
public abstract class DataModelCollection<TModel>: DataModelCollectionBase
{
    /// <summary>
    /// 获取所有模型
    /// </summary>
    public abstract IEnumerable<TModel> GetAll();

    /// <summary>
    /// 根据ID获取模型
    /// </summary>
    /// <param name="id">模型ID</param>
    /// <returns>模型，如果不存在则返回null</returns>
    public abstract TModel? GetById(string id);

    /// <summary>
    /// 检查模型是否存在
    /// </summary>
    /// <param name="id">模型ID</param>
    /// <returns>是否存在</returns>
    public abstract bool Exists(string id);

    /// <summary>
    /// 搜索模型
    /// </summary>
    /// <param name="predicate">搜索条件</param>
    /// <returns>匹配的模型列表</returns>
    public abstract IEnumerable<TModel> Search(Func<TModel, bool> predicate);
}