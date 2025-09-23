using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ContentPatcherMaker.Core.Services.AssetWriters;

/// <summary>
/// 忽略默认可选属性的解析器
/// 基于StardewXnbHack的IgnoreDefaultOptionalPropertiesResolver
/// </summary>
public class IgnoreDefaultOptionalPropertiesResolver : DefaultContractResolver
{
    private readonly bool _omitDefaultFields;

    /// <summary>
    /// 初始化忽略默认可选属性的解析器
    /// </summary>
    /// <param name="omitDefaultFields">是否忽略默认字段</param>
    public IgnoreDefaultOptionalPropertiesResolver(bool omitDefaultFields = false)
    {
        _omitDefaultFields = omitDefaultFields;
    }

    /// <summary>
    /// 创建属性
    /// </summary>
    /// <param name="member">成员信息</param>
    /// <param name="memberSerialization">成员序列化</param>
    /// <returns>属性</returns>
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);

        if (_omitDefaultFields && ShouldIgnoreProperty(member))
        {
            property.ShouldSerialize = instance => false;
        }

        return property;
    }

    /// <summary>
    /// 检查是否应该忽略属性
    /// </summary>
    /// <param name="member">成员信息</param>
    /// <returns>是否应该忽略</returns>
    private bool ShouldIgnoreProperty(MemberInfo member)
    {
        // 这里可以添加更复杂的逻辑来判断是否应该忽略属性
        // 例如检查ContentSerializerAttribute.Optional等
        return false;
    }
}
