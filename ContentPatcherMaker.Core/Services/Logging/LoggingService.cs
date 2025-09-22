namespace ContentPatcherMaker.Core.Services.Logging;

/// <summary>
/// 日志服务
/// </summary>
public class LoggingService
{
    private readonly List<LogEntry> _logs = [];
    private readonly object _lockObject = new();
    private LogLevel _minimumLevel = LogLevel.Information;

    /// <summary>
    /// 设置最小日志级别
    /// </summary>
    /// <param name="level">日志级别</param>
    public void SetMinimumLevel(LogLevel level)
    {
        _minimumLevel = level;
    }

    /// <summary>
    /// 记录调试信息
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="context">上下文</param>
    public void LogDebug(string message, string? context = null)
    {
        Log(LogLevel.Debug, message, context);
    }

    /// <summary>
    /// 记录信息
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="context">上下文</param>
    public void LogInformation(string message, string? context = null)
    {
        Log(LogLevel.Information, message, context);
    }

    /// <summary>
    /// 记录警告
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="context">上下文</param>
    public void LogWarning(string message, string? context = null)
    {
        Log(LogLevel.Warning, message, context);
    }

    /// <summary>
    /// 记录错误
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="exception">异常</param>
    /// <param name="context">上下文</param>
    public void LogError(string message, Exception? exception = null, string? context = null)
    {
        Log(LogLevel.Error, message, context, exception);
    }

    /// <summary>
    /// 记录关键错误
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="exception">异常</param>
    /// <param name="context">上下文</param>
    public void LogCritical(string message, Exception? exception = null, string? context = null)
    {
        Log(LogLevel.Critical, message, context, exception);
    }

    /// <summary>
    /// 记录日志
    /// </summary>
    /// <param name="level">日志级别</param>
    /// <param name="message">消息</param>
    /// <param name="context">上下文</param>
    /// <param name="exception">异常</param>
    private void Log(LogLevel level, string message, string? context = null, Exception? exception = null)
    {
        if (level < _minimumLevel)
            return;

        lock (_lockObject)
        {
            var logEntry = new LogEntry
            {
                Level = level,
                Message = message,
                Context = context,
                Exception = exception,
                Timestamp = DateTime.UtcNow
            };

            _logs.Add(logEntry);

            // 限制日志数量，避免内存泄漏
            if (_logs.Count > 10000)
            {
                _logs.RemoveRange(0, 1000);
            }
        }
    }

    /// <summary>
    /// 获取指定级别的日志
    /// </summary>
    /// <param name="level">日志级别</param>
    /// <returns>日志列表</returns>
    public IReadOnlyList<LogEntry> GetLogs(LogLevel level)
    {
        lock (_lockObject)
        {
            return _logs.Where(l => l.Level >= level).ToList().AsReadOnly();
        }
    }

    /// <summary>
    /// 获取所有日志
    /// </summary>
    /// <returns>日志列表</returns>
    public IReadOnlyList<LogEntry> GetAllLogs()
    {
        lock (_lockObject)
        {
            return _logs.ToList().AsReadOnly();
        }
    }

    /// <summary>
    /// 获取最近的日志
    /// </summary>
    /// <param name="count">数量</param>
    /// <returns>日志列表</returns>
    public IReadOnlyList<LogEntry> GetRecentLogs(int count = 100)
    {
        lock (_lockObject)
        {
            return _logs.TakeLast(count).ToList().AsReadOnly();
        }
    }

    /// <summary>
    /// 清除所有日志
    /// </summary>
    public void ClearLogs()
    {
        lock (_lockObject)
        {
            _logs.Clear();
        }
    }

    /// <summary>
    /// 导出日志到文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="level">最小日志级别</param>
    public void ExportToFile(string filePath, LogLevel level = LogLevel.Debug)
    {
        try
        {
            var logs = GetLogs(level);
            var content = string.Join(Environment.NewLine, logs.Select(l => l.ToString()));
                
            File.WriteAllText(filePath, content);
            LogInformation($"日志已导出到: {filePath}");
        }
        catch (Exception ex)
        {
            LogError($"导出日志失败: {ex.Message}", ex, "LoggingService.ExportToFile");
        }
    }

    /// <summary>
    /// 获取日志统计信息
    /// </summary>
    /// <returns>统计信息</returns>
    public LogStatistics GetStatistics()
    {
        lock (_lockObject)
        {
            return new LogStatistics
            {
                TotalLogs = _logs.Count,
                DebugCount = _logs.Count(l => l.Level == LogLevel.Debug),
                InformationCount = _logs.Count(l => l.Level == LogLevel.Information),
                WarningCount = _logs.Count(l => l.Level == LogLevel.Warning),
                ErrorCount = _logs.Count(l => l.Level == LogLevel.Error),
                CriticalCount = _logs.Count(l => l.Level == LogLevel.Critical),
                FirstLogTime = _logs.FirstOrDefault()?.Timestamp,
                LastLogTime = _logs.LastOrDefault()?.Timestamp
            };
        }
    }
}

/// <summary>
/// 日志级别
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// 调试级别
    /// </summary>
    Debug = 0,
    
    /// <summary>
    /// 信息级别
    /// </summary>
    Information = 1,
    
    /// <summary>
    /// 警告级别
    /// </summary>
    Warning = 2,
    
    /// <summary>
    /// 错误级别
    /// </summary>
    Error = 3,
    
    /// <summary>
    /// 严重级别
    /// </summary>
    Critical = 4
}

/// <summary>
/// 日志条目
/// </summary>
public class LogEntry
{
    /// <summary>
    /// 日志级别
    /// </summary>
    public LogLevel Level { get; set; }
    
    /// <summary>
    /// 日志消息
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// 日志上下文
    /// </summary>
    public string? Context { get; set; }
    
    /// <summary>
    /// 异常对象
    /// </summary>
    public Exception? Exception { get; set; }
    
    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// 返回日志条目的字符串表示
    /// </summary>
    /// <returns>日志条目字符串</returns>
    public override string ToString()
    {
        var levelStr = Level switch
        {
            LogLevel.Debug => "DEBUG",
            LogLevel.Information => "INFO",
            LogLevel.Warning => "WARN",
            LogLevel.Error => "ERROR",
            LogLevel.Critical => "CRITICAL",
            _ => "UNKNOWN"
        };

        var result = $"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{levelStr}] {Message}";
            
        if (!string.IsNullOrEmpty(Context))
        {
            result += $" (上下文: {Context})";
        }
            
        if (Exception != null)
        {
            result += $"\n异常: {Exception}";
        }
            
        return result;
    }
}

/// <summary>
/// 日志统计信息
/// </summary>
public class LogStatistics
{
    /// <summary>
    /// 总日志数量
    /// </summary>
    public int TotalLogs { get; set; }
    
    /// <summary>
    /// 调试日志数量
    /// </summary>
    public int DebugCount { get; set; }
    
    /// <summary>
    /// 信息日志数量
    /// </summary>
    public int InformationCount { get; set; }
    
    /// <summary>
    /// 警告日志数量
    /// </summary>
    public int WarningCount { get; set; }
    
    /// <summary>
    /// 错误日志数量
    /// </summary>
    public int ErrorCount { get; set; }
    
    /// <summary>
    /// 严重日志数量
    /// </summary>
    public int CriticalCount { get; set; }
    
    /// <summary>
    /// 首次日志时间
    /// </summary>
    public DateTime? FirstLogTime { get; set; }
    
    /// <summary>
    /// 最后日志时间
    /// </summary>
    public DateTime? LastLogTime { get; set; }

    /// <summary>
    /// 返回日志统计的字符串表示
    /// </summary>
    /// <returns>日志统计字符串</returns>
    public override string ToString()
    {
        return $"日志统计: 总计 {TotalLogs} 条, " +
               $"调试 {DebugCount}, 信息 {InformationCount}, 警告 {WarningCount}, " +
               $"错误 {ErrorCount}, 严重 {CriticalCount}";
    }
}