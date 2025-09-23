using System.Diagnostics.CodeAnalysis;
using ContentPatcherMaker.Core.Services.AssetWriters;
using ContentPatcherMaker.Core.Services.Logging;
using Microsoft.Xna.Framework.Content;

namespace ContentPatcherMaker.Core.Services;

/// <summary>
/// XNB解包服务
/// 基于StardewXnbHack的XNB解包功能
/// </summary>
public class XnbUnpackingService
{
    private readonly LoggingService _loggingService;
    private readonly List<IAssetWriter> _assetWriters;

    /// <summary>
    /// 初始化XNB解包服务
    /// </summary>
    /// <param name="loggingService">日志服务</param>
    public XnbUnpackingService(LoggingService loggingService)
    {
        _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        _assetWriters =
        [
            new DataAssetWriter(),
            new TextureAssetWriter(),
            new MapAssetWriter(),
            new SpriteFontAssetWriter(),
            new XmlAssetWriter()
        ];
    }

    /// <summary>
    /// 解包XNB文件
    /// </summary>
    /// <param name="xnbFilePath">XNB文件路径</param>
    /// <param name="outputPath">输出路径（不含扩展名）</param>
    /// <param name="contentManager">内容管理器</param>
    /// <returns>解包结果</returns>
    public XnbUnpackingResult UnpackXnbFile(string xnbFilePath, string outputPath, ContentManager contentManager)
    {
        try
        {
            _loggingService.LogInformation($"开始解包XNB文件: {xnbFilePath}", "XnbUnpackingService");

            if (!File.Exists(xnbFilePath))
            {
                return new XnbUnpackingResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"XNB文件不存在: {xnbFilePath}"
                };
            }

            // 获取相对路径
            var relativePath = Path.GetFileNameWithoutExtension(xnbFilePath);
            
            // 加载资源
            object asset;
            try
            {
                asset = contentManager.Load<object>(relativePath);
            }
            catch (Exception ex)
            {
                _loggingService.LogError($"加载XNB文件失败: {ex.Message}", ex, "XnbUnpackingService");
                return new XnbUnpackingResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"加载XNB文件失败: {ex.Message}"
                };
            }

            // 查找合适的写入器
            var writer = _assetWriters.FirstOrDefault(w => w.CanWrite(asset));
            if (writer == null)
            {
                _loggingService.LogWarning($"未找到合适的写入器处理资源类型: {asset.GetType().Name}", "XnbUnpackingService");
                return new XnbUnpackingResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"不支持的资源类型: {asset.GetType().Name}"
                };
            }

            // 写入文件
            var success = writer.TryWriteFile(asset, outputPath, relativePath, Platform.Windows, out var error);
            if (!success)
            {
                _loggingService.LogError($"写入文件失败: {error}", context: "XnbUnpackingService");
                return new XnbUnpackingResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"写入文件失败: {error}"
                };
            }

            _loggingService.LogInformation($"成功解包XNB文件: {xnbFilePath} -> {outputPath}", "XnbUnpackingService");
            return new XnbUnpackingResult
            {
                IsSuccess = true,
                OutputPath = outputPath,
                AssetType = asset.GetType().Name
            };
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"解包XNB文件时发生异常: {ex.Message}", ex, "XnbUnpackingService");
            return new XnbUnpackingResult
            {
                IsSuccess = false,
                ErrorMessage = $"解包XNB文件时发生异常: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// 批量解包XNB文件
    /// </summary>
    /// <param name="xnbFiles">XNB文件路径列表</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="contentManager">内容管理器</param>
    /// <returns>解包结果列表</returns>
    public List<XnbUnpackingResult> UnpackXnbFiles(IEnumerable<string> xnbFiles, string outputDirectory,
        ContentManager contentManager)
    {
        var results = new List<XnbUnpackingResult>();
        
        foreach (var xnbFile in xnbFiles)
        {
            var fileName = Path.GetFileNameWithoutExtension(xnbFile);
            var outputPath = Path.Combine(outputDirectory, fileName);
            
            // 确保输出目录存在
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            
            var result = UnpackXnbFile(xnbFile, outputPath, contentManager);
            results.Add(result);
        }

        return results;
    }

    /// <summary>
    /// 解包Content目录下的所有XNB文件
    /// </summary>
    /// <param name="contentDirectory">Content目录路径</param>
    /// <param name="outputDirectory">输出目录</param>
    /// <param name="contentManager">内容管理器</param>
    /// <returns>解包结果</returns>
    public XnbUnpackingResult UnpackAllXnbFiles(string contentDirectory, string outputDirectory,
        ContentManager contentManager)
    {
        try
        {
            _loggingService.LogInformation($"开始解包Content目录: {contentDirectory}", "XnbUnpackingService");

            if (!Directory.Exists(contentDirectory))
            {
                return new XnbUnpackingResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Content目录不存在: {contentDirectory}"
                };
            }

            // 查找所有XNB文件
            var xnbFiles = Directory.GetFiles(contentDirectory, "*.xnb", SearchOption.AllDirectories);
            _loggingService.LogInformation($"找到 {xnbFiles.Length} 个XNB文件", "XnbUnpackingService");

            var results = UnpackXnbFiles(xnbFiles, outputDirectory, contentManager);
            
            var successCount = results.Count(r => r.IsSuccess);
            var failureCount = results.Count(r => !r.IsSuccess);

            _loggingService.LogInformation($"解包完成: 成功 {successCount} 个，失败 {failureCount} 个", "XnbUnpackingService");

            return new XnbUnpackingResult
            {
                IsSuccess = true,
                OutputPath = outputDirectory,
                AssetType = "Multiple",
                SuccessCount = successCount,
                FailureCount = failureCount,
                DetailedResults = results
            };
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"解包Content目录时发生异常: {ex.Message}", ex, "XnbUnpackingService");
            return new XnbUnpackingResult
            {
                IsSuccess = false,
                ErrorMessage = $"解包Content目录时发生异常: {ex.Message}"
            };
        }
    }
}

/// <summary>
/// XNB解包结果
/// </summary>
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class XnbUnpackingResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 输出路径
    /// </summary>
    public string? OutputPath { get; set; }

    /// <summary>
    /// 资源类型
    /// </summary>
    public string? AssetType { get; set; }

    /// <summary>
    /// 成功数量（批量解包时）
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// 失败数量（批量解包时）
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// 详细结果列表（批量解包时）
    /// </summary>
    public List<XnbUnpackingResult>? DetailedResults { get; set; }
}
