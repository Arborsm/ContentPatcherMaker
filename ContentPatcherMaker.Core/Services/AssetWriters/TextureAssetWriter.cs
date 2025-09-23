using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ContentPatcherMaker.Core.Services.AssetWriters;

/// <summary>
/// 纹理资产写入器
/// 基于StardewXnbHack的TextureWriter
/// </summary>
public class TextureAssetWriter : BaseAssetWriter
{
    /// <summary>
    /// 检查是否可以处理指定的资产
    /// </summary>
    /// <param name="asset">资产对象</param>
    /// <returns>是否可以处理</returns>
    public override bool CanWrite(object asset)
    {
        return asset is Texture2D;
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
            var texture = (Texture2D)asset;
            
            // 反预乘透明度
            UnpremultiplyTransparency(texture);
            
            // 保存为PNG
            using (var stream = File.Create($"{toPathWithoutExtension}.png"))
            {
                texture.SaveAsPng(stream, texture.Width, texture.Height);
            }
            
            error = string.Empty;
            return true;
        }
        catch (Exception ex)
        {
            error = $"写入纹理文件失败: {ex.Message}";
            return false;
        }
    }

    /// <summary>
    /// 反预乘透明度
    /// 基于StardewXnbHack的UnpremultiplyTransparency方法
    /// </summary>
    /// <param name="texture">纹理对象</param>
    private void UnpremultiplyTransparency(Texture2D texture)
    {
        var data = new Color[texture.Width * texture.Height];
        texture.GetData(data);

        for (int i = 0; i < data.Length; i++)
        {
            var pixel = data[i];
            if (pixel.A == 0)
                continue;

            data[i] = new Color(
                (byte)((pixel.R * 255) / pixel.A),
                (byte)((pixel.G * 255) / pixel.A),
                (byte)((pixel.B * 255) / pixel.A),
                pixel.A
            );
        }

        texture.SetData(data);
    }
}
