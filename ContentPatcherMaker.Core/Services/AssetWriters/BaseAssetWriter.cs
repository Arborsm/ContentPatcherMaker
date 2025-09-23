using Newtonsoft.Json;

namespace ContentPatcherMaker.Core.Services.AssetWriters;

/// <summary>
/// 资产写入器基类
/// 基于StardewXnbHack的BaseAssetWriter
/// </summary>
public abstract class BaseAssetWriter : IAssetWriter
{
    private readonly Lazy<JsonSerializerSettings> _jsonSettings;

    /// <summary>
    /// 初始化资产写入器基类
    /// </summary>
    /// <param name="omitDefaultFields">是否忽略默认字段</param>
    protected BaseAssetWriter(bool omitDefaultFields = false)
    {
        _jsonSettings = new(() => GetJsonSerializerSettings(omitDefaultFields));
    }

    /// <summary>
    /// 检查是否可以处理指定的资产
    /// </summary>
    /// <param name="asset">资产对象</param>
    /// <returns>是否可以处理</returns>
    public abstract bool CanWrite(object asset);

    /// <summary>
    /// 将资产写入文件
    /// </summary>
    /// <param name="asset">资产对象</param>
    /// <param name="toPathWithoutExtension">输出路径（不含扩展名）</param>
    /// <param name="relativePath">相对路径</param>
    /// <param name="platform">平台</param>
    /// <param name="error">错误信息</param>
    /// <returns>是否写入成功</returns>
    public abstract bool TryWriteFile(object asset, string toPathWithoutExtension, string relativePath, Platform platform, out string error);

    /// <summary>
    /// 格式化数据为文本
    /// </summary>
    /// <param name="asset">资产对象</param>
    /// <returns>格式化后的文本</returns>
    protected string FormatData(object asset)
    {
        return JsonConvert.SerializeObject(asset, _jsonSettings.Value);
    }

    /// <summary>
    /// 获取数据文件扩展名
    /// </summary>
    /// <returns>文件扩展名</returns>
    protected string GetDataExtension()
    {
        return "json";
    }

    /// <summary>
    /// 获取JSON序列化设置
    /// </summary>
    /// <param name="omitDefaultFields">是否忽略默认字段</param>
    /// <returns>JSON序列化设置</returns>
    private static JsonSerializerSettings GetJsonSerializerSettings(bool omitDefaultFields = false)
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        if (omitDefaultFields)
        {
            settings.ContractResolver = new IgnoreDefaultOptionalPropertiesResolver();
        }

        return settings;
    }
}
