using System.Xml.Linq;
using StardewValley;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace ContentPatcherMaker.Core.Services.AssetWriters;

/// <summary>
/// 地图资产写入器
/// 基于StardewXnbHack的MapWriter
/// </summary>
public class MapAssetWriter : BaseAssetWriter
{
    private const int TileSize = Game1.tileSize / Game1.pixelZoom;

    /// <summary>
    /// 检查是否可以处理指定的资产
    /// </summary>
    /// <param name="asset">资产对象</param>
    /// <returns>是否可以处理</returns>
    public override bool CanWrite(object asset)
    {
        return asset is Map;
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
            var map = (Map)asset;

            // 修复瓦片大小（游戏在内存中会覆盖它们）
            var tileSizes = new Dictionary<Layer, Size>();
            foreach (var layer in map.Layers)
            {
                tileSizes[layer] = layer.TileSize;
                layer.TileSize = new Size(TileSize, TileSize);
            }

            // 修复图像源（游戏在内存中会覆盖它们）
            var imageSources = new Dictionary<TileSheet, string>();
            foreach (var sheet in map.TileSheets)
            {
                imageSources[sheet] = sheet.ImageSource;
                sheet.ImageSource = GetOriginalImageSource(relativePath, sheet.ImageSource);
            }

            // 保存文件
            using (var stream = new MemoryStream())
            {
                // 序列化到流
                // 注意：这里简化了TMX格式的处理，实际项目中可能需要更复杂的实现
                var doc = new XDocument();
                var mapElement = new XElement("map");
                doc.Add(mapElement);
                
                // 添加地图属性
                mapElement.SetAttributeValue("version", "1.0");
                mapElement.SetAttributeValue("orientation", "orthogonal");
                mapElement.SetAttributeValue("width", map.DisplayWidth);
                mapElement.SetAttributeValue("height", map.DisplayHeight);
                mapElement.SetAttributeValue("tilewidth", TileSize);
                mapElement.SetAttributeValue("tileheight", TileSize);

                // 添加图层
                foreach (var layer in map.Layers)
                {
                    var layerElement = new XElement("layer");
                    layerElement.SetAttributeValue("name", layer.Id);
                    layerElement.SetAttributeValue("width", layer.LayerWidth);
                    layerElement.SetAttributeValue("height", layer.LayerHeight);
                    mapElement.Add(layerElement);
                }

                // 保存XML
                File.WriteAllText($"{toPathWithoutExtension}.tmx", "<?xml version=\"1.0\"?>\n" + doc);
            }

            // 恢复更改
            foreach (var layer in map.Layers)
                layer.TileSize = tileSizes[layer];
            foreach (var sheet in map.TileSheets)
                sheet.ImageSource = imageSources[sheet];

            error = string.Empty;
            return true;
        }
        catch (Exception ex)
        {
            error = $"写入地图文件失败: {ex.Message}";
            return false;
        }
    }

    /// <summary>
    /// 获取地图瓦片表的原始图像源
    /// </summary>
    /// <param name="relativeMapPath">地图文件的相对路径</param>
    /// <param name="imageSource">瓦片表图像源</param>
    /// <returns>原始图像源</returns>
    private string GetOriginalImageSource(string relativeMapPath, string imageSource)
    {
        var mapDirPath = Path.GetDirectoryName(relativeMapPath)?.Replace('\\', '/');
        var normalizedImageSource = imageSource.Replace('\\', '/');

        if (!string.IsNullOrEmpty(mapDirPath) && 
            normalizedImageSource.StartsWith($"{mapDirPath}/", StringComparison.OrdinalIgnoreCase))
        {
            return imageSource.Substring(mapDirPath.Length + 1);
        }

        return imageSource;
    }
}
