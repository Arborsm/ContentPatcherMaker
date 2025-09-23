using System.Xml.Linq;

namespace ContentPatcherMaker.Core.Services.AssetWriters;

/// <summary>
/// XML资产写入器
/// 基于StardewXnbHack的XmlSourceWriter
/// </summary>
public class XmlAssetWriter : BaseAssetWriter
{
    /// <summary>
    /// 检查是否可以处理指定的资产
    /// </summary>
    /// <param name="asset">资产对象</param>
    /// <returns>是否可以处理</returns>
    public override bool CanWrite(object asset)
    {
        return asset is XDocument || asset is XElement;
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
            XDocument document;
            
            if (asset is XDocument doc)
            {
                document = doc;
            }
            else if (asset is XElement element)
            {
                document = new XDocument(element);
            }
            else
            {
                error = "不支持的XML资产类型";
                return false;
            }

            // 保存XML文件
            var filePath = $"{toPathWithoutExtension}.xml";
            document.Save(filePath);
            
            error = string.Empty;
            return true;
        }
        catch (Exception ex)
        {
            error = $"写入XML文件失败: {ex.Message}";
            return false;
        }
    }
}
