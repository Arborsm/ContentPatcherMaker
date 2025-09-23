using ContentPatcherMaker.Core.Services.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace ContentPatcherMaker.Core.Services;

/// <summary>
/// XNB数据加载器
/// 用于从XNB文件加载游戏数据
/// </summary>
public class XnbDataLoader
{
    private readonly LoggingService _loggingService;
    private readonly string _contentPath;
    private readonly XnbUnpackingService _unpackingService;
    private ContentManager? _contentManager;

    /// <summary>
    /// Content文件夹路径
    /// </summary>
    public string ContentPath => _contentPath;

    /// <summary>
    /// 初始化XNB数据加载器
    /// </summary>
    /// <param name="loggingService">日志服务</param>
    /// <param name="contentPath">Content文件夹路径</param>
    public XnbDataLoader(LoggingService loggingService, string contentPath)
    {
        _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        _contentPath = contentPath ?? throw new ArgumentNullException(nameof(contentPath));
        _unpackingService = new XnbUnpackingService(loggingService);
    }

    /// <summary>
    /// 初始化ContentManager
    /// </summary>
    private void InitializeContentManager()
    {
        if (_contentManager != null) return;
        using var game = new XnaGame();
        _contentManager = new ContentManager(game.Services, _contentPath);
    }

    /// <summary>
    /// 从XNB文件加载数据（同步版本）
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="relativePath">相对路径（不含.xnb扩展名）</param>
    /// <returns>加载的数据，如果失败则返回null</returns>
    public T? LoadXnbFile<T>(string relativePath)
    {
        try
        {
            InitializeContentManager();
            
            var xnbPath = Path.Combine(_contentPath, $"{relativePath}.xnb");
            if (!File.Exists(xnbPath))
            {
                _loggingService.LogWarning($"XNB文件不存在: {xnbPath}", "XnbDataLoader");
                return default;
            }

            var asset = _contentManager!.Load<T>(relativePath);
            _loggingService.LogDebug($"成功加载XNB文件: {relativePath}", "XnbDataLoader");
            return asset;
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"加载XNB文件失败: {relativePath}, 错误: {ex.Message}", ex, "XnbDataLoader");
            return default;
        }
    }

    /// <summary>
    /// 获取指定目录下的所有XNB文件路径
    /// </summary>
    /// <param name="relativeDirectory">相对目录路径</param>
    /// <returns>XNB文件路径列表（不含.xnb扩展名）</returns>
    public IEnumerable<string> GetXnbFilePaths(string relativeDirectory)
    {
        try
        {
            var fullPath = Path.Combine(_contentPath, relativeDirectory);
            if (!Directory.Exists(fullPath))
            {
                _loggingService.LogWarning($"目录不存在: {fullPath}", "XnbDataLoader");
                return [];
            }

            var xnbFiles = Directory.GetFiles(fullPath, "*.xnb", SearchOption.AllDirectories)
                .Select(path => 
                {
                    var relativePath = Path.GetRelativePath(_contentPath, path);
                    return Path.ChangeExtension(relativePath, null); // 移除.xnb扩展名
                })
                .ToList();

            _loggingService.LogDebug($"找到 {xnbFiles.Count} 个XNB文件在目录: {relativeDirectory}", "XnbDataLoader");
            return xnbFiles;
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"获取XNB文件路径失败: {relativeDirectory}, 错误: {ex.Message}", ex, "XnbDataLoader");
            return [];
        }
    }

    /// <summary>
    /// 解包XNB文件为JSON并加载
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="relativePath">相对路径（不含.xnb扩展名）</param>
    /// <param name="tempDirectory">临时解包目录</param>
    /// <returns>加载的数据，如果失败则返回null</returns>
    public async Task<T?> LoadXnbAsJsonAsync<T>(string relativePath, string tempDirectory)
    {
        try
        {
            InitializeContentManager();
            
            var xnbPath = Path.Combine(_contentPath, $"{relativePath}.xnb");
            if (!File.Exists(xnbPath))
            {
                _loggingService.LogWarning($"XNB文件不存在: {xnbPath}", "XnbDataLoader");
                return default;
            }

            // 解包XNB文件
            var tempOutputPath = Path.Combine(tempDirectory, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(tempOutputPath)!);
            
            var unpackResult = _unpackingService.UnpackXnbFile(xnbPath, tempOutputPath, _contentManager!);
            if (!unpackResult.IsSuccess)
            {
                _loggingService.LogError($"解包XNB文件失败: {unpackResult.ErrorMessage}", context: "XnbDataLoader");
                return default;
            }

            // 查找解包后的JSON文件
            var jsonPath = Directory.GetFiles(Path.GetDirectoryName(tempOutputPath)!, "*.json")
                .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == Path.GetFileNameWithoutExtension(tempOutputPath));

            if (jsonPath == null)
            {
                _loggingService.LogWarning($"解包后未找到JSON文件: {tempOutputPath}", "XnbDataLoader");
                return default;
            }

            // 加载JSON文件
            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            var result = JsonConvert.DeserializeObject<T>(jsonContent, settings);
            _loggingService.LogDebug($"成功从XNB解包并加载JSON: {relativePath}", "XnbDataLoader");
            return result;
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"从XNB解包并加载JSON失败: {relativePath}, 错误: {ex.Message}", ex, "XnbDataLoader");
            return default;
        }
    }

    /// <summary>
    /// 批量解包XNB文件为JSON并加载
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="relativePaths">相对路径列表</param>
    /// <param name="tempDirectory">临时解包目录</param>
    /// <returns>加载的数据列表</returns>
    public async Task<IEnumerable<T>> LoadXnbFilesAsJsonAsync<T>(IEnumerable<string> relativePaths, string tempDirectory)
    {
        var results = new List<T>();
        var tasks = relativePaths.Select(async path =>
        {
            var data = await LoadXnbAsJsonAsync<T>(path, tempDirectory);
            return data;
        });

        var loadedData = await Task.WhenAll(tasks);
        results.AddRange(loadedData.Where(data => data != null)!);
        
        return results;
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        _contentManager?.Dispose();
        _contentManager = null;
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
