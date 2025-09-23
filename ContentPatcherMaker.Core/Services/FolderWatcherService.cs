using System.IO;
using ContentPatcherMaker.Core.Services.Logging;

namespace ContentPatcherMaker.Core.Services;

/// <summary>
/// 文件夹监控服务
/// 监控指定文件夹的变化，支持文件过滤和回调处理
/// </summary>
public class FolderWatcherService : IDisposable
{
    private readonly LoggingService _loggingService;
    private readonly List<FileSystemWatcher> _watchers = new();
    private readonly Dictionary<string, Action<string>> _fileChangeCallbacks = new();
    private bool _disposed = false;

    public FolderWatcherService(LoggingService loggingService)
    {
        _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
    }

    /// <summary>
    /// 开始监控文件夹
    /// </summary>
    /// <param name="folderPath">要监控的文件夹路径</param>
    /// <param name="includeSubdirectories">是否包含子目录</param>
    /// <param name="filter">文件过滤器，默认为"*.*"</param>
    /// <param name="onFileChanged">文件变化回调</param>
    public void StartWatching(string folderPath, bool includeSubdirectories = true, string filter = "*.*", Action<string>? onFileChanged = null)
    {
        if (_disposed)
        {
            _loggingService.LogWarning("服务已释放，无法开始监控", "FolderWatcherService");
            return;
        }

        if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
        {
            _loggingService.LogWarning($"文件夹不存在或路径无效: {folderPath}", "FolderWatcherService");
            return;
        }

        try
        {
            var watcher = new FileSystemWatcher(folderPath)
            {
                IncludeSubdirectories = includeSubdirectories,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Filter = filter
            };

            watcher.Created += OnFileCreated;
            watcher.Changed += OnFileChanged;
            watcher.Deleted += OnFileDeleted;
            watcher.Renamed += OnFileRenamed;
            watcher.Error += OnWatcherError;

            watcher.EnableRaisingEvents = true;
            _watchers.Add(watcher);

            // 保存回调
            if (onFileChanged != null)
            {
                _fileChangeCallbacks[folderPath] = onFileChanged;
            }

            _loggingService.LogInformation($"开始监控文件夹: {folderPath} (包含子目录: {includeSubdirectories}, 过滤器: {filter})", "FolderWatcherService");
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"开始监控文件夹失败: {folderPath}, 错误: {ex.Message}", ex, "FolderWatcherService");
        }
    }

    /// <summary>
    /// 停止监控文件夹
    /// </summary>
    /// <param name="folderPath">要停止监控的文件夹路径</param>
    public void StopWatching(string folderPath)
    {
        var watcher = _watchers.FirstOrDefault(w => w.Path == folderPath);
        if (watcher != null)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
            _watchers.Remove(watcher);
            _fileChangeCallbacks.Remove(folderPath);
            _loggingService.LogInformation($"停止监控文件夹: {folderPath}", "FolderWatcherService");
        }
    }

    /// <summary>
    /// 停止所有监控
    /// </summary>
    public void StopAllWatching()
    {
        foreach (var watcher in _watchers)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }
        _watchers.Clear();
        _fileChangeCallbacks.Clear();
        _loggingService.LogInformation("已停止所有文件夹监控", "FolderWatcherService");
    }

    /// <summary>
    /// 获取监控的文件夹列表
    /// </summary>
    /// <returns>文件夹路径列表</returns>
    public IEnumerable<string> GetWatchedFolders()
    {
        return _watchers.Select(w => w.Path).ToList();
    }

    /// <summary>
    /// 检查是否正在监控指定文件夹
    /// </summary>
    /// <param name="folderPath">文件夹路径</param>
    /// <returns>是否正在监控</returns>
    public bool IsWatching(string folderPath)
    {
        return _watchers.Any(w => w.Path == folderPath);
    }

    /// <summary>
    /// 获取监控统计信息
    /// </summary>
    /// <returns>统计信息</returns>
    public FolderWatcherStatistics GetStatistics()
    {
        return new FolderWatcherStatistics
        {
            WatchedFoldersCount = _watchers.Count,
            WatchedFolders = _watchers.Select(w => w.Path).ToList(),
            CallbacksCount = _fileChangeCallbacks.Count,
            IsDisposed = _disposed
        };
    }

    #region 文件系统事件处理

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        _loggingService.LogInformation($"文件创建: {e.FullPath}", "FolderWatcherService");
        ProcessFileChange(e.FullPath, "Created");
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        _loggingService.LogInformation($"文件修改: {e.FullPath}", "FolderWatcherService");
        ProcessFileChange(e.FullPath, "Changed");
    }

    private void OnFileDeleted(object sender, FileSystemEventArgs e)
    {
        _loggingService.LogInformation($"文件删除: {e.FullPath}", "FolderWatcherService");
        ProcessFileChange(e.FullPath, "Deleted");
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        _loggingService.LogInformation($"文件重命名: {e.OldFullPath} -> {e.FullPath}", "FolderWatcherService");
        ProcessFileChange(e.FullPath, "Renamed");
    }

    private void OnWatcherError(object sender, ErrorEventArgs e)
    {
        _loggingService.LogError($"文件夹监控错误: {e.GetException().Message}", e.GetException(), "FolderWatcherService");
    }

    private void ProcessFileChange(string filePath, string changeType)
    {
        try
        {
            // 查找对应的回调
            var folderPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(folderPath))
            {
                // 查找最匹配的监控路径
                var watchedPath = _watchers
                    .Where(w => filePath.StartsWith(w.Path, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(w => w.Path.Length)
                    .FirstOrDefault()?.Path;

                if (!string.IsNullOrEmpty(watchedPath) && _fileChangeCallbacks.TryGetValue(watchedPath, out var callback))
                {
                    callback(filePath);
                }
            }

            _loggingService.LogDebug($"处理文件变化: {filePath}, 类型: {changeType}", "FolderWatcherService");
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"处理文件变化失败: {filePath}, 错误: {ex.Message}", ex, "FolderWatcherService");
        }
    }

    #endregion

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        StopAllWatching();
        _disposed = true;
        _loggingService.LogInformation("文件夹监控服务已释放", "FolderWatcherService");
    }
}

/// <summary>
/// 文件夹监控统计信息
/// </summary>
public class FolderWatcherStatistics
{
    /// <summary>
    /// 监控的文件夹数量
    /// </summary>
    public int WatchedFoldersCount { get; set; }

    /// <summary>
    /// 监控的文件夹列表
    /// </summary>
    public List<string> WatchedFolders { get; set; } = new();

    /// <summary>
    /// 回调数量
    /// </summary>
    public int CallbacksCount { get; set; }

    /// <summary>
    /// 是否已释放
    /// </summary>
    public bool IsDisposed { get; set; }

    /// <summary>
    /// 返回统计信息的字符串表示
    /// </summary>
    public override string ToString()
    {
        return $"文件夹监控统计: 监控文件夹 {WatchedFoldersCount} 个, 回调 {CallbacksCount} 个, 已释放 {IsDisposed}";
    }
}
