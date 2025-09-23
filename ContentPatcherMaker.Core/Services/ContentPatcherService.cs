using System.Diagnostics.CodeAnalysis;
using ContentPatcherMaker.Core.Models;
using ContentPatcherMaker.Core.Services.ErrorHandling;
using ContentPatcherMaker.Core.Services.Logging;
using ContentPatcherMaker.Core.Validation;
using ValidationResult = ContentPatcherMaker.Core.Validation.ValidationResult;

namespace ContentPatcherMaker.Core.Services;

/// <summary>
/// ContentPatcher主服务类，整合所有功能
/// </summary>
public class ContentPatcherService
{
    private readonly ContentPatcherValidator _validator;
    private readonly ErrorHandlingService _errorHandlingService;
    private readonly LoggingService _loggingService;
    private readonly JsonGeneratorService _jsonGeneratorService;
    private readonly StardewValleyCompatibilityService _compatibilityService;

    /// <summary>
    /// 初始化ContentPatcher服务
    /// </summary>
    public ContentPatcherService()
    {
        _loggingService = new LoggingService();
        _errorHandlingService = new ErrorHandlingService();
        _validator = new ContentPatcherValidator();
        _jsonGeneratorService = new JsonGeneratorService(_loggingService);
        _compatibilityService = new StardewValleyCompatibilityService(_loggingService);
    }

    /// <summary>
    /// 创建新的内容包
    /// </summary>
    /// <returns>新的内容包</returns>
    public ContentPack CreateContentPack()
    {
        try
        {
            const string latestFormat = "2.8.0";
            _loggingService.LogInformation($"创建新的ContentPatcher内容包，格式版本: {latestFormat}", "ContentPatcherService");

            var contentPack = new ContentPack
            {
                Format = latestFormat,
                Changes = []
            };

            _loggingService.LogInformation("ContentPatcher内容包创建成功", "ContentPatcherService");
            return contentPack;
        }
        catch (Exception ex)
        {
            _errorHandlingService.HandleException(ex, "ContentPatcherService.CreateContentPack");
            _loggingService.LogError($"创建ContentPatcher内容包失败: {ex.Message}", ex, "ContentPatcherService");
            throw;
        }
    }

    /// <summary>
    /// 验证内容包
    /// </summary>
    /// <param name="contentPack">要验证的内容包</param>
    /// <returns>验证结果</returns>
    public ValidationResult ValidateContentPack(ContentPack contentPack)
    {
        if (contentPack == null)
            throw new ArgumentNullException(nameof(contentPack));

        try
        {
            _loggingService.LogInformation("开始验证ContentPatcher内容包", "ContentPatcherService");

            var result = _validator.ValidateContentPack(contentPack);

            if (result.IsValid)
            {
                _loggingService.LogInformation("ContentPatcher内容包验证成功", "ContentPatcherService");
            }
            else
            {
                _loggingService.LogWarning($"ContentPatcher内容包验证失败: {string.Join(", ", result.Errors)}", "ContentPatcherService");
            }

            return result;
        }
        catch (Exception ex)
        {
            _errorHandlingService.HandleException(ex, "ContentPatcherService.ValidateContentPack");
            _loggingService.LogError($"验证ContentPatcher内容包时发生异常: {ex.Message}", ex, "ContentPatcherService");
            throw;
        }
    }

    /// <summary>
    /// 检查Stardew Valley兼容性
    /// </summary>
    /// <param name="contentPack">要检查的内容包</param>
    /// <returns>兼容性结果</returns>
    public CompatibilityResult CheckCompatibility(ContentPack contentPack)
    {
        if (contentPack == null)
            throw new ArgumentNullException(nameof(contentPack));

        try
        {
            _loggingService.LogInformation("开始检查Stardew Valley兼容性", "ContentPatcherService");

            var result = _compatibilityService.ValidateCompatibility(contentPack);

            if (result.IsCompatible)
            {
                _loggingService.LogInformation("Stardew Valley兼容性检查通过", "ContentPatcherService");
            }
            else
            {
                _loggingService.LogWarning($"Stardew Valley兼容性检查失败: {string.Join(", ", result.Errors)}", "ContentPatcherService");
            }

            return result;
        }
        catch (Exception ex)
        {
            _errorHandlingService.HandleException(ex, "ContentPatcherService.CheckCompatibility");
            _loggingService.LogError($"检查Stardew Valley兼容性时发生异常: {ex.Message}", ex, "ContentPatcherService");
            throw;
        }
    }

    /// <summary>
    /// 生成JSON内容
    /// </summary>
    /// <param name="contentPack">内容包</param>
    /// <returns>JSON字符串</returns>
    public string GenerateJson(ContentPack contentPack)
    {
        if (contentPack == null)
            throw new ArgumentNullException(nameof(contentPack));

        try
        {
            _loggingService.LogInformation("开始生成ContentPatcher JSON", "ContentPatcherService");

            var json = _jsonGeneratorService.GenerateJson(contentPack);

            _loggingService.LogInformation("ContentPatcher JSON生成成功", "ContentPatcherService");
            return json;
        }
        catch (Exception ex)
        {
            _errorHandlingService.HandleException(ex, "ContentPatcherService.GenerateJson");
            _loggingService.LogError($"生成ContentPatcher JSON时发生异常: {ex.Message}", ex, "ContentPatcherService");
            throw;
        }
    }

    /// <summary>
    /// 保存内容包到文件
    /// </summary>
    /// <param name="contentPack">内容包</param>
    /// <param name="filePath">文件路径</param>
    public void SaveContentPack(ContentPack contentPack, string filePath)
    {
        if (contentPack == null)
            throw new ArgumentNullException(nameof(contentPack));

        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("文件路径不能为空", nameof(filePath));

        try
        {
            _loggingService.LogInformation($"开始保存ContentPatcher内容包到: {filePath}", "ContentPatcherService");

            _jsonGeneratorService.GenerateAndSaveJson(contentPack, filePath);

            _loggingService.LogInformation($"ContentPatcher内容包保存成功: {filePath}", "ContentPatcherService");
        }
        catch (Exception ex)
        {
            _errorHandlingService.HandleException(ex, "ContentPatcherService.SaveContentPack");
            _loggingService.LogError($"保存ContentPatcher内容包时发生异常: {ex.Message}", ex, "ContentPatcherService");
            throw;
        }
    }

    /// <summary>
    /// 从文件加载内容包
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>内容包</returns>
    public ContentPack LoadContentPack(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("文件路径不能为空", nameof(filePath));

        try
        {
            _loggingService.LogInformation($"开始从文件加载ContentPatcher内容包: {filePath}", "ContentPatcherService");

            var contentPack = _jsonGeneratorService.LoadFromFile(filePath);

            _loggingService.LogInformation($"ContentPatcher内容包加载成功: {filePath}", "ContentPatcherService");
            return contentPack;
        }
        catch (Exception ex)
        {
            _errorHandlingService.HandleException(ex, "ContentPatcherService.LoadContentPack");
            _loggingService.LogError($"加载ContentPatcher内容包时发生异常: {ex.Message}", ex, "ContentPatcherService");
            throw;
        }
    }

    /// <summary>
    /// 添加补丁到内容包
    /// </summary>
    /// <param name="contentPack">内容包</param>
    /// <param name="patch">补丁</param>
    public void AddPatch(ContentPack contentPack, Patch patch)
    {
        if (contentPack == null)
            throw new ArgumentNullException(nameof(contentPack));

        if (patch == null)
            throw new ArgumentNullException(nameof(patch));

        try
        {
            _loggingService.LogInformation($"添加补丁到内容包: {patch.Action}", "ContentPatcherService");

            contentPack.Changes.Add(patch);

            _loggingService.LogInformation($"补丁添加成功: {patch.Action}", "ContentPatcherService");
        }
        catch (Exception ex)
        {
            _errorHandlingService.HandleException(ex, "ContentPatcherService.AddPatch");
            _loggingService.LogError($"添加补丁时发生异常: {ex.Message}", ex, "ContentPatcherService");
            throw;
        }
    }

    /// <summary>
    /// 移除补丁
    /// </summary>
    /// <param name="contentPack">内容包</param>
    /// <param name="patchIndex">补丁索引</param>
    public void RemovePatch(ContentPack contentPack, int patchIndex)
    {
        if (contentPack == null)
            throw new ArgumentNullException(nameof(contentPack));

        if (patchIndex < 0 || patchIndex >= contentPack.Changes.Count)
            throw new ArgumentOutOfRangeException(nameof(patchIndex));

        try
        {
            _loggingService.LogInformation($"移除补丁: {patchIndex}", "ContentPatcherService");

            var patch = contentPack.Changes[patchIndex];
            contentPack.Changes.RemoveAt(patchIndex);

            _loggingService.LogInformation($"补丁移除成功: {patch.Action}", "ContentPatcherService");
        }
        catch (Exception ex)
        {
            _errorHandlingService.HandleException(ex, "ContentPatcherService.RemovePatch");
            _loggingService.LogError($"移除补丁时发生异常: {ex.Message}", ex, "ContentPatcherService");
            throw;
        }
    }

    /// <summary>
    /// 获取错误处理服务
    /// </summary>
    /// <returns>错误处理服务</returns>
    public ErrorHandlingService GetErrorHandlingService()
    {
        return _errorHandlingService;
    }

    /// <summary>
    /// 获取日志服务
    /// </summary>
    /// <returns>日志服务</returns>
    public LoggingService GetLoggingService()
    {
        return _loggingService;
    }

    /// <summary>
    /// 获取服务统计信息
    /// </summary>
    /// <returns>统计信息</returns>
    public ServiceStatistics GetStatistics()
    {
        return new ServiceStatistics
        {
            ErrorCount = _errorHandlingService.GetErrors().Count,
            WarningCount = _errorHandlingService.GetWarnings().Count,
            LogCount = _loggingService.GetAllLogs().Count,
        };
    }

    /// <summary>
    /// 清除所有错误和日志
    /// </summary>
    public void ClearAll()
    {
        _errorHandlingService.Clear();
        _loggingService.ClearLogs();
        _loggingService.LogInformation("已清除所有错误和日志", "ContentPatcherService");
    }
}

/// <summary>
/// 服务统计信息
/// </summary>
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class ServiceStatistics
{
    /// <summary>
    /// 错误数量
    /// </summary>
    public int ErrorCount { get; set; }
        
    /// <summary>
    /// 警告数量
    /// </summary>
    public int WarningCount { get; set; }
        
    /// <summary>
    /// 日志数量
    /// </summary>
    public int LogCount { get; set; }

    /// <summary>
    /// 返回统计信息的字符串表示
    /// </summary>
    /// <returns>统计信息字符串</returns>
    public override string ToString()
    {
        return $"服务统计: 错误 {ErrorCount}, 警告 {WarningCount}, 日志 {LogCount}";
    }
}