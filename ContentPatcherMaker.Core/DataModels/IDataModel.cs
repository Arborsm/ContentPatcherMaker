using System.ComponentModel.DataAnnotations;

namespace ContentPatcherMaker.Core.DataModels;

/// <summary>
/// 数据模型基础接口
/// </summary>
public interface IDataModel
{
    /// <summary>
    /// 获取模型ID
    /// </summary>
    string Id { get; }

    /// <summary>
    /// 获取模型名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 获取模型描述
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// 验证模型数据
    /// </summary>
    /// <returns>验证结果</returns>
    ValidationResult Validate();
}

/// <summary>
/// 可枚举的数据模型接口
/// </summary>
/// <typeparam name="T">枚举类型</typeparam>
public interface IEnumeratedDataModel<T> : IDataModel where T : Enum
{
    /// <summary>
    /// 获取枚举值
    /// </summary>
    T EnumValue { get; }
}

/// <summary>
/// 数据模型集合接口
/// </summary>
/// <typeparam name="TModel">数据模型类型</typeparam>
public interface IDataModelCollection<TModel> where TModel : IDataModel
{
    /// <summary>
    /// 获取所有模型
    /// </summary>
    IEnumerable<TModel> GetAll();

    /// <summary>
    /// 根据ID获取模型
    /// </summary>
    /// <param name="id">模型ID</param>
    /// <returns>模型，如果不存在则返回null</returns>
    TModel? GetById(string id);

    /// <summary>
    /// 检查模型是否存在
    /// </summary>
    /// <param name="id">模型ID</param>
    /// <returns>是否存在</returns>
    bool Exists(string id);

    /// <summary>
    /// 搜索模型
    /// </summary>
    /// <param name="predicate">搜索条件</param>
    /// <returns>匹配的模型列表</returns>
    IEnumerable<TModel> Search(Func<TModel, bool> predicate);
}
