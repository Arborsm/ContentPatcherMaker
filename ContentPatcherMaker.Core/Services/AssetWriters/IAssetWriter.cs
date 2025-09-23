namespace ContentPatcherMaker.Core.Services.AssetWriters;

/// <summary>
/// 资产写入器接口
/// 基于StardewXnbHack的IAssetWriter
/// </summary>
public interface IAssetWriter
{
    /// <summary>
    /// 检查是否可以处理指定的资产
    /// </summary>
    /// <param name="asset">资产对象</param>
    /// <returns>是否可以处理</returns>
    bool CanWrite(object asset);

    /// <summary>
    /// 将资产写入文件
    /// </summary>
    /// <param name="asset">资产对象</param>
    /// <param name="toPathWithoutExtension">输出路径（不含扩展名）</param>
    /// <param name="relativePath">相对路径</param>
    /// <param name="platform">平台</param>
    /// <param name="error">错误信息</param>
    /// <returns>是否写入成功</returns>
    bool TryWriteFile(object asset, string toPathWithoutExtension, string relativePath, Platform platform, out string error);
}
