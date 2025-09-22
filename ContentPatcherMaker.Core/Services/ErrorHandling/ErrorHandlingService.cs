using System;
using System.Collections.Generic;
using System.Linq;

namespace ContentPatcherMaker.Core.Services.ErrorHandling
{
    /// <summary>
    /// 错误处理服务
    /// </summary>
    public class ErrorHandlingService
    {
        private readonly List<ErrorInfo> _errors = new();
        private readonly List<WarningInfo> _warnings = new();

        /// <summary>
        /// 添加错误
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="exception">异常对象</param>
        /// <param name="context">错误上下文</param>
        public void AddError(string message, Exception? exception = null, string? context = null)
        {
            _errors.Add(new ErrorInfo
            {
                Message = message,
                Exception = exception,
                Context = context,
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// 添加警告
        /// </summary>
        /// <param name="message">警告消息</param>
        /// <param name="context">警告上下文</param>
        public void AddWarning(string message, string? context = null)
        {
            _warnings.Add(new WarningInfo
            {
                Message = message,
                Context = context,
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// 获取所有错误
        /// </summary>
        /// <returns>错误列表</returns>
        public IReadOnlyList<ErrorInfo> GetErrors()
        {
            return _errors.AsReadOnly();
        }

        /// <summary>
        /// 获取所有警告
        /// </summary>
        /// <returns>警告列表</returns>
        public IReadOnlyList<WarningInfo> GetWarnings()
        {
            return _warnings.AsReadOnly();
        }

        /// <summary>
        /// 检查是否有错误
        /// </summary>
        /// <returns>是否有错误</returns>
        public bool HasErrors()
        {
            return _errors.Any();
        }

        /// <summary>
        /// 检查是否有警告
        /// </summary>
        /// <returns>是否有警告</returns>
        public bool HasWarnings()
        {
            return _warnings.Any();
        }

        /// <summary>
        /// 清除所有错误和警告
        /// </summary>
        public void Clear()
        {
            _errors.Clear();
            _warnings.Clear();
        }

        /// <summary>
        /// 获取错误摘要
        /// </summary>
        /// <returns>错误摘要</returns>
        public ErrorSummary GetSummary()
        {
            return new ErrorSummary
            {
                ErrorCount = _errors.Count,
                WarningCount = _warnings.Count,
                HasErrors = HasErrors(),
                HasWarnings = HasWarnings(),
                LatestError = _errors.LastOrDefault(),
                LatestWarning = _warnings.LastOrDefault()
            };
        }

        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="context">上下文</param>
        public void HandleException(Exception exception, string? context = null)
        {
            var message = exception switch
            {
                ArgumentNullException nullEx => $"空参数错误: {nullEx.Message}",
                ArgumentException argEx => $"参数错误: {argEx.Message}",
                InvalidOperationException opEx => $"操作错误: {opEx.Message}",
                NotSupportedException notSupEx => $"不支持的操作: {notSupEx.Message}",
                _ => $"未处理的异常: {exception.Message}"
            };

            AddError(message, exception, context);
        }

        /// <summary>
        /// 验证操作结果
        /// </summary>
        /// <param name="isValid">是否有效</param>
        /// <param name="errorMessage">错误消息</param>
        /// <param name="context">上下文</param>
        /// <returns>验证是否通过</returns>
        public bool ValidateOperation(bool isValid, string errorMessage, string? context = null)
        {
            if (!isValid)
            {
                AddError(errorMessage, context: context);
            }
            return isValid;
        }
    }

    /// <summary>
    /// 错误信息
    /// </summary>
    public class ErrorInfo
    {
        public string Message { get; set; } = string.Empty;
        public Exception? Exception { get; set; }
        public string? Context { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            var result = $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Message}";
            if (!string.IsNullOrEmpty(Context))
            {
                result += $" (上下文: {Context})";
            }
            if (Exception != null)
            {
                result += $"\n异常详情: {Exception}";
            }
            return result;
        }
    }

    /// <summary>
    /// 警告信息
    /// </summary>
    public class WarningInfo
    {
        public string Message { get; set; } = string.Empty;
        public string? Context { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            var result = $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] 警告: {Message}";
            if (!string.IsNullOrEmpty(Context))
            {
                result += $" (上下文: {Context})";
            }
            return result;
        }
    }

    /// <summary>
    /// 错误摘要
    /// </summary>
    public class ErrorSummary
    {
        public int ErrorCount { get; set; }
        public int WarningCount { get; set; }
        public bool HasErrors { get; set; }
        public bool HasWarnings { get; set; }
        public ErrorInfo? LatestError { get; set; }
        public WarningInfo? LatestWarning { get; set; }

        public override string ToString()
        {
            var result = $"错误摘要: {ErrorCount} 个错误, {WarningCount} 个警告";
            if (LatestError != null)
            {
                result += $"\n最新错误: {LatestError.Message}";
            }
            if (LatestWarning != null)
            {
                result += $"\n最新警告: {LatestWarning.Message}";
            }
            return result;
        }
    }
}

