using ContentPatcherMaker.Core.DataModels;
using ContentPatcherMaker.Core.Services.Logging;

namespace ContentPatcherMaker.Core.Services;

/// <summary>
/// DataModelManager工厂服务
/// 负责创建和配置DataModelManager实例
/// </summary>
public class DataModelManagerFactory
{
    private readonly LoggingService _loggingService;

    /// <summary>
    /// 初始化DataModelManager工厂
    /// </summary>
    /// <param name="loggingService">日志服务</param>
    public DataModelManagerFactory(LoggingService loggingService)
    {
        _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
    }

    /// <summary>
    /// 从游戏目录创建DataModelManager（智能模式）
    /// </summary>
    /// <param name="gamePath">游戏路径</param>
    /// <param name="contentPath">内容路径</param>
    /// <returns>配置好的DataModelManager实例</returns>
    public DataModelManager CreateFromGameDirectory(string gamePath, string contentPath)
    {
        if (string.IsNullOrEmpty(gamePath))
            throw new ArgumentException("游戏路径不能为空", nameof(gamePath));
        if (string.IsNullOrEmpty(contentPath))
            throw new ArgumentException("内容路径不能为空", nameof(contentPath));

        if (!Directory.Exists(gamePath))
            throw new DirectoryNotFoundException($"游戏路径不存在: {gamePath}");
        if (!Directory.Exists(contentPath))
            throw new DirectoryNotFoundException($"内容路径不存在: {contentPath}");

        _loggingService.LogInformation($"从游戏目录创建DataModelManager (智能模式): {gamePath}", "DataModelManagerFactory");

        // 创建DataModelManager（智能模式）
        var dataModelManager = new DataModelManager(contentPath, _loggingService);

        _loggingService.LogInformation("DataModelManager创建成功 (智能模式)", "DataModelManagerFactory");
        return dataModelManager;
    }

    /// <summary>
    /// 从游戏目录创建DataModelManager（JSON模式）- 内部使用
    /// </summary>
    /// <param name="gamePath">游戏路径</param>
    /// <param name="contentPath">内容路径</param>
    /// <returns>配置好的DataModelManager实例</returns>
    public DataModelManager CreateFromGameDirectoryJson(string gamePath, string contentPath)
    {
        if (string.IsNullOrEmpty(gamePath))
            throw new ArgumentException("游戏路径不能为空", nameof(gamePath));
        if (string.IsNullOrEmpty(contentPath))
            throw new ArgumentException("内容路径不能为空", nameof(contentPath));

        if (!Directory.Exists(gamePath))
            throw new DirectoryNotFoundException($"游戏路径不存在: {gamePath}");
        if (!Directory.Exists(contentPath))
            throw new DirectoryNotFoundException($"内容路径不存在: {contentPath}");

        _loggingService.LogInformation($"从游戏目录创建DataModelManager (JSON模式): {gamePath}", "DataModelManagerFactory");

        // 创建JSON数据加载器
        var jsonLoader = new JsonDataLoader(_loggingService, contentPath);

        // 创建DataModelManager
        var dataModelManager = new DataModelManager(jsonLoader, _loggingService);

        _loggingService.LogInformation("DataModelManager创建成功 (JSON模式)", "DataModelManagerFactory");
        return dataModelManager;
    }

    /// <summary>
    /// 从游戏目录创建DataModelManager（XNB模式）- 内部使用
    /// </summary>
    /// <param name="gamePath">游戏路径</param>
    /// <param name="contentPath">内容路径</param>
    /// <returns>配置好的DataModelManager实例</returns>
    public DataModelManager CreateFromGameDirectoryXnb(string gamePath, string contentPath)
    {
        if (string.IsNullOrEmpty(gamePath))
            throw new ArgumentException("游戏路径不能为空", nameof(gamePath));
        if (string.IsNullOrEmpty(contentPath))
            throw new ArgumentException("内容路径不能为空", nameof(contentPath));

        if (!Directory.Exists(gamePath))
            throw new DirectoryNotFoundException($"游戏路径不存在: {gamePath}");
        if (!Directory.Exists(contentPath))
            throw new DirectoryNotFoundException($"内容路径不存在: {contentPath}");

        _loggingService.LogInformation($"从游戏目录创建DataModelManager (XNB模式): {gamePath}", "DataModelManagerFactory");

        // 创建XNB数据加载器
        var xnbLoader = new XnbDataLoader(_loggingService, contentPath);

        // 创建DataModelManager
        var dataModelManager = new DataModelManager(xnbLoader, _loggingService);

        _loggingService.LogInformation("DataModelManager创建成功 (XNB模式)", "DataModelManagerFactory");
        return dataModelManager;
    }

    /// <summary>
    /// 从游戏路径检测结果创建DataModelManager（JSON模式）- 内部使用
    /// </summary>
    /// <param name="detectionResult">游戏路径检测结果</param>
    /// <returns>配置好的DataModelManager实例</returns>
    public DataModelManager CreateFromDetectionResult(GamePathDetectionResult detectionResult)
    {
        if (detectionResult == null)
            throw new ArgumentNullException(nameof(detectionResult));

        if (!detectionResult.IsSuccess)
            throw new InvalidOperationException($"游戏路径检测失败: {detectionResult.ErrorMessage}");

        if (!detectionResult.ValidatePaths())
            throw new InvalidOperationException("检测到的路径无效");

        return CreateFromGameDirectoryJson(detectionResult.GamePath!, detectionResult.ContentPath!);
    }

    /// <summary>
    /// 从游戏路径检测结果创建DataModelManager（XNB模式）- 内部使用
    /// </summary>
    /// <param name="detectionResult">游戏路径检测结果</param>
    /// <returns>配置好的DataModelManager实例</returns>
    public DataModelManager CreateFromDetectionResultXnb(GamePathDetectionResult detectionResult)
    {
        if (detectionResult == null)
            throw new ArgumentNullException(nameof(detectionResult));

        if (!detectionResult.IsSuccess)
            throw new InvalidOperationException($"游戏路径检测失败: {detectionResult.ErrorMessage}");

        if (!detectionResult.ValidatePaths())
            throw new InvalidOperationException("检测到的路径无效");

        return CreateFromGameDirectoryXnb(detectionResult.GamePath!, detectionResult.ContentPath!);
    }

    /// <summary>
    /// 自动检测并创建DataModelManager（智能模式）
    /// </summary>
    /// <param name="specifiedGamePath">指定的游戏路径，如果为null则自动检测</param>
    /// <returns>DataModelManager创建结果</returns>
    public async Task<DataModelManagerCreationResult> CreateWithAutoDetectionAsync(string? specifiedGamePath = null)
    {
        try
        {
            _loggingService.LogInformation("开始自动检测并创建DataModelManager (智能模式)", "DataModelManagerFactory");

            // 检测游戏路径
            var pathDetectionService = new GamePathDetectionService(_loggingService);
            var detectionResult = pathDetectionService.DetectGamePaths(specifiedGamePath);

            if (!detectionResult.IsSuccess)
            {
                return new DataModelManagerCreationResult
                {
                    IsSuccess = false,
                    ErrorMessage = detectionResult.ErrorMessage,
                    DataModelManager = null
                };
            }

            // 创建DataModelManager（智能模式）
            var dataModelManager = CreateFromGameDirectory(detectionResult.GamePath!, detectionResult.ContentPath!);

            // 加载所有数据
            await dataModelManager.LoadAllDataAsync();

            _loggingService.LogInformation("DataModelManager自动创建并加载完成 (智能模式)", "DataModelManagerFactory");

            return new DataModelManagerCreationResult
            {
                IsSuccess = true,
                ErrorMessage = null,
                DataModelManager = dataModelManager,
                GamePath = detectionResult.GamePath,
                ContentPath = detectionResult.ContentPath,
                Platform = detectionResult.Platform
            };
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"自动创建DataModelManager时发生异常: {ex.Message}", ex, "DataModelManagerFactory");
            return new DataModelManagerCreationResult
            {
                IsSuccess = false,
                ErrorMessage = $"创建DataModelManager时发生异常: {ex.Message}",
                DataModelManager = null
            };
        }
    }

    /// <summary>
    /// 自动检测并创建DataModelManager（JSON模式）- 内部使用
    /// </summary>
    /// <param name="specifiedGamePath">指定的游戏路径，如果为null则自动检测</param>
    /// <returns>DataModelManager创建结果</returns>
    public async Task<DataModelManagerCreationResult> CreateWithAutoDetectionJsonAsync(string? specifiedGamePath = null)
    {
        try
        {
            _loggingService.LogInformation("开始自动检测并创建DataModelManager (JSON模式)", "DataModelManagerFactory");

            // 检测游戏路径
            var pathDetectionService = new GamePathDetectionService(_loggingService);
            var detectionResult = pathDetectionService.DetectGamePaths(specifiedGamePath);

            if (!detectionResult.IsSuccess)
            {
                return new DataModelManagerCreationResult
                {
                    IsSuccess = false,
                    ErrorMessage = detectionResult.ErrorMessage,
                    DataModelManager = null
                };
            }

            // 创建DataModelManager
            var dataModelManager = CreateFromDetectionResult(detectionResult);

            // 加载所有数据
            await dataModelManager.LoadAllDataAsync();

            _loggingService.LogInformation("DataModelManager自动创建并加载完成 (JSON模式)", "DataModelManagerFactory");

            return new DataModelManagerCreationResult
            {
                IsSuccess = true,
                ErrorMessage = null,
                DataModelManager = dataModelManager,
                GamePath = detectionResult.GamePath,
                ContentPath = detectionResult.ContentPath,
                Platform = detectionResult.Platform
            };
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"自动创建DataModelManager时发生异常: {ex.Message}", ex, "DataModelManagerFactory");
            return new DataModelManagerCreationResult
            {
                IsSuccess = false,
                ErrorMessage = $"创建DataModelManager时发生异常: {ex.Message}",
                DataModelManager = null
            };
        }
    }

    /// <summary>
    /// 自动检测并创建DataModelManager（XNB模式）- 内部使用
    /// </summary>
    /// <param name="specifiedGamePath">指定的游戏路径，如果为null则自动检测</param>
    /// <returns>DataModelManager创建结果</returns>
    public async Task<DataModelManagerCreationResult> CreateWithAutoDetectionXnbAsync(string? specifiedGamePath = null)
    {
        try
        {
            _loggingService.LogInformation("开始自动检测并创建DataModelManager (XNB模式)", "DataModelManagerFactory");

            // 检测游戏路径
            var pathDetectionService = new GamePathDetectionService(_loggingService);
            var detectionResult = pathDetectionService.DetectGamePaths(specifiedGamePath);

            if (!detectionResult.IsSuccess)
            {
                return new DataModelManagerCreationResult
                {
                    IsSuccess = false,
                    ErrorMessage = detectionResult.ErrorMessage,
                    DataModelManager = null
                };
            }

            // 创建DataModelManager
            var dataModelManager = CreateFromDetectionResultXnb(detectionResult);

            // 加载所有数据
            await dataModelManager.LoadAllDataAsync();

            _loggingService.LogInformation("DataModelManager自动创建并加载完成 (XNB模式)", "DataModelManagerFactory");

            return new DataModelManagerCreationResult
            {
                IsSuccess = true,
                ErrorMessage = null,
                DataModelManager = dataModelManager,
                GamePath = detectionResult.GamePath,
                ContentPath = detectionResult.ContentPath,
                Platform = detectionResult.Platform
            };
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"自动创建DataModelManager时发生异常: {ex.Message}", ex, "DataModelManagerFactory");
            return new DataModelManagerCreationResult
            {
                IsSuccess = false,
                ErrorMessage = $"创建DataModelManager时发生异常: {ex.Message}",
                DataModelManager = null
            };
        }
    }

    /// <summary>
    /// 创建DataModelManager并加载指定类型的数据
    /// </summary>
    /// <param name="gamePath">游戏路径</param>
    /// <param name="contentPath">内容路径</param>
    /// <param name="dataTypes">要加载的数据类型</param>
    /// <returns>配置好的DataModelManager实例</returns>
    public async Task<DataModelManager> CreateWithSelectiveLoadingAsync(
        string gamePath, 
        string contentPath, 
        params DataType[] dataTypes)
    {
        var dataModelManager = CreateFromGameDirectory(gamePath, contentPath);

        _loggingService.LogInformation($"开始加载指定的数据类型: {string.Join(", ", dataTypes)}", "DataModelManagerFactory");

        // 根据指定的数据类型加载数据
        var tasks = new List<Task>();

        if (dataTypes.Contains(DataType.Achievements))
            tasks.Add(dataModelManager.LoadAchievementsAsync());

        if (dataTypes.Contains(DataType.Farms))
            tasks.Add(dataModelManager.LoadFarmsAsync());

        if (dataTypes.Contains(DataType.Characters))
            tasks.Add(dataModelManager.LoadCharactersAsync());

        if (dataTypes.Contains(DataType.Events))
            tasks.Add(dataModelManager.LoadEventsAsync());

        if (dataTypes.Contains(DataType.Festivals))
            tasks.Add(dataModelManager.LoadFestivalsAsync());

        if (dataTypes.Contains(DataType.Languages))
            tasks.Add(dataModelManager.LoadLanguagesAsync());

        if (tasks.Count > 0)
        {
            await Task.WhenAll(tasks);
        }

        _loggingService.LogInformation("指定数据类型加载完成", "DataModelManagerFactory");
        return dataModelManager;
    }
}

/// <summary>
/// DataModelManager创建结果
/// </summary>
public class DataModelManagerCreationResult
{
    /// <summary>
    /// 是否创建成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 创建的DataModelManager实例
    /// </summary>
    public DataModelManager? DataModelManager { get; set; }

    /// <summary>
    /// 检测到的游戏路径
    /// </summary>
    public string? GamePath { get; set; }

    /// <summary>
    /// 检测到的内容路径
    /// </summary>
    public string? ContentPath { get; set; }

    /// <summary>
    /// 检测到的平台
    /// </summary>
    public Platform? Platform { get; set; }
}

/// <summary>
/// 数据类型枚举
/// </summary>
public enum DataType
{
    /// <summary>
    /// 成就数据
    /// </summary>
    Achievements,

    /// <summary>
    /// 农场数据
    /// </summary>
    Farms,

    /// <summary>
    /// 角色数据
    /// </summary>
    Characters,

    /// <summary>
    /// 事件数据
    /// </summary>
    Events,

    /// <summary>
    /// 节日数据
    /// </summary>
    Festivals,

    /// <summary>
    /// 语言数据
    /// </summary>
    Languages
}
