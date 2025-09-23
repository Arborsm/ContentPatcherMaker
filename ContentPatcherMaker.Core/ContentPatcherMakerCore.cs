using System.Reflection;
using ContentPatcherMaker.Core.DataModels;
using ContentPatcherMaker.Core.DataModels.Datas;
using ContentPatcherMaker.Core.Services;
using ContentPatcherMaker.Core.Services.Logging;
using ContentPatcherMaker.Core.Services.Persistence;

namespace ContentPatcherMaker.Core;

/// <summary>
/// ContentPatcherMaker核心类
/// 负责初始化和管理整个应用程序的核心功能
/// </summary>
public class ContentPatcherMakerCore
{
    private readonly LoggingService _loggingService;
    private readonly Dictionary<Type, DataModelCollectionBase> _dataModelCollections = new();
    private bool _isInitialized;

    /// <summary>
    /// 数据持久化服务
    /// </summary>
    public IDataPersistenceService PersistenceService { get; }

    /// <summary>
    /// 文件夹监控服务
    /// </summary>
    public FolderWatcherService FolderWatcherService { get; }

    /// <summary>
    /// 数据模型管理器
    /// </summary>
    public DataModelManager DataModelManager { get; private set; }

    /// <summary>
    /// 初始化ContentPatcherMakerCore
    /// </summary>
    public ContentPatcherMakerCore()
    {
        _loggingService = new LoggingService();
        PersistenceService = new JsonDataPersistenceService(_loggingService);
        FolderWatcherService = new FolderWatcherService(_loggingService);
        
        // _dataModelManager将在InitializeDataModelManagerAsync中通过DataModelManagerFactory创建
        DataModelManager = null!;
    }

    /// <summary>
    /// 初始化核心系统
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            _loggingService.LogWarning("ContentPatcherMakerCore已经初始化", "ContentPatcherMakerCore");
            return;
        }

        try
        {
            _loggingService.LogInformation("开始初始化ContentPatcherMakerCore", "ContentPatcherMakerCore");
            
            // 1. 初始化程序集解析器
            Init();

            // 2. 初始化DataModelManager
            await InitializeDataModelManagerAsync();

            // 3. 初始化文件夹监控服务
            await InitializeFolderWatcherServiceAsync();

            _isInitialized = true;
            _loggingService.LogInformation("ContentPatcherMakerCore初始化完成", "ContentPatcherMakerCore");
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"ContentPatcherMakerCore初始化失败: {ex.Message}", ex, "ContentPatcherMakerCore");
            throw;
        }
    }

    /// <summary>
    /// 初始化程序集解析器
    /// </summary>
    public void Init()
    {
        AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver.CurrentDomain_AssemblyResolve;
        _loggingService.LogInformation("程序集解析器已初始化", "ContentPatcherMakerCore");
    }

    /// <summary>
    /// 初始化DataModelManager
    /// </summary>
    private async Task InitializeDataModelManagerAsync()
    {
        _loggingService.LogInformation("初始化DataModelManager", "ContentPatcherMakerCore");

        try
        {
            // 使用DataModelManagerFactory自动检测并创建DataModelManager
            var factory = new DataModelManagerFactory(_loggingService);
            var result = await factory.CreateWithAutoDetectionAsync();

            if (!result.IsSuccess || result.DataModelManager == null)
            {
                throw new InvalidOperationException($"DataModelManager创建失败: {result.ErrorMessage}");
            }

            // 替换当前的DataModelManager实例
            DataModelManager = result.DataModelManager;

            // 将加载的数据注册到内部字典中
            RegisterDataModelCollections();

            _loggingService.LogInformation($"DataModelManager初始化完成 - 游戏路径: {result.GamePath}, 内容路径: {result.ContentPath}", "ContentPatcherMakerCore");
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"DataModelManager初始化失败: {ex.Message}", ex, "ContentPatcherMakerCore");
            throw;
        }
    }

    /// <summary>
    /// 注册DataModelCollection到内部字典
    /// </summary>
    private void RegisterDataModelCollections()
    {
        try
        {
            // 注册各种数据集合
            if (DataModelManager.GetCollection<AchievementDataCollection>() is { } achievements)
            {
                _dataModelCollections[typeof(AchievementDataCollection)] = achievements;
            }

            if (DataModelManager.GetCollection<FarmDataCollection>() is { } farms)
            {
                _dataModelCollections[typeof(FarmDataCollection)] = farms;
            }

            if (DataModelManager.GetCollection<CharacterDataCollection>() is { } characters)
            {
                _dataModelCollections[typeof(CharacterDataCollection)] = characters;
            }

            if (DataModelManager.GetCollection<EventDataCollection>() is { } events)
            {
                _dataModelCollections[typeof(EventDataCollection)] = events;
            }

            if (DataModelManager.GetCollection<FestivalDataCollection>() is { } festivals)
            {
                _dataModelCollections[typeof(FestivalDataCollection)] = festivals;
            }

            if (DataModelManager.GetCollection<LanguageDataCollection>() is { } languages)
            {
                _dataModelCollections[typeof(LanguageDataCollection)] = languages;
            }

            _loggingService.LogInformation($"已注册 {_dataModelCollections.Count} 个DataModelCollection", "ContentPatcherMakerCore");
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"注册DataModelCollection失败: {ex.Message}", ex, "ContentPatcherMakerCore");
        }
    }
    

    /// <summary>
    /// 初始化文件夹监控服务
    /// </summary>
    private async Task InitializeFolderWatcherServiceAsync()
    {
        _loggingService.LogInformation("初始化文件夹监控服务", "ContentPatcherMakerCore");

        // 开始监控数据文件夹
        var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        if (Directory.Exists(dataPath))
        {
            FolderWatcherService.StartWatching(dataPath, true, "*.json", OnDataFileChanged);
        }

        _loggingService.LogInformation("文件夹监控服务初始化完成", "ContentPatcherMakerCore");
        await Task.CompletedTask;
    }

    /// <summary>
    /// 数据文件变化处理
    /// </summary>
    private void OnDataFileChanged(string filePath)
    {
        try
        {
            _loggingService.LogInformation($"数据文件变化: {filePath}", "ContentPatcherMakerCore");
            
            // 根据文件名重新加载对应的DataModelCollection
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            switch (fileName.ToLower())
            {
                case "achievements":
                    ReloadDataModelCollection<AchievementDataCollection>(filePath);
                    break;
                case "farms":
                    ReloadDataModelCollection<FarmDataCollection>(filePath);
                    break;
                case "characters":
                    ReloadDataModelCollection<CharacterDataCollection>(filePath);
                    break;
                case "events":
                    ReloadDataModelCollection<EventDataCollection>(filePath);
                    break;
                case "festivals":
                    ReloadDataModelCollection<FestivalDataCollection>(filePath);
                    break;
                case "languages":
                    ReloadDataModelCollection<LanguageDataCollection>(filePath);
                    break;
            }
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"处理数据文件变化失败: {filePath}, 错误: {ex.Message}", ex, "ContentPatcherMakerCore");
        }
    }

    /// <summary>
    /// 重新加载DataModelCollection
    /// </summary>
    private async void ReloadDataModelCollection<T>(string filePath) where T : DataModelCollectionBase
    {
        try
        {
            var collection = await PersistenceService.LoadAsync<T>(filePath);
            if (collection != null)
            {
                _dataModelCollections[typeof(T)] = collection;
                _loggingService.LogInformation($"已重新加载DataModelCollection: {typeof(T).Name}", "ContentPatcherMakerCore");
            }
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"重新加载DataModelCollection失败: {typeof(T).Name}, 错误: {ex.Message}", ex, "ContentPatcherMakerCore");
        }
    }

    /// <summary>
    /// 添加数据路径
    /// </summary>
    /// <param name="path">数据路径</param>
    public void AddDataPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            _loggingService.LogWarning("数据路径不能为空", "ContentPatcherMakerCore");
            return;
        }

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            _loggingService.LogInformation($"创建数据目录: {path}", "ContentPatcherMakerCore");
        }

        // 开始监控新路径
        FolderWatcherService.StartWatching(path, true, "*.json", OnDataFileChanged);
        _loggingService.LogInformation($"已添加数据路径: {path}", "ContentPatcherMakerCore");
    }

    /// <summary>
    /// 移除数据路径
    /// </summary>
    /// <param name="path">数据路径</param>
    public void RemoveDataPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            _loggingService.LogWarning("数据路径不能为空", "ContentPatcherMakerCore");
            return;
        }

        // 停止监控路径
        FolderWatcherService.StopWatching(path);
        _loggingService.LogInformation($"已移除数据路径: {path}", "ContentPatcherMakerCore");
    }

    /// <summary>
    /// 获取DataModelCollection
    /// </summary>
    /// <typeparam name="T">DataModelCollection类型</typeparam>
    /// <returns>数据集合</returns>
    public T? GetDataModelCollection<T>() where T : DataModelCollectionBase
    {
        var type = typeof(T);
        if (_dataModelCollections.TryGetValue(type, out var collection))
        {
            return collection as T;
        }
        return null;
    }

    /// <summary>
    /// 获取所有已注册的DataModelCollection类型
    /// </summary>
    /// <returns>类型列表</returns>
    public IEnumerable<Type> GetRegisteredDataModelCollectionTypes()
    {
        return _dataModelCollections.Keys.ToList();
    }

    /// <summary>
    /// 检查DataModelCollection是否已注册
    /// </summary>
    /// <typeparam name="T">DataModelCollection类型</typeparam>
    /// <returns>是否已注册</returns>
    public bool IsDataModelCollectionRegistered<T>() where T : DataModelCollectionBase
    {
        var type = typeof(T);
        return _dataModelCollections.ContainsKey(type);
    }

    /// <summary>
    /// 获取系统统计信息
    /// </summary>
    /// <returns>系统统计信息</returns>
    public SystemStatistics GetSystemStatistics()
    {
        var folderWatcherStats = FolderWatcherService.GetStatistics();
        var dataLoadingStatus = DataModelManager.GetLoadingStatus();

        return new SystemStatistics
        {
            IsInitialized = _isInitialized,
            DataModelCollectionCount = _dataModelCollections.Count,
            DataModelCollectionTypes = _dataModelCollections.Keys.Select(t => t.Name).ToList(),
            DataLoadingStatus = dataLoadingStatus,
            WatchedFoldersCount = folderWatcherStats.WatchedFoldersCount,
            LoggingServiceEnabled = true,
            PersistenceServiceEnabled = true
        };
    }

    /// <summary>
    /// 保存系统配置
    /// </summary>
    /// <param name="configPath">配置文件路径</param>
    public async Task SaveSystemConfigurationAsync(string configPath)
    {
        try
        {
            var config = new SystemConfiguration
            {
                DataPaths = FolderWatcherService.GetWatchedFolders().ToList(),
                AutoReloadData = true,
                LogLevel = "Information",
                LastUpdated = DateTime.UtcNow
            };

            await PersistenceService.SaveAsync(config, configPath);
            _loggingService.LogInformation($"系统配置已保存: {configPath}", "ContentPatcherMakerCore");
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"保存系统配置失败: {ex.Message}", ex, "ContentPatcherMakerCore");
            throw;
        }
    }

    /// <summary>
    /// 加载系统配置
    /// </summary>
    /// <param name="configPath">配置文件路径</param>
    public async Task LoadSystemConfigurationAsync(string configPath)
    {
        try
        {
            var config = await PersistenceService.LoadAsync<SystemConfiguration>(configPath);
            if (config != null)
            {
                // 重新设置数据路径
                foreach (var path in config.DataPaths)
                {
                    AddDataPath(path);
                }

                _loggingService.LogInformation($"系统配置已加载: {configPath}", "ContentPatcherMakerCore");
            }
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"加载系统配置失败: {ex.Message}", ex, "ContentPatcherMakerCore");
            throw;
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        if (!_isInitialized)
            return;

        try
        {
            _loggingService.LogInformation("开始释放ContentPatcherMakerCore资源", "ContentPatcherMakerCore");

            // 停止文件夹监控
            FolderWatcherService.StopAllWatching();

            // 释放文件夹监控服务
            FolderWatcherService.Dispose();

            _isInitialized = false;
            _loggingService.LogInformation("ContentPatcherMakerCore资源释放完成", "ContentPatcherMakerCore");
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"释放ContentPatcherMakerCore资源时发生错误: {ex.Message}", ex, "ContentPatcherMakerCore");
        }
    }
}

internal static class AssemblyResolver
{
     private static readonly string[] RelativeAssemblyProbePaths =
    [
        "", // app directory
        "smapi-public"
    ];
    private static readonly HashSet<string> ResolvingAssemblies = [];
    
    /// <summary>Method called when assembly resolution fails, which may return a manually resolved assembly.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public static Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs e)
    {
        if (ResolvingAssemblies.Contains(e.Name))
        {
            return null;
        }

        // get assembly name
        var name = new AssemblyName(e.Name);
        if (name.Name == null) return null;
        
        ResolvingAssemblies.Add(e.Name);

        try
        {
            // check search folders
            foreach (string relativePath in RelativeAssemblyProbePaths)
            {
                // get absolute path of search folder
                var path = GameHelper.GamePath;
                if (string.IsNullOrEmpty(path))
                    path = Path.GetDirectoryName(Environment.ProcessPath) ?? string.Empty;
                string searchPath = Path.Combine(path, relativePath);
                if (!Directory.Exists(searchPath)) continue;

                if (GetAssembly(searchPath, name, out var currentDomainAssemblyResolve)) 
                {
                    return currentDomainAssemblyResolve;
                }
            }

            return null;
        }
        finally
        {
            ResolvingAssemblies.Remove(e.Name);
        }
    }

    private static bool GetAssembly(string searchPath, AssemblyName name, out Assembly? currentDomainAssemblyResolve)
    {
        currentDomainAssemblyResolve = null;
        
        // try to resolve DLL
        try
        {
            foreach (var dll in new DirectoryInfo(searchPath).EnumerateFiles("*.dll"))
            {
                string? dllAssemblyName = GetAssemblyNameFromFile(dll.FullName);
                if (dllAssemblyName == null) continue;

                // check for match
                if (name.Name == null ||
                    !name.Name.Equals(dllAssemblyName, StringComparison.OrdinalIgnoreCase)) continue;
                
                try
                {
                    currentDomainAssemblyResolve = Assembly.LoadFrom(dll.FullName);
                    return true;
                }
                catch (Exception loadEx)
                {
                    Console.WriteLine($"加载程序集失败 {dll.FullName}: {loadEx.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error resolving assembly: {ex}");
            return false;
        }

        return false;
    }
    
    private static string? GetAssemblyNameFromFile(string filePath)
    {
        try
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            
            if (!string.IsNullOrEmpty(fileName) && fileName.Length > 0)
            {
                return fileName;
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }
}

/// <summary>
/// 系统配置
/// </summary>
public class SystemConfiguration
{
    /// <summary>
    /// 数据路径列表
    /// </summary>
    public List<string> DataPaths { get; set; } = new();

    /// <summary>
    /// 是否自动重新加载数据
    /// </summary>
    public bool AutoReloadData { get; set; } = true;

    /// <summary>
    /// 日志级别
    /// </summary>
    public string LogLevel { get; set; } = "Information";

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 是否启用文件夹监控
    /// </summary>
    public bool EnableFolderWatching { get; set; } = true;

    /// <summary>
    /// 是否启用数据持久化
    /// </summary>
    public bool EnablePersistence { get; set; } = true;
}

/// <summary>
/// 系统统计信息
/// </summary>
public class SystemStatistics
{
    /// <summary>
    /// 是否已初始化
    /// </summary>
    public bool IsInitialized { get; set; }

    /// <summary>
    /// DataModelCollection数量
    /// </summary>
    public int DataModelCollectionCount { get; set; }

    /// <summary>
    /// DataModelCollection类型列表
    /// </summary>
    public List<string> DataModelCollectionTypes { get; set; } = new();

    /// <summary>
    /// 数据加载状态
    /// </summary>
    public DataLoadingStatus? DataLoadingStatus { get; set; }

    /// <summary>
    /// 监控的文件夹数量
    /// </summary>
    public int WatchedFoldersCount { get; set; }

    /// <summary>
    /// 日志服务是否启用
    /// </summary>
    public bool LoggingServiceEnabled { get; set; }

    /// <summary>
    /// 持久化服务是否启用
    /// </summary>
    public bool PersistenceServiceEnabled { get; set; }

    /// <summary>
    /// 系统运行时间
    /// </summary>
    public TimeSpan Uptime { get; set; }

    /// <summary>
    /// 返回统计信息的字符串表示
    /// </summary>
    /// <returns>统计信息字符串</returns>
    public override string ToString()
    {
        var dataStatus = DataLoadingStatus?.ToString() ?? "未知";
        return $"系统统计: 已初始化={IsInitialized}, DataModelCollection={DataModelCollectionCount}, " +
               $"数据加载状态={dataStatus}, 监控文件夹={WatchedFoldersCount}, 运行时间={Uptime}";
    }
}