namespace ContentPatcherMaker.Core.Services.AssetWriters;

/// <summary>
/// 数据资产写入器
/// 基于StardewXnbHack的DataWriter
/// </summary>
public class DataAssetWriter : BaseAssetWriter
{
    /// <summary>
    /// 初始化数据资产写入器
    /// </summary>
    /// <param name="omitDefaultFields">是否忽略默认字段</param>
    public DataAssetWriter(bool omitDefaultFields = false) : base(omitDefaultFields)
    {
    }

    /// <summary>
    /// 检查是否可以处理指定的资产
    /// </summary>
    /// <param name="asset">资产对象</param>
    /// <returns>是否可以处理</returns>
    public override bool CanWrite(object asset)
    {
        var type = asset.GetType();
        type = type.IsGenericType ? type.GetGenericTypeDefinition() : type;

        return type == typeof(Dictionary<,>) ||
               type == typeof(List<>) ||
               type.FullName?.StartsWith("StardewValley.GameData.") == true;
    }

    /// <summary>
    /// 将资产写入文件
    /// </summary>
    /// <param name="asset">资产对象</param>
    /// <param name="toPathWithoutExtension">输出路径（不含扩展名）</param>
    /// <param name="relativePath">相对路径</param>
    /// <param name="platform">平台</param>
    /// <param name="error">错误信息</param>
    /// <returns>是否写入成功</returns>
    public override bool TryWriteFile(object asset, string toPathWithoutExtension, string relativePath, Platform platform, out string error)
    {
        try
        {
            var jsonContent = FormatData(asset);
            var filePath = $"{toPathWithoutExtension}.{GetDataExtension()}";
            File.WriteAllText(filePath, jsonContent);
            
            error = string.Empty;
            return true;
        }
        catch (Exception ex)
        {
            error = $"写入数据文件失败: {ex.Message}";
            return false;
        }
    }
}
