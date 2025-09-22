using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using ContentPatcherMaker.Core.Services.Logging;

namespace ContentPatcherMaker.Core.DataModels;

/// <summary>
/// JSON数据加载器
/// </summary>
public class JsonDataLoader
{
    private readonly LoggingService _loggingService;
    private readonly string _contentPath;

    /// <summary>
    /// 初始化JSON数据加载器
    /// </summary>
    /// <param name="loggingService">日志服务</param>
    /// <param name="contentPath">Content文件夹路径</param>
    public JsonDataLoader(LoggingService loggingService, string contentPath)
    {
        _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        _contentPath = contentPath ?? throw new ArgumentNullException(nameof(contentPath));
    }

    /// <summary>
    /// 加载JSON文件
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="relativePath">相对路径</param>
    /// <returns>加载的数据，如果失败则返回null</returns>
    public async Task<T?> LoadJsonFileAsync<T>(string relativePath)
    {
        try
        {
            var fullPath = Path.Combine(_contentPath, relativePath);
            if (!File.Exists(fullPath))
            {
                _loggingService.LogWarning($"JSON文件不存在: {fullPath}", "JsonDataLoader");
                return default;
            }

            var jsonContent = await File.ReadAllTextAsync(fullPath);
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            var result = JsonConvert.DeserializeObject<T>(jsonContent, settings);
            _loggingService.LogDebug($"成功加载JSON文件: {relativePath}", "JsonDataLoader");
            return result;
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"加载JSON文件失败: {relativePath}, 错误: {ex.Message}", ex, "JsonDataLoader");
            return default;
        }
    }

    /// <summary>
    /// 加载JSON文件（同步版本）
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="relativePath">相对路径</param>
    /// <returns>加载的数据，如果失败则返回null</returns>
    public T? LoadJsonFile<T>(string relativePath)
    {
        try
        {
            var fullPath = Path.Combine(_contentPath, relativePath);
            if (!File.Exists(fullPath))
            {
                _loggingService.LogWarning($"JSON文件不存在: {fullPath}", "JsonDataLoader");
                return default;
            }

            var jsonContent = File.ReadAllText(fullPath);
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            var result = JsonConvert.DeserializeObject<T>(jsonContent, settings);
            _loggingService.LogDebug($"成功加载JSON文件: {relativePath}", "JsonDataLoader");
            return result;
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"加载JSON文件失败: {relativePath}, 错误: {ex.Message}", ex, "JsonDataLoader");
            return default;
        }
    }

    /// <summary>
    /// 加载多个JSON文件
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="relativePaths">相对路径列表</param>
    /// <returns>加载的数据列表</returns>
    public async Task<IEnumerable<T>> LoadJsonFilesAsync<T>(IEnumerable<string> relativePaths)
    {
        var results = new List<T>();
        var tasks = relativePaths.Select(async path =>
        {
            var data = await LoadJsonFileAsync<T>(path);
            return data;
        });

        var loadedData = await Task.WhenAll(tasks);
        results.AddRange(loadedData.Where(data => data != null)!);
        
        return results;
    }

    /// <summary>
    /// 获取指定目录下的所有JSON文件路径
    /// </summary>
    /// <param name="relativeDirectory">相对目录路径</param>
    /// <returns>JSON文件路径列表</returns>
    public IEnumerable<string> GetJsonFilePaths(string relativeDirectory)
    {
        try
        {
            var fullPath = Path.Combine(_contentPath, relativeDirectory);
            if (!Directory.Exists(fullPath))
            {
                _loggingService.LogWarning($"目录不存在: {fullPath}", "JsonDataLoader");
                return Enumerable.Empty<string>();
            }

            var jsonFiles = Directory.GetFiles(fullPath, "*.json", SearchOption.AllDirectories)
                .Select(fullPath => Path.GetRelativePath(_contentPath, fullPath))
                .ToList();

            _loggingService.LogDebug($"找到 {jsonFiles.Count} 个JSON文件在目录: {relativeDirectory}", "JsonDataLoader");
            return jsonFiles;
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"获取JSON文件路径失败: {relativeDirectory}, 错误: {ex.Message}", ex, "JsonDataLoader");
            return Enumerable.Empty<string>();
        }
    }
}
