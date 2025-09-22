using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using ContentPatcherMaker.Core.Models;
using ContentPatcherMaker.Core.Services.Logging;

namespace ContentPatcherMaker.Core.Services
{
    /// <summary>
    /// JSON生成服务
    /// </summary>
    public class JsonGeneratorService
    {
        private readonly LoggingService _loggingService;
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonGeneratorService(LoggingService loggingService)
        {
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter() }
            };
        }

        /// <summary>
        /// 生成ContentPatcher JSON内容
        /// </summary>
        /// <param name="contentPack">内容包</param>
        /// <returns>JSON字符串</returns>
        public string GenerateJson(ContentPack contentPack)
        {
            if (contentPack == null)
                throw new ArgumentNullException(nameof(contentPack));

            try
            {
                _loggingService.LogInformation("开始生成ContentPatcher JSON", "JsonGenerator");

                // 验证内容包
                ValidateContentPack(contentPack);

                // 生成JSON
                var json = JsonSerializer.Serialize(contentPack, _jsonOptions);
                
                _loggingService.LogInformation("ContentPatcher JSON生成成功", "JsonGenerator");
                return json;
            }
            catch (Exception ex)
            {
                _loggingService.LogError($"生成ContentPatcher JSON失败: {ex.Message}", ex, "JsonGenerator");
                throw;
            }
        }

        /// <summary>
        /// 生成并保存JSON文件
        /// </summary>
        /// <param name="contentPack">内容包</param>
        /// <param name="filePath">文件路径</param>
        public void GenerateAndSaveJson(ContentPack contentPack, string filePath)
        {
            if (contentPack == null)
                throw new ArgumentNullException(nameof(contentPack));

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("文件路径不能为空", nameof(filePath));

            try
            {
                _loggingService.LogInformation($"开始生成并保存JSON文件: {filePath}", "JsonGenerator");

                var json = GenerateJson(contentPack);
                
                // 确保目录存在
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 保存文件
                File.WriteAllText(filePath, json);
                
                _loggingService.LogInformation($"JSON文件保存成功: {filePath}", "JsonGenerator");
            }
            catch (Exception ex)
            {
                _loggingService.LogError($"生成并保存JSON文件失败: {ex.Message}", ex, "JsonGenerator");
                throw;
            }
        }

        /// <summary>
        /// 从JSON字符串解析内容包
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <returns>内容包</returns>
        public ContentPack ParseFromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                throw new ArgumentException("JSON字符串不能为空", nameof(json));

            try
            {
                _loggingService.LogInformation("开始解析ContentPatcher JSON", "JsonGenerator");

                var contentPack = JsonSerializer.Deserialize<ContentPack>(json, _jsonOptions);
                
                if (contentPack == null)
                    throw new InvalidOperationException("JSON解析结果为空");

                _loggingService.LogInformation("ContentPatcher JSON解析成功", "JsonGenerator");
                return contentPack;
            }
            catch (Exception ex)
            {
                _loggingService.LogError($"解析ContentPatcher JSON失败: {ex.Message}", ex, "JsonGenerator");
                throw;
            }
        }

        /// <summary>
        /// 从文件加载内容包
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>内容包</returns>
        public ContentPack LoadFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("文件路径不能为空", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"文件不存在: {filePath}");

            try
            {
                _loggingService.LogInformation($"开始从文件加载ContentPatcher JSON: {filePath}", "JsonGenerator");

                var json = File.ReadAllText(filePath);
                var contentPack = ParseFromJson(json);
                
                _loggingService.LogInformation($"从文件加载ContentPatcher JSON成功: {filePath}", "JsonGenerator");
                return contentPack;
            }
            catch (Exception ex)
            {
                _loggingService.LogError($"从文件加载ContentPatcher JSON失败: {ex.Message}", ex, "JsonGenerator");
                throw;
            }
        }

        /// <summary>
        /// 验证JSON格式
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <returns>验证结果</returns>
        public JsonValidationResult ValidateJson(string json)
        {
            var result = new JsonValidationResult();

            if (string.IsNullOrEmpty(json))
            {
                result.IsValid = false;
                result.Errors.Add("JSON字符串不能为空");
                return result;
            }

            try
            {
                // 尝试解析JSON
                JsonDocument.Parse(json);
                result.IsValid = true;
                
                // 尝试解析为ContentPack
                var contentPack = JsonSerializer.Deserialize<ContentPack>(json, _jsonOptions);
                if (contentPack == null)
                {
                    result.IsValid = false;
                    result.Errors.Add("JSON解析结果为空");
                }
                else
                {
                    // 验证ContentPatcher特定规则
                    ValidateContentPatcherRules(contentPack, result);
                }
            }
            catch (JsonException ex)
            {
                result.IsValid = false;
                result.Errors.Add($"JSON格式错误: {ex.Message}");
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.Errors.Add($"解析错误: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// 验证内容包
        /// </summary>
        private void ValidateContentPack(ContentPack contentPack)
        {
            if (string.IsNullOrEmpty(contentPack.Format))
                throw new InvalidOperationException("Format字段不能为空");

            if (contentPack.Changes == null || contentPack.Changes.Count == 0)
                throw new InvalidOperationException("Changes字段不能为空");

            // 验证每个补丁
            for (int i = 0; i < contentPack.Changes.Count; i++)
            {
                var patch = contentPack.Changes[i];
                ValidatePatch(patch, i);
            }
        }

        /// <summary>
        /// 验证补丁
        /// </summary>
        private void ValidatePatch(Patch patch, int index)
        {
            if (patch == null)
                throw new InvalidOperationException($"补丁 {index} 不能为空");

            if (string.IsNullOrEmpty(patch.Action))
                throw new InvalidOperationException($"补丁 {index} 的Action字段不能为空");

            if (string.IsNullOrEmpty(patch.Target))
                throw new InvalidOperationException($"补丁 {index} 的Target字段不能为空");

            // 根据操作类型验证特定字段
            switch (patch.Action)
            {
                case "Load":
                    ValidateLoadPatch((LoadPatch)patch, index);
                    break;
                case "EditData":
                    ValidateEditDataPatch((EditDataPatch)patch, index);
                    break;
                case "EditImage":
                    ValidateEditImagePatch((EditImagePatch)patch, index);
                    break;
                case "EditMap":
                    ValidateEditMapPatch((EditMapPatch)patch, index);
                    break;
                case "Include":
                    ValidateIncludePatch((IncludePatch)patch, index);
                    break;
            }
        }

        /// <summary>
        /// 验证Load补丁
        /// </summary>
        private void ValidateLoadPatch(LoadPatch patch, int index)
        {
            if (string.IsNullOrEmpty(patch.FromFile))
                throw new InvalidOperationException($"Load补丁 {index} 的FromFile字段不能为空");
        }

        /// <summary>
        /// 验证EditData补丁
        /// </summary>
        private void ValidateEditDataPatch(EditDataPatch patch, int index)
        {
            var hasAnyOperation = patch.Fields != null || patch.Entries != null || 
                                 patch.MoveEntries != null || patch.TextOperations != null;

            if (!hasAnyOperation)
                throw new InvalidOperationException($"EditData补丁 {index} 必须包含至少一个操作");
        }

        /// <summary>
        /// 验证EditImage补丁
        /// </summary>
        private void ValidateEditImagePatch(EditImagePatch patch, int index)
        {
            if (string.IsNullOrEmpty(patch.FromFile))
                throw new InvalidOperationException($"EditImage补丁 {index} 的FromFile字段不能为空");
        }

        /// <summary>
        /// 验证EditMap补丁
        /// </summary>
        private void ValidateEditMapPatch(EditMapPatch patch, int index)
        {
            var hasAnyOperation = !string.IsNullOrEmpty(patch.FromFile) || 
                                 patch.MapProperties != null || 
                                 patch.AddNpcWarps != null || 
                                 patch.AddWarps != null || 
                                 patch.MapTiles != null || 
                                 patch.TextOperations != null;

            if (!hasAnyOperation)
                throw new InvalidOperationException($"EditMap补丁 {index} 必须包含至少一个操作");
        }

        /// <summary>
        /// 验证Include补丁
        /// </summary>
        private void ValidateIncludePatch(IncludePatch patch, int index)
        {
            if (string.IsNullOrEmpty(patch.FromFile))
                throw new InvalidOperationException($"Include补丁 {index} 的FromFile字段不能为空");
        }

        /// <summary>
        /// 验证ContentPatcher特定规则
        /// </summary>
        private void ValidateContentPatcherRules(ContentPack contentPack, JsonValidationResult result)
        {
            // 验证格式版本
            if (!IsValidFormatVersion(contentPack.Format))
            {
                result.Warnings.Add($"格式版本 '{contentPack.Format}' 可能不是最新版本");
            }

            // 验证补丁操作类型
            var validActions = new[] { "Load", "EditData", "EditImage", "EditMap", "Include" };
            for (int i = 0; i < contentPack.Changes.Count; i++)
            {
                var patch = contentPack.Changes[i];
                if (!validActions.Contains(patch.Action))
                {
                    result.Warnings.Add($"补丁 {i} 包含未知的操作类型: {patch.Action}");
                }
            }
        }

        /// <summary>
        /// 检查格式版本是否有效
        /// </summary>
        private bool IsValidFormatVersion(string format)
        {
            if (string.IsNullOrEmpty(format))
                return false;

            // 检查版本格式
            var versionPattern = @"^\d+\.\d+\.\d+$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(format, versionPattern))
                return false;

            // 检查版本是否过旧
            try
            {
                var version = new Version(format);
                var minVersion = new Version("1.0.0");
                return version >= minVersion;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// JSON验证结果
    /// </summary>
    public class JsonValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();

        public override string ToString()
        {
            var result = $"JSON验证结果: {(IsValid ? "有效" : "无效")}";
            if (Errors.Any())
            {
                result += $"\n错误: {string.Join(", ", Errors)}";
            }
            if (Warnings.Any())
            {
                result += $"\n警告: {string.Join(", ", Warnings)}";
            }
            return result;
        }
    }
}

