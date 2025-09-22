using System;
using System.Collections.Generic;
using System.Linq;
using ContentPatcherMaker.Core.Models;
using ContentPatcherMaker.Core.Services.Logging;

namespace ContentPatcherMaker.Core.Extensions
{
    /// <summary>
    /// 扩展API服务实现
    /// </summary>
    public class ExtensionApiService : IExtensionApi
    {
        private readonly Dictionary<string, ICustomPatchValidator> _patchValidators = new();
        private readonly Dictionary<string, ICustomPatchProcessor> _patchProcessors = new();
        private readonly Dictionary<string, ITokenProvider> _tokenProviders = new();
        private readonly Dictionary<string, IValidationRule> _validationRules = new();
        private readonly Dictionary<string, IOutputFormatter> _outputFormatters = new();
        private readonly LoggingService _loggingService;

        public ExtensionApiService(LoggingService loggingService)
        {
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        }

        /// <summary>
        /// 注册自定义补丁类型
        /// </summary>
        public void RegisterCustomPatchType(string patchType, ICustomPatchValidator validator, ICustomPatchProcessor processor)
        {
            if (string.IsNullOrEmpty(patchType))
                throw new ArgumentException("补丁类型不能为空", nameof(patchType));

            if (validator == null)
                throw new ArgumentNullException(nameof(validator));

            if (processor == null)
                throw new ArgumentNullException(nameof(processor));

            _patchValidators[patchType] = validator;
            _patchProcessors[patchType] = processor;

            _loggingService.LogInformation($"已注册自定义补丁类型: {patchType}", "ExtensionApi");
        }

        /// <summary>
        /// 注册自定义令牌提供者
        /// </summary>
        public void RegisterTokenProvider(string tokenName, ITokenProvider provider)
        {
            if (string.IsNullOrEmpty(tokenName))
                throw new ArgumentException("令牌名称不能为空", nameof(tokenName));

            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            _tokenProviders[tokenName] = provider;

            _loggingService.LogInformation($"已注册令牌提供者: {tokenName}", "ExtensionApi");
        }

        /// <summary>
        /// 注册自定义验证规则
        /// </summary>
        public void RegisterValidationRule(string ruleName, IValidationRule validator)
        {
            if (string.IsNullOrEmpty(ruleName))
                throw new ArgumentException("规则名称不能为空", nameof(ruleName));

            if (validator == null)
                throw new ArgumentNullException(nameof(validator));

            _validationRules[ruleName] = validator;

            _loggingService.LogInformation($"已注册验证规则: {ruleName}", "ExtensionApi");
        }

        /// <summary>
        /// 注册自定义输出格式化器
        /// </summary>
        public void RegisterOutputFormatter(string formatType, IOutputFormatter formatter)
        {
            if (string.IsNullOrEmpty(formatType))
                throw new ArgumentException("格式类型不能为空", nameof(formatType));

            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            _outputFormatters[formatType] = formatter;

            _loggingService.LogInformation($"已注册输出格式化器: {formatType}", "ExtensionApi");
        }

        /// <summary>
        /// 获取所有已注册的补丁类型
        /// </summary>
        public IEnumerable<string> GetRegisteredPatchTypes()
        {
            return _patchValidators.Keys.ToList();
        }

        /// <summary>
        /// 获取所有已注册的令牌提供者
        /// </summary>
        public IEnumerable<string> GetRegisteredTokenProviders()
        {
            return _tokenProviders.Keys.ToList();
        }

        /// <summary>
        /// 验证自定义补丁
        /// </summary>
        public ValidationResult ValidateCustomPatch(string patchType, Dictionary<string, object> patchData)
        {
            if (string.IsNullOrEmpty(patchType))
                throw new ArgumentException("补丁类型不能为空", nameof(patchType));

            if (patchData == null)
                throw new ArgumentNullException(nameof(patchData));

            if (!_patchValidators.TryGetValue(patchType, out var validator))
            {
                var error = $"未找到补丁类型 '{patchType}' 的验证器";
                _loggingService.LogError(error, context: "ExtensionApi.ValidateCustomPatch");
                return new ValidationResult
                {
                    IsValid = false,
                    Errors = new List<string> { error }
                };
            }

            try
            {
                var result = validator.Validate(patchData);
                _loggingService.LogDebug($"验证补丁类型 '{patchType}': {(result.IsValid ? "成功" : "失败")}", "ExtensionApi");
                return result;
            }
            catch (Exception ex)
            {
                var error = $"验证补丁类型 '{patchType}' 时发生异常: {ex.Message}";
                _loggingService.LogError(error, ex, "ExtensionApi.ValidateCustomPatch");
                return new ValidationResult
                {
                    IsValid = false,
                    Errors = new List<string> { error }
                };
            }
        }

        /// <summary>
        /// 处理自定义补丁
        /// </summary>
        public ProcessingResult ProcessCustomPatch(string patchType, Dictionary<string, object> patchData)
        {
            if (string.IsNullOrEmpty(patchType))
                throw new ArgumentException("补丁类型不能为空", nameof(patchType));

            if (patchData == null)
                throw new ArgumentNullException(nameof(patchData));

            if (!_patchProcessors.TryGetValue(patchType, out var processor))
            {
                var error = $"未找到补丁类型 '{patchType}' 的处理器";
                _loggingService.LogError(error, context: "ExtensionApi.ProcessCustomPatch");
                return new ProcessingResult
                {
                    IsSuccess = false,
                    Errors = new List<string> { error }
                };
            }

            try
            {
                var result = processor.Process(patchData);
                _loggingService.LogDebug($"处理补丁类型 '{patchType}': {(result.IsSuccess ? "成功" : "失败")}", "ExtensionApi");
                return result;
            }
            catch (Exception ex)
            {
                var error = $"处理补丁类型 '{patchType}' 时发生异常: {ex.Message}";
                _loggingService.LogError(error, ex, "ExtensionApi.ProcessCustomPatch");
                return new ProcessingResult
                {
                    IsSuccess = false,
                    Errors = new List<string> { error }
                };
            }
        }

        /// <summary>
        /// 获取令牌值
        /// </summary>
        /// <param name="tokenName">令牌名称</param>
        /// <param name="context">上下文</param>
        /// <returns>令牌值</returns>
        public string? GetTokenValue(string tokenName, TokenContext context)
        {
            if (string.IsNullOrEmpty(tokenName))
                return null;

            if (_tokenProviders.TryGetValue(tokenName, out var provider))
            {
                try
                {
                    if (provider.IsAvailable(context))
                    {
                        return provider.GetValue(context);
                    }
                }
                catch (Exception ex)
                {
                    _loggingService.LogError($"获取令牌 '{tokenName}' 值时发生异常: {ex.Message}", ex, "ExtensionApi.GetTokenValue");
                }
            }

            return null;
        }

        /// <summary>
        /// 应用验证规则
        /// </summary>
        /// <param name="ruleName">规则名称</param>
        /// <param name="data">要验证的数据</param>
        /// <returns>验证结果</returns>
        public ValidationResult ApplyValidationRule(string ruleName, object data)
        {
            if (string.IsNullOrEmpty(ruleName))
                throw new ArgumentException("规则名称不能为空", nameof(ruleName));

            if (!_validationRules.TryGetValue(ruleName, out var validator))
            {
                var error = $"未找到验证规则 '{ruleName}'";
                _loggingService.LogError(error, context: "ExtensionApi.ApplyValidationRule");
                return new ValidationResult
                {
                    IsValid = false,
                    Errors = new List<string> { error }
                };
            }

            try
            {
                var result = validator.Validate(data);
                _loggingService.LogDebug($"应用验证规则 '{ruleName}': {(result.IsValid ? "成功" : "失败")}", "ExtensionApi");
                return result;
            }
            catch (Exception ex)
            {
                var error = $"应用验证规则 '{ruleName}' 时发生异常: {ex.Message}";
                _loggingService.LogError(error, ex, "ExtensionApi.ApplyValidationRule");
                return new ValidationResult
                {
                    IsValid = false,
                    Errors = new List<string> { error }
                };
            }
        }

        /// <summary>
        /// 格式化输出
        /// </summary>
        /// <param name="formatType">格式类型</param>
        /// <param name="contentPack">内容包</param>
        /// <returns>格式化后的字符串</returns>
        public string? FormatOutput(string formatType, ContentPack contentPack)
        {
            if (string.IsNullOrEmpty(formatType))
                throw new ArgumentException("格式类型不能为空", nameof(formatType));

            if (contentPack == null)
                throw new ArgumentNullException(nameof(contentPack));

            if (!_outputFormatters.TryGetValue(formatType, out var formatter))
            {
                var error = $"未找到输出格式化器 '{formatType}'";
                _loggingService.LogError(error, context: "ExtensionApi.FormatOutput");
                return null;
            }

            try
            {
                var result = formatter.Format(contentPack);
                _loggingService.LogDebug($"使用格式化器 '{formatType}' 格式化输出", "ExtensionApi");
                return result;
            }
            catch (Exception ex)
            {
                var error = $"使用格式化器 '{formatType}' 格式化输出时发生异常: {ex.Message}";
                _loggingService.LogError(error, ex, "ExtensionApi.FormatOutput");
                return null;
            }
        }

        /// <summary>
        /// 获取扩展统计信息
        /// </summary>
        /// <returns>统计信息</returns>
        public ExtensionStatistics GetStatistics()
        {
            return new ExtensionStatistics
            {
                RegisteredPatchTypes = _patchValidators.Count,
                RegisteredTokenProviders = _tokenProviders.Count,
                RegisteredValidationRules = _validationRules.Count,
                RegisteredOutputFormatters = _outputFormatters.Count
            };
        }
    }

    /// <summary>
    /// 扩展统计信息
    /// </summary>
    public class ExtensionStatistics
    {
        public int RegisteredPatchTypes { get; set; }
        public int RegisteredTokenProviders { get; set; }
        public int RegisteredValidationRules { get; set; }
        public int RegisteredOutputFormatters { get; set; }

        public override string ToString()
        {
            return $"扩展统计: 补丁类型 {RegisteredPatchTypes}, 令牌提供者 {RegisteredTokenProviders}, " +
                   $"验证规则 {RegisteredValidationRules}, 输出格式化器 {RegisteredOutputFormatters}";
        }
    }
}

