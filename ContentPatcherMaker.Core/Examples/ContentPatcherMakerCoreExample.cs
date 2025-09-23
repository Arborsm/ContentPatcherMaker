using ContentPatcherMaker.Core.DataModels.Datas;
using ContentPatcherMaker.Core.Services.Logging;

namespace ContentPatcherMaker.Core.Examples;

/// <summary>
/// ContentPatcherMakerCore使用示例
/// </summary>
public class ContentPatcherMakerCoreExample
{
    private ContentPatcherMakerCore? _core;
    private readonly LoggingService _loggingService = new();

    /// <summary>
    /// 运行示例
    /// </summary>
    public async Task RunExampleAsync()
    {
        try
        {
            _loggingService.LogInformation("开始ContentPatcherMakerCore示例", "ContentPatcherMakerCoreExample");

            // 1. 创建和初始化核心系统
            await InitializeCoreAsync();

            // 2. 演示DataModelManager功能
            await DemonstrateDataModelManagerAsync();

            // 3. 演示数据持久化功能
            await DemonstrateDataPersistenceAsync();

            // 4. 演示文件夹监控功能
            await DemonstrateFolderWatchingAsync();

            // 5. 演示系统配置管理
            await DemonstrateSystemConfigurationAsync();

            _loggingService.LogInformation("ContentPatcherMakerCore示例完成", "ContentPatcherMakerCoreExample");
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"ContentPatcherMakerCore示例执行失败: {ex.Message}", ex, "ContentPatcherMakerCoreExample");
        }
        finally
        {
            // 清理资源
            _core?.Dispose();
        }
    }

    /// <summary>
    /// 初始化核心系统
    /// </summary>
    private async Task InitializeCoreAsync()
    {
        _loggingService.LogInformation("初始化ContentPatcherMakerCore", "ContentPatcherMakerCoreExample");

        _core = new ContentPatcherMakerCore();
        await _core.InitializeAsync();

        _loggingService.LogInformation("ContentPatcherMakerCore初始化完成", "ContentPatcherMakerCoreExample");
    }

    /// <summary>
    /// 演示DataModelManager功能
    /// </summary>
    private async Task DemonstrateDataModelManagerAsync()
    {
        if (_core == null) return;

        _loggingService.LogInformation("演示DataModelManager功能", "ContentPatcherMakerCoreExample");

        // 获取数据加载状态
        var loadingStatus = _core.DataModelManager.GetLoadingStatus();
        _loggingService.LogInformation($"数据加载状态: {loadingStatus}", "ContentPatcherMakerCoreExample");

        // 检查数据是否已加载
        var isDataLoaded = _core.DataModelManager.IsDataLoaded();
        _loggingService.LogInformation($"数据是否已加载: {isDataLoaded}", "ContentPatcherMakerCoreExample");

        // 演示获取各种数据集合
        try
        {
            var achievements = _core.DataModelManager.GetCollection<AchievementDataCollection>();
            _loggingService.LogInformation($"获取到成就数据集合，包含 {achievements.GetAll().Count()} 个成就", "ContentPatcherMakerCoreExample");

            var farms = _core.DataModelManager.GetCollection<FarmDataCollection>();
            _loggingService.LogInformation($"获取到农场数据集合，包含 {farms.GetAll().Count()} 个农场", "ContentPatcherMakerCoreExample");

            var characters = _core.DataModelManager.GetCollection<CharacterDataCollection>();
            _loggingService.LogInformation($"获取到角色数据集合，包含 {characters.GetAll().Count()} 个角色", "ContentPatcherMakerCoreExample");

            var events = _core.DataModelManager.GetCollection<EventDataCollection>();
            _loggingService.LogInformation($"获取到事件数据集合，包含 {events.GetAll().Count()} 个事件", "ContentPatcherMakerCoreExample");

            var festivals = _core.DataModelManager.GetCollection<FestivalDataCollection>();
            _loggingService.LogInformation($"获取到节日数据集合，包含 {festivals.GetAll().Count()} 个节日", "ContentPatcherMakerCoreExample");

            var languages = _core.DataModelManager.GetCollection<LanguageDataCollection>();
            _loggingService.LogInformation($"获取到语言数据集合，包含 {languages.GetAll().Count()} 个语言", "ContentPatcherMakerCoreExample");
        }
        catch (Exception ex)
        {
            _loggingService.LogWarning($"获取数据集合时发生错误: {ex.Message}", "ContentPatcherMakerCoreExample");
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// 演示数据持久化功能
    /// </summary>
    private async Task DemonstrateDataPersistenceAsync()
    {
        if (_core == null) return;

        _loggingService.LogInformation("演示数据持久化功能", "ContentPatcherMakerCoreExample");

        // 创建测试数据
        var testData = new TestData
        {
            Id = Guid.NewGuid().ToString(),
            Name = "测试数据",
            Value = 42,
            CreatedAt = DateTime.UtcNow,
            Tags = ["测试", "示例", "数据"]
        };

        // 保存数据
        var dataPath = Path.Combine(Path.GetTempPath(), "test-data.json");
        await _core.PersistenceService.SaveAsync(testData, dataPath);
        _loggingService.LogInformation($"测试数据已保存到: {dataPath}", "ContentPatcherMakerCoreExample");

        // 加载数据
        var loadedData = await _core.PersistenceService.LoadAsync<TestData>(dataPath);
        if (loadedData != null)
        {
            _loggingService.LogInformation($"测试数据已加载: {loadedData.Name} (值: {loadedData.Value})", "ContentPatcherMakerCoreExample");
        }

        // 备份数据
        var backupPath = Path.Combine(Path.GetTempPath(), "test-data-backup.json");
        await _core.PersistenceService.BackupAsync(dataPath, backupPath);
        _loggingService.LogInformation($"测试数据已备份到: {backupPath}", "ContentPatcherMakerCoreExample");

        // 清理测试文件
        await _core.PersistenceService.DeleteAsync(dataPath);
        await _core.PersistenceService.DeleteAsync(backupPath);
        _loggingService.LogInformation("测试文件已清理", "ContentPatcherMakerCoreExample");
    }

    /// <summary>
    /// 演示文件夹监控功能
    /// </summary>
    private async Task DemonstrateFolderWatchingAsync()
    {
        if (_core == null) return;

        _loggingService.LogInformation("演示文件夹监控功能", "ContentPatcherMakerCoreExample");

        // 创建测试文件夹
        var testFolder = Path.Combine(Path.GetTempPath(), "ContentPatcherMakerTest");
        Directory.CreateDirectory(testFolder);

        try
        {
            // 添加数据路径（会自动开始监控）
            _core.AddDataPath(testFolder);
            _loggingService.LogInformation($"已添加测试文件夹并开始监控: {testFolder}", "ContentPatcherMakerCoreExample");

            // 获取监控状态
            var watchedFolders = _core.FolderWatcherService.GetWatchedFolders();
            _loggingService.LogInformation($"当前监控的文件夹: {string.Join(", ", watchedFolders)}", "ContentPatcherMakerCoreExample");

            // 创建测试数据文件
            var testDataFile = Path.Combine(testFolder, "test-data.json");
            var testDataContent = """
            {
              "id": "test-data",
              "name": "测试数据",
              "value": 42,
              "createdAt": "2024-01-01T00:00:00Z"
            }
            """;
            await File.WriteAllTextAsync(testDataFile, testDataContent);
            _loggingService.LogInformation("已创建测试数据文件", "ContentPatcherMakerCoreExample");

            // 等待一段时间让文件系统事件触发
            await Task.Delay(1000);

            // 移除测试文件夹监控
            _core.RemoveDataPath(testFolder);
            _loggingService.LogInformation("已移除测试文件夹监控", "ContentPatcherMakerCoreExample");
        }
        finally
        {
            // 清理测试文件夹
            if (Directory.Exists(testFolder))
            {
                Directory.Delete(testFolder, true);
                _loggingService.LogInformation("测试文件夹已清理", "ContentPatcherMakerCoreExample");
            }
        }
    }

    /// <summary>
    /// 演示系统配置管理
    /// </summary>
    private async Task DemonstrateSystemConfigurationAsync()
    {
        if (_core == null) return;

        _loggingService.LogInformation("演示系统配置管理", "ContentPatcherMakerCoreExample");

        // 获取系统统计信息
        var systemStats = _core.GetSystemStatistics();
        _loggingService.LogInformation($"系统统计信息: {systemStats}", "ContentPatcherMakerCoreExample");

        // 保存系统配置
        var configPath = Path.Combine(Path.GetTempPath(), "system-config.json");
        await _core.SaveSystemConfigurationAsync(configPath);
        _loggingService.LogInformation($"系统配置已保存到: {configPath}", "ContentPatcherMakerCoreExample");

        // 加载系统配置
        await _core.LoadSystemConfigurationAsync(configPath);
        _loggingService.LogInformation("系统配置已加载", "ContentPatcherMakerCoreExample");

        // 清理配置文件
        if (File.Exists(configPath))
        {
            File.Delete(configPath);
            _loggingService.LogInformation("配置文件已清理", "ContentPatcherMakerCoreExample");
        }
    }
}

/// <summary>
/// 测试数据类
/// </summary>
public class TestData
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Tags { get; set; } = new();
}
