using System.Text.Json;
using ContentPatcherMaker.Core.Services.Logging;

namespace ContentPatcherMaker.Core.Services.Persistence;

/// <summary>
/// JSON数据持久化服务实现
/// </summary>
public class JsonDataPersistenceService : IDataPersistenceService
{
    private readonly LoggingService _loggingService;
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonDataPersistenceService(LoggingService loggingService)
    {
        _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    /// <summary>
    /// 保存数据到文件
    /// </summary>
    public async Task SaveAsync<T>(T data, string filePath)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("文件路径不能为空", nameof(filePath));

        try
        {
            // 确保目录存在
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 序列化数据
            var json = JsonSerializer.Serialize(data, _jsonOptions);

            // 写入文件
            await File.WriteAllTextAsync(filePath, json);

            _loggingService.LogDebug($"数据已保存到: {filePath}", "JsonDataPersistenceService");
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"保存数据失败: {filePath}, 错误: {ex.Message}", ex, "JsonDataPersistenceService");
            throw;
        }
    }

    /// <summary>
    /// 从文件加载数据
    /// </summary>
    public async Task<T?> LoadAsync<T>(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("文件路径不能为空", nameof(filePath));

        if (!File.Exists(filePath))
        {
            _loggingService.LogWarning($"文件不存在: {filePath}", "JsonDataPersistenceService");
            return default;
        }

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            if (string.IsNullOrEmpty(json))
            {
                _loggingService.LogWarning($"文件为空: {filePath}", "JsonDataPersistenceService");
                return default;
            }

            var data = JsonSerializer.Deserialize<T>(json, _jsonOptions);
            _loggingService.LogDebug($"数据已从文件加载: {filePath}", "JsonDataPersistenceService");
            return data;
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"加载数据失败: {filePath}, 错误: {ex.Message}", ex, "JsonDataPersistenceService");
            return default;
        }
    }

    /// <summary>
    /// 检查文件是否存在
    /// </summary>
    public bool Exists(string filePath)
    {
        return !string.IsNullOrEmpty(filePath) && File.Exists(filePath);
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    public async Task DeleteAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("文件路径不能为空", nameof(filePath));

        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _loggingService.LogDebug($"文件已删除: {filePath}", "JsonDataPersistenceService");
            }
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"删除文件失败: {filePath}, 错误: {ex.Message}", ex, "JsonDataPersistenceService");
            throw;
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// 获取文件信息
    /// </summary>
    public FileInfo? GetFileInfo(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            return null;

        try
        {
            return new FileInfo(filePath);
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"获取文件信息失败: {filePath}, 错误: {ex.Message}", ex, "JsonDataPersistenceService");
            return null;
        }
    }

    /// <summary>
    /// 备份文件
    /// </summary>
    public async Task BackupAsync(string filePath, string backupPath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("原文件路径不能为空", nameof(filePath));

        if (string.IsNullOrEmpty(backupPath))
            throw new ArgumentException("备份路径不能为空", nameof(backupPath));

        try
        {
            if (!File.Exists(filePath))
            {
                _loggingService.LogWarning($"原文件不存在，无法备份: {filePath}", "JsonDataPersistenceService");
                return;
            }

            // 确保备份目录存在
            var backupDirectory = Path.GetDirectoryName(backupPath);
            if (!string.IsNullOrEmpty(backupDirectory) && !Directory.Exists(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
            }

            File.Copy(filePath, backupPath, true);
            _loggingService.LogDebug($"文件已备份: {filePath} -> {backupPath}", "JsonDataPersistenceService");
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"备份文件失败: {filePath}, 错误: {ex.Message}", ex, "JsonDataPersistenceService");
            throw;
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// 恢复文件
    /// </summary>
    public async Task RestoreAsync(string backupPath, string filePath)
    {
        if (string.IsNullOrEmpty(backupPath))
            throw new ArgumentException("备份路径不能为空", nameof(backupPath));

        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("目标文件路径不能为空", nameof(filePath));

        try
        {
            if (!File.Exists(backupPath))
            {
                _loggingService.LogWarning($"备份文件不存在，无法恢复: {backupPath}", "JsonDataPersistenceService");
                return;
            }

            // 确保目标目录存在
            var targetDirectory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(targetDirectory) && !Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            File.Copy(backupPath, filePath, true);
            _loggingService.LogDebug($"文件已恢复: {backupPath} -> {filePath}", "JsonDataPersistenceService");
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"恢复文件失败: {backupPath}, 错误: {ex.Message}", ex, "JsonDataPersistenceService");
            throw;
        }

        await Task.CompletedTask;
    }
}
