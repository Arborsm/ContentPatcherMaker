using ContentPatcherMaker.Core.Services;
using ContentPatcherMaker.Core.Services.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ContentPatcherMaker.Core.Examples;

/// <summary>
/// XNB解包示例
/// 演示如何使用XNB解包服务
/// </summary>
public class XnbUnpackingExample
{
    private readonly XnbUnpackingService _unpackingService;
    private readonly GamePathDetectionService _pathDetectionService;

    /// <summary>
    /// 初始化XNB解包示例
    /// </summary>
    public XnbUnpackingExample()
    {
        var loggingService = new LoggingService();
        _unpackingService = new XnbUnpackingService(loggingService);
        _pathDetectionService = new GamePathDetectionService(loggingService);
    }

    /// <summary>
    /// 示例1：解包单个XNB文件
    /// </summary>
    public void Example1_UnpackSingleFile(string xnbFilePath, string outputPath)
    {
        Console.WriteLine("=== 示例1：解包单个XNB文件 ===");
        
        try
        {
            // 创建临时的ContentManager
            using var game = new XnaGame();
            var contentManager = new ContentManager(game.Services, "Content");
            
            // 解包XNB文件
            var result = _unpackingService.UnpackXnbFile(xnbFilePath, outputPath, contentManager);
            
            if (result.IsSuccess)
            {
                Console.WriteLine($"✅ 解包成功: {result.OutputPath}");
                Console.WriteLine($"资源类型: {result.AssetType}");
            }
            else
            {
                Console.WriteLine($"❌ 解包失败: {result.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 发生异常: {ex.Message}");
        }
    }

    /// <summary>
    /// 示例2：解包Content目录下的所有XNB文件
    /// </summary>
    public void Example2_UnpackAllFiles(string contentDirectory, string outputDirectory)
    {
        Console.WriteLine("=== 示例2：解包Content目录下的所有XNB文件 ===");
        
        try
        {
            // 创建临时的ContentManager
            using var game = new XnaGame();
            var contentManager = new ContentManager(game.Services, contentDirectory);
            
            // 解包所有XNB文件
            var result = _unpackingService.UnpackAllXnbFiles(contentDirectory, outputDirectory, contentManager);
            
            if (result.IsSuccess)
            {
                Console.WriteLine($"✅ 解包完成: {result.OutputPath}");
                Console.WriteLine($"成功: {result.SuccessCount} 个文件");
                Console.WriteLine($"失败: {result.FailureCount} 个文件");

                if (result.DetailedResults == null) return;
                Console.WriteLine("\n详细结果:");
                foreach (var detail in result.DetailedResults)
                {
                    var status = detail.IsSuccess ? "✅" : "❌";
                    Console.WriteLine($"  {status} {Path.GetFileName(detail.OutputPath)} - {detail.AssetType}");
                    if (!detail.IsSuccess)
                    {
                        Console.WriteLine($"    错误: {detail.ErrorMessage}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"❌ 解包失败: {result.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 发生异常: {ex.Message}");
        }
    }

    /// <summary>
    /// 示例3：自动检测游戏路径并解包
    /// </summary>
    public void Example3_AutoDetectAndUnpack(string outputDirectory)
    {
        Console.WriteLine("=== 示例3：自动检测游戏路径并解包 ===");
        
        try
        {
            // 检测游戏路径
            var detectionResult = _pathDetectionService.DetectGamePaths();
            
            if (!detectionResult.IsSuccess)
            {
                Console.WriteLine($"❌ 游戏路径检测失败: {detectionResult.ErrorMessage}");
                return;
            }

            Console.WriteLine($"游戏路径: {detectionResult.GamePath}");
            Console.WriteLine($"内容路径: {detectionResult.ContentPath}");
            Console.WriteLine($"平台: {detectionResult.Platform}");

            // 解包Content目录
            Example2_UnpackAllFiles(detectionResult.ContentPath!, outputDirectory);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 发生异常: {ex.Message}");
        }
    }

    /// <summary>
    /// 示例4：解包特定类型的文件
    /// </summary>
    public void Example4_UnpackSpecificTypes(string contentDirectory, string outputDirectory,
        string[] filePatterns)
    {
        Console.WriteLine("=== 示例4：解包特定类型的文件 ===");
        
        try
        {
            // 创建临时的ContentManager
            using var game = new XnaGame();
            var contentManager = new ContentManager(game.Services, contentDirectory);
            
            // 查找特定模式的文件
            var xnbFiles = new List<string>();
            foreach (var pattern in filePatterns)
            {
                var files = Directory.GetFiles(contentDirectory, pattern, SearchOption.AllDirectories);
                xnbFiles.AddRange(files);
            }

            Console.WriteLine($"找到 {xnbFiles.Count} 个匹配的文件");

            // 解包文件
            var results = _unpackingService.UnpackXnbFiles(xnbFiles, outputDirectory, contentManager);
            
            var successCount = results.Count(r => r.IsSuccess);
            var failureCount = results.Count(r => !r.IsSuccess);

            Console.WriteLine($"解包完成: 成功 {successCount} 个，失败 {failureCount} 个");

            // 显示结果
            foreach (var result in results)
            {
                var status = result.IsSuccess ? "✅" : "❌";
                var fileName = Path.GetFileName(result.OutputPath ?? "未知");
                Console.WriteLine($"  {status} {fileName} - {result.AssetType}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 发生异常: {ex.Message}");
        }
    }

    /// <summary>
    /// 运行所有示例
    /// </summary>
    public void RunAllExamplesAsync()
    {
        Console.WriteLine("开始运行XNB解包示例...\n");

        // 示例3：自动检测并解包
        Example3_AutoDetectAndUnpack("unpacked_content");

        Console.WriteLine("\n所有示例运行完成！");
    }

    /// <summary>
    /// 运行指定路径的示例
    /// </summary>
    public void RunWithSpecifiedPathAsync(string gamePath, string outputDirectory)
    {
        Console.WriteLine($"使用指定游戏路径运行示例: {gamePath}\n");

        // 检测游戏路径
        var detectionResult = _pathDetectionService.DetectGamePaths(gamePath);
        
        if (detectionResult.IsSuccess)
        {
            // 解包Content目录
            Example2_UnpackAllFiles(detectionResult.ContentPath!, outputDirectory);
            
            // 解包特定类型的文件
            Example4_UnpackSpecificTypes(
                detectionResult.ContentPath!, 
                Path.Combine(outputDirectory, "specific"), 
                new[] { "*.xnb" });
        }
        else
        {
            Console.WriteLine($"❌ 游戏路径检测失败: {detectionResult.ErrorMessage}");
        }

        Console.WriteLine("\n指定路径示例运行完成！");
    }
}

/// <summary>
/// 简单的Game类用于创建ContentManager
/// </summary>
public class XnaGame : Game
{
    /// <inheritdoc cref="Services" />
    public new GameServiceContainer Services { get; }

    /// <inheritdoc />
    public XnaGame()
    {
        Services = new GameServiceContainer();
        
        // 添加图形设备服务
        var graphicsDeviceManager = new GraphicsDeviceManager(this);
        Services.AddService<IGraphicsDeviceService>(graphicsDeviceManager);
    }
}
