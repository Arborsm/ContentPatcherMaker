using Microsoft.Xna.Framework.Graphics;

namespace ContentPatcherMaker.Core.Services.AssetWriters;

/// <summary>
/// 精灵字体资产写入器
/// 基于StardewXnbHack的SpriteFontWriter
/// </summary>
public class SpriteFontAssetWriter : BaseAssetWriter
{
    /// <summary>
    /// 检查是否可以处理指定的资产
    /// </summary>
    /// <param name="asset">资产对象</param>
    /// <returns>是否可以处理</returns>
    public override bool CanWrite(object asset)
    {
        return asset is SpriteFont;
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
            var spriteFont = (SpriteFont)asset;
            
            // 将SpriteFont序列化为JSON
            var fontData = new
            {
                TextureName = spriteFont.Texture?.Name ?? "Unknown",
                Glyphs = GetPropertyValue(spriteFont, "Glyphs"),
                Cropping = GetPropertyValue(spriteFont, "Cropping"),
                CharacterMap = GetPropertyValue(spriteFont, "CharacterMap"),
                VerticalLineSpacing = spriteFont.LineSpacing,
                HorizontalSpacing = spriteFont.Spacing,
                Kerning = GetPropertyValue(spriteFont, "Kerning"),
                spriteFont.DefaultCharacter
            };

            var jsonContent = FormatData(fontData);
            var filePath = $"{toPathWithoutExtension}.{GetDataExtension()}";
            File.WriteAllText(filePath, jsonContent);
            
            error = string.Empty;
            return true;
        }
        catch (Exception ex)
        {
            error = $"写入精灵字体文件失败: {ex.Message}";
            return false;
        }
    }

    /// <summary>
    /// 使用反射安全地获取属性值
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="propertyName">属性名</param>
    /// <returns>属性值，如果不存在则返回null</returns>
    private static object? GetPropertyValue(object obj, string propertyName)
    {
        try
        {
            var property = obj.GetType().GetProperty(propertyName);
            return property?.GetValue(obj);
        }
        catch
        {
            return null;
        }
    }
}
