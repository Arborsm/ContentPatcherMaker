using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using ContentPatcherMaker.Core.Models;

namespace ContentPatcherMaker.Core.Validation
{
    /// <summary>
    /// ContentPatcher内容包验证器
    /// </summary>
    public class ContentPatcherValidator
    {
        private readonly List<ValidationError> _validationResults = new();

        /// <summary>
        /// 验证内容包
        /// </summary>
        /// <param name="contentPack">要验证的内容包</param>
        /// <returns>验证结果</returns>
        public ValidationResult ValidateContentPack(ContentPack contentPack)
        {
            _validationResults.Clear();

            // 验证基本结构
            ValidateFormat(contentPack.Format);
            ValidateChanges(contentPack.Changes);
            ValidateConfigSchema(contentPack.ConfigSchema);
            ValidateCustomLocations(contentPack.CustomLocations);
            ValidateDynamicTokens(contentPack.DynamicTokens);

            return new ValidationResult
            {
                IsValid = !_validationResults.Any(),
                Errors = _validationResults.Select(r => r.ErrorMessage).ToList(),
                Warnings = _validationResults.Where(r => r.Severity == ValidationSeverity.Warning)
                    .Select(r => r.ErrorMessage).ToList()
            };
        }

        /// <summary>
        /// 验证格式版本
        /// </summary>
        private void ValidateFormat(string format)
        {
            if (string.IsNullOrEmpty(format))
            {
                AddValidationError("Format字段不能为空", ValidationSeverity.Error);
                return;
            }

            // 验证格式版本格式
            var versionPattern = @"^\d+\.\d+\.\d+$";
            if (!Regex.IsMatch(format, versionPattern))
            {
                AddValidationError($"Format版本格式无效: {format}，应为 x.y.z 格式", ValidationSeverity.Error);
                return;
            }

            // 验证版本是否过旧
            var version = new Version(format);
            var minVersion = new Version("1.0.0");
            if (version < minVersion)
            {
                AddValidationError($"Format版本过旧: {format}，建议使用最新版本", ValidationSeverity.Warning);
            }
        }

        /// <summary>
        /// 验证补丁列表
        /// </summary>
        private void ValidateChanges(List<Patch> changes)
        {
            if (changes == null || !changes.Any())
            {
                AddValidationError("Changes字段不能为空", ValidationSeverity.Error);
                return;
            }

            for (int i = 0; i < changes.Count; i++)
            {
                var patch = changes[i];
                ValidatePatch(patch, i);
            }
        }

        /// <summary>
        /// 验证单个补丁
        /// </summary>
        private void ValidatePatch(Patch patch, int index)
        {
            if (patch == null)
            {
                AddValidationError($"补丁 {index} 不能为空", ValidationSeverity.Error);
                return;
            }

            // 验证基本字段
            ValidatePatchBasicFields(patch, index);

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
                default:
                    AddValidationError($"补丁 {index} 包含未知的操作类型: {patch.Action}", ValidationSeverity.Error);
                    break;
            }
        }

        /// <summary>
        /// 验证补丁基本字段
        /// </summary>
        private void ValidatePatchBasicFields(Patch patch, int index)
        {
            if (string.IsNullOrEmpty(patch.Target))
            {
                AddValidationError($"补丁 {index} 的Target字段不能为空", ValidationSeverity.Error);
            }

            // 验证更新频率
            if (!string.IsNullOrEmpty(patch.Update))
            {
                var validUpdates = new[] { "OnDayStart", "OnLocationChange", "OnTimeChange" };
                var updates = patch.Update.Split(',').Select(u => u.Trim());
                
                foreach (var update in updates)
                {
                    if (!validUpdates.Contains(update))
                    {
                        AddValidationError($"补丁 {index} 的Update字段包含无效值: {update}", ValidationSeverity.Error);
                    }
                }
            }

            // 验证优先级
            if (!string.IsNullOrEmpty(patch.Priority))
            {
                var validPriorities = new[] { "Early", "Default", "Late", "Low", "Medium", "High", "Exclusive" };
                if (!validPriorities.Contains(patch.Priority))
                {
                    AddValidationError($"补丁 {index} 的Priority字段包含无效值: {patch.Priority}", ValidationSeverity.Error);
                }
            }
        }

        /// <summary>
        /// 验证Load补丁
        /// </summary>
        private void ValidateLoadPatch(LoadPatch patch, int index)
        {
            if (string.IsNullOrEmpty(patch.FromFile))
            {
                AddValidationError($"Load补丁 {index} 的FromFile字段不能为空", ValidationSeverity.Error);
            }
        }

        /// <summary>
        /// 验证EditData补丁
        /// </summary>
        private void ValidateEditDataPatch(EditDataPatch patch, int index)
        {
            var hasAnyOperation = patch.Fields != null || patch.Entries != null || 
                                 patch.MoveEntries != null || patch.TextOperations != null;

            if (!hasAnyOperation)
            {
                AddValidationError($"EditData补丁 {index} 必须包含Fields、Entries、MoveEntries或TextOperations中的至少一个", ValidationSeverity.Error);
            }

            // 验证MoveEntries
            if (patch.MoveEntries != null)
            {
                foreach (var moveEntry in patch.MoveEntries)
                {
                    ValidateMoveEntry(moveEntry, index);
                }
            }

            // 验证TextOperations
            if (patch.TextOperations != null)
            {
                foreach (var textOp in patch.TextOperations)
                {
                    ValidateTextOperation(textOp, index);
                }
            }
        }

        /// <summary>
        /// 验证EditImage补丁
        /// </summary>
        private void ValidateEditImagePatch(EditImagePatch patch, int index)
        {
            if (string.IsNullOrEmpty(patch.FromFile))
            {
                AddValidationError($"EditImage补丁 {index} 的FromFile字段不能为空", ValidationSeverity.Error);
            }

            // 验证补丁模式
            if (!string.IsNullOrEmpty(patch.PatchMode))
            {
                var validModes = new[] { "Replace", "Overlay" };
                if (!validModes.Contains(patch.PatchMode))
                {
                    AddValidationError($"EditImage补丁 {index} 的PatchMode字段包含无效值: {patch.PatchMode}", ValidationSeverity.Error);
                }
            }

            // 验证区域
            ValidateArea(patch.FromArea, $"EditImage补丁 {index} 的FromArea", index);
            ValidateArea(patch.ToArea, $"EditImage补丁 {index} 的ToArea", index);
        }

        /// <summary>
        /// 验证EditMap补丁
        /// </summary>
        private void ValidateEditMapPatch(EditMapPatch patch, int index)
        {
            // EditMap补丁必须至少有一个操作
            var hasAnyOperation = !string.IsNullOrEmpty(patch.FromFile) || 
                                 patch.MapProperties != null || 
                                 patch.AddNpcWarps != null || 
                                 patch.AddWarps != null || 
                                 patch.MapTiles != null || 
                                 patch.TextOperations != null;

            if (!hasAnyOperation)
            {
                AddValidationError($"EditMap补丁 {index} 必须包含至少一个操作", ValidationSeverity.Error);
            }

            // 验证补丁模式
            if (!string.IsNullOrEmpty(patch.PatchMode))
            {
                var validModes = new[] { "Replace", "Overlay", "ReplaceByLayer" };
                if (!validModes.Contains(patch.PatchMode))
                {
                    AddValidationError($"EditMap补丁 {index} 的PatchMode字段包含无效值: {patch.PatchMode}", ValidationSeverity.Error);
                }
            }

            // 验证区域
            ValidateArea(patch.FromArea, $"EditMap补丁 {index} 的FromArea", index);
            ValidateArea(patch.ToArea, $"EditMap补丁 {index} 的ToArea", index);

            // 验证地图图块
            if (patch.MapTiles != null)
            {
                foreach (var tile in patch.MapTiles)
                {
                    ValidateMapTile(tile, index);
                }
            }
        }

        /// <summary>
        /// 验证Include补丁
        /// </summary>
        private void ValidateIncludePatch(IncludePatch patch, int index)
        {
            if (string.IsNullOrEmpty(patch.FromFile))
            {
                AddValidationError($"Include补丁 {index} 的FromFile字段不能为空", ValidationSeverity.Error);
            }
        }

        /// <summary>
        /// 验证移动条目
        /// </summary>
        private void ValidateMoveEntry(MoveEntry moveEntry, int patchIndex)
        {
            if (string.IsNullOrEmpty(moveEntry.ID))
            {
                AddValidationError($"补丁 {patchIndex} 的MoveEntry缺少ID字段", ValidationSeverity.Error);
            }

            var hasPosition = !string.IsNullOrEmpty(moveEntry.BeforeID) || 
                            !string.IsNullOrEmpty(moveEntry.AfterID) || 
                            !string.IsNullOrEmpty(moveEntry.ToPosition);

            if (!hasPosition)
            {
                AddValidationError($"补丁 {patchIndex} 的MoveEntry必须指定位置", ValidationSeverity.Error);
            }

            if (!string.IsNullOrEmpty(moveEntry.ToPosition))
            {
                var validPositions = new[] { "Top", "Bottom" };
                if (!validPositions.Contains(moveEntry.ToPosition))
                {
                    AddValidationError($"补丁 {patchIndex} 的MoveEntry的ToPosition字段包含无效值: {moveEntry.ToPosition}", ValidationSeverity.Error);
                }
            }
        }

        /// <summary>
        /// 验证文本操作
        /// </summary>
        private void ValidateTextOperation(TextOperation textOp, int patchIndex)
        {
            if (string.IsNullOrEmpty(textOp.Operation))
            {
                AddValidationError($"补丁 {patchIndex} 的TextOperation缺少Operation字段", ValidationSeverity.Error);
            }

            var validOperations = new[] { "Append", "Prepend", "Replace", "Remove" };
            if (!validOperations.Contains(textOp.Operation))
            {
                AddValidationError($"补丁 {patchIndex} 的TextOperation包含无效操作: {textOp.Operation}", ValidationSeverity.Error);
            }

            if (textOp.Target == null || !textOp.Target.Any())
            {
                AddValidationError($"补丁 {patchIndex} 的TextOperation缺少Target字段", ValidationSeverity.Error);
            }
        }

        /// <summary>
        /// 验证区域
        /// </summary>
        private void ValidateArea(Area? area, string fieldName, int patchIndex)
        {
            if (area == null) return;

            if (area.X < 0 || area.Y < 0 || area.Width <= 0 || area.Height <= 0)
            {
                AddValidationError($"{fieldName}包含无效的坐标或尺寸", ValidationSeverity.Error);
            }
        }

        /// <summary>
        /// 验证地图图块
        /// </summary>
        private void ValidateMapTile(MapTile tile, int patchIndex)
        {
            if (string.IsNullOrEmpty(tile.Layer))
            {
                AddValidationError($"补丁 {patchIndex} 的MapTile缺少Layer字段", ValidationSeverity.Error);
            }

            if (tile.Position == null)
            {
                AddValidationError($"补丁 {patchIndex} 的MapTile缺少Position字段", ValidationSeverity.Error);
            }
            else
            {
                ValidateArea(tile.Position, $"补丁 {patchIndex} 的MapTile的Position", patchIndex);
            }
        }

        /// <summary>
        /// 验证配置模式
        /// </summary>
        private void ValidateConfigSchema(Dictionary<string, ConfigSchemaField>? configSchema)
        {
            if (configSchema == null) return;

            foreach (var (key, field) in configSchema)
            {
                if (string.IsNullOrEmpty(key))
                {
                    AddValidationError("ConfigSchema中的键不能为空", ValidationSeverity.Error);
                }

                if (field == null)
                {
                    AddValidationError($"ConfigSchema字段 '{key}' 不能为空", ValidationSeverity.Error);
                }
            }
        }

        /// <summary>
        /// 验证自定义地点
        /// </summary>
        private void ValidateCustomLocations(List<CustomLocation>? customLocations)
        {
            if (customLocations == null) return;

            foreach (var location in customLocations)
            {
                if (string.IsNullOrEmpty(location.Name))
                {
                    AddValidationError("CustomLocation的Name字段不能为空", ValidationSeverity.Error);
                }

                if (string.IsNullOrEmpty(location.FromMapFile) && string.IsNullOrEmpty(location.FromMapData))
                {
                    AddValidationError($"CustomLocation '{location.Name}' 必须指定FromMapFile或FromMapData", ValidationSeverity.Error);
                }
            }
        }

        /// <summary>
        /// 验证动态令牌
        /// </summary>
        private void ValidateDynamicTokens(List<DynamicToken>? dynamicTokens)
        {
            if (dynamicTokens == null) return;

            foreach (var token in dynamicTokens)
            {
                if (string.IsNullOrEmpty(token.Name))
                {
                    AddValidationError("DynamicToken的Name字段不能为空", ValidationSeverity.Error);
                }

                if (string.IsNullOrEmpty(token.Value))
                {
                    AddValidationError($"DynamicToken '{token.Name}' 的Value字段不能为空", ValidationSeverity.Error);
                }
            }
        }

        /// <summary>
        /// 添加验证错误
        /// </summary>
        private void AddValidationError(string message, ValidationSeverity severity)
        {
            _validationResults.Add(new ValidationError
            {
                ErrorMessage = message,
                Severity = severity
            });
        }
    }

    /// <summary>
    /// 验证错误信息
    /// </summary>
    public class ValidationError
    {
        public string ErrorMessage { get; set; } = string.Empty;
        public ValidationSeverity Severity { get; set; }
    }

    /// <summary>
    /// 验证结果
    /// </summary>
    public class ValidationResult
    {
        public ValidationResult()
        {
        }

        public ValidationResult(bool isValid)
        {
            IsValid = isValid;
        }

        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }

    /// <summary>
    /// 验证严重程度
    /// </summary>
    public enum ValidationSeverity
    {
        Error,
        Warning
    }
}

