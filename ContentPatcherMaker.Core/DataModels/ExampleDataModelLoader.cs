using ContentPatcherMaker.Core.Services.Logging;
using ContentPatcherMaker.Core.Extensions;

namespace ContentPatcherMaker.Core.DataModels;

/// <summary>
/// 示例数据模型加载器
/// 展示如何创建自定义数据模型加载器
/// </summary>
public class ExampleDataModelLoader : IDataModelLoader<AchievementData>
{
    private readonly JsonDataLoader _jsonLoader;
    private readonly LoggingService _loggingService;
    private AchievementDataCollection? _cachedData;

    /// <summary>
    /// 模型名称
    /// </summary>
    public string ModelName => "Achievements";

    /// <summary>
    /// 初始化示例数据模型加载器
    /// </summary>
    /// <param name="jsonLoader">JSON加载器</param>
    /// <param name="loggingService">日志服务</param>
    public ExampleDataModelLoader(JsonDataLoader jsonLoader, LoggingService loggingService)
    {
        _jsonLoader = jsonLoader ?? throw new ArgumentNullException(nameof(jsonLoader));
        _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
    }

    /// <summary>
    /// 加载数据模型
    /// </summary>
    /// <returns>数据模型集合</returns>
    public async Task<IDataModelCollection<AchievementData>> LoadAsync()
    {
        if (_cachedData != null)
        {
            _loggingService.LogDebug("返回缓存的成就数据", "ExampleDataModelLoader");
            return _cachedData;
        }

        try
        {
            _loggingService.LogInformation("开始加载成就数据", "ExampleDataModelLoader");

            var achievementsData = await _jsonLoader.LoadJsonFileAsync<Dictionary<string, string>>("Data/Achievements.json");
            if (achievementsData == null)
            {
                _loggingService.LogWarning("成就数据文件为空或加载失败", "ExampleDataModelLoader");
                _cachedData = new AchievementDataCollection(Enumerable.Empty<AchievementData>());
                return _cachedData;
            }

            var achievements = achievementsData.Select(kvp => new AchievementData
            {
                Id = kvp.Key,
                Name = ExtractAchievementName(kvp.Value),
                Description = ExtractAchievementDescription(kvp.Value),
                IsHidden = ExtractAchievementIsHidden(kvp.Value),
                PrerequisiteId = ExtractAchievementPrerequisite(kvp.Value),
                IconId = ExtractAchievementIcon(kvp.Value)
            }).ToList();

            _cachedData = new AchievementDataCollection(achievements);
            _loggingService.LogInformation($"成功加载 {achievements.Count} 个成就", "ExampleDataModelLoader");
            return _cachedData;
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"加载成就数据时发生错误: {ex.Message}", ex, "ExampleDataModelLoader");
            _cachedData = new AchievementDataCollection(Enumerable.Empty<AchievementData>());
            return _cachedData;
        }
    }

    /// <summary>
    /// 重新加载数据模型
    /// </summary>
    /// <returns>数据模型集合</returns>
    public async Task<IDataModelCollection<AchievementData>> ReloadAsync()
    {
        _loggingService.LogInformation("重新加载成就数据", "ExampleDataModelLoader");
        _cachedData = null; // 清除缓存
        return await LoadAsync();
    }

    #region 私有辅助方法

    private static string ExtractAchievementName(string value)
    {
        var parts = value.Split('^');
        return parts.Length > 0 ? parts[0] : value;
    }

    private static string ExtractAchievementDescription(string value)
    {
        var parts = value.Split('^');
        return parts.Length > 1 ? parts[1] : string.Empty;
    }

    private static bool ExtractAchievementIsHidden(string value)
    {
        var parts = value.Split('^');
        return parts.Length > 2 && bool.TryParse(parts[2], out var isHidden) && isHidden;
    }

    private static int ExtractAchievementPrerequisite(string value)
    {
        var parts = value.Split('^');
        return parts.Length > 3 && int.TryParse(parts[3], out var prereq) ? prereq : -1;
    }

    private static int ExtractAchievementIcon(string value)
    {
        var parts = value.Split('^');
        return parts.Length > 4 && int.TryParse(parts[4], out var icon) ? icon : 0;
    }

    #endregion
}
