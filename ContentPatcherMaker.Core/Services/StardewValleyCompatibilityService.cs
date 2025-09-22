using System;
using System.Collections.Generic;
using System.Linq;
using ContentPatcherMaker.Core.Models;
using ContentPatcherMaker.Core.Services.Logging;

namespace ContentPatcherMaker.Core.Services
{
    /// <summary>
    /// Stardew Valley兼容性验证服务
    /// </summary>
    public class StardewValleyCompatibilityService
    {
        private readonly LoggingService _loggingService;
        private readonly Dictionary<string, AssetInfo> _knownAssets;

        public StardewValleyCompatibilityService(LoggingService loggingService)
        {
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            _knownAssets = InitializeKnownAssets();
        }

        /// <summary>
        /// 验证内容包与Stardew Valley的兼容性
        /// </summary>
        /// <param name="contentPack">内容包</param>
        /// <returns>兼容性验证结果</returns>
        public CompatibilityResult ValidateCompatibility(ContentPack contentPack)
        {
            if (contentPack == null)
                throw new ArgumentNullException(nameof(contentPack));

            var result = new CompatibilityResult();

            try
            {
                _loggingService.LogInformation("开始验证Stardew Valley兼容性", "CompatibilityService");

                // 验证格式版本兼容性
                ValidateFormatCompatibility(contentPack.Format, result);

                // 验证补丁兼容性
                ValidatePatchesCompatibility(contentPack.Changes, result);

                // 验证自定义地点兼容性
                ValidateCustomLocationsCompatibility(contentPack.CustomLocations, result);

                // 验证动态令牌兼容性
                ValidateDynamicTokensCompatibility(contentPack.DynamicTokens, result);

                result.IsCompatible = !result.Errors.Any();
                
                _loggingService.LogInformation($"Stardew Valley兼容性验证完成: {(result.IsCompatible ? "兼容" : "不兼容")}", "CompatibilityService");
            }
            catch (Exception ex)
            {
                _loggingService.LogError($"验证Stardew Valley兼容性时发生异常: {ex.Message}", ex, "CompatibilityService");
                result.Errors.Add($"验证过程中发生异常: {ex.Message}");
                result.IsCompatible = false;
            }

            return result;
        }

        /// <summary>
        /// 验证格式版本兼容性
        /// </summary>
        private void ValidateFormatCompatibility(string format, CompatibilityResult result)
        {
            if (string.IsNullOrEmpty(format))
            {
                result.Errors.Add("Format字段不能为空");
                return;
            }

            // 检查格式版本是否受支持
            var supportedVersions = new[]
            {
                "1.0.0", "1.1.0", "1.2.0", "1.3.0", "1.4.0", "1.5.0", "1.6.0", "1.7.0", "1.8.0", "1.9.0",
                "1.10.0", "1.11.0", "1.12.0", "1.13.0", "1.14.0", "1.15.0", "1.16.0", "1.17.0", "1.18.0", "1.19.0",
                "1.20.0", "1.21.0", "1.22.0", "1.23.0", "1.24.0", "1.25.0", "2.0.0", "2.1.0", "2.2.0", "2.3.0",
                "2.4.0", "2.5.0", "2.6.0", "2.7.0", "2.8.0"
            };

            if (!supportedVersions.Contains(format))
            {
                result.Warnings.Add($"格式版本 '{format}' 可能不受支持，建议使用最新版本");
            }

            // 检查版本是否过旧
            try
            {
                var version = new Version(format);
                var minRecommendedVersion = new Version("2.0.0");
                if (version < minRecommendedVersion)
                {
                    result.Warnings.Add($"格式版本 '{format}' 过旧，建议升级到2.0.0或更高版本");
                }
            }
            catch
            {
                result.Errors.Add($"格式版本 '{format}' 格式无效");
            }
        }

        /// <summary>
        /// 验证补丁兼容性
        /// </summary>
        private void ValidatePatchesCompatibility(List<Patch> patches, CompatibilityResult result)
        {
            if (patches == null || !patches.Any())
            {
                result.Errors.Add("Changes字段不能为空");
                return;
            }

            for (int i = 0; i < patches.Count; i++)
            {
                var patch = patches[i];
                ValidatePatchCompatibility(patch, i, result);
            }
        }

        /// <summary>
        /// 验证单个补丁兼容性
        /// </summary>
        private void ValidatePatchCompatibility(Patch patch, int index, CompatibilityResult result)
        {
            if (patch == null)
            {
                result.Errors.Add($"补丁 {index} 不能为空");
                return;
            }

            // 验证目标素材是否存在
            ValidateTargetAsset(patch.Target, index, result);

            // 根据操作类型验证特定兼容性
            switch (patch.Action)
            {
                case "Load":
                    ValidateLoadPatchCompatibility((LoadPatch)patch, index, result);
                    break;
                case "EditData":
                    ValidateEditDataPatchCompatibility((EditDataPatch)patch, index, result);
                    break;
                case "EditImage":
                    ValidateEditImagePatchCompatibility((EditImagePatch)patch, index, result);
                    break;
                case "EditMap":
                    ValidateEditMapPatchCompatibility((EditMapPatch)patch, index, result);
                    break;
                case "Include":
                    ValidateIncludePatchCompatibility((IncludePatch)patch, index, result);
                    break;
            }
        }

        /// <summary>
        /// 验证目标素材
        /// </summary>
        private void ValidateTargetAsset(string target, int patchIndex, CompatibilityResult result)
        {
            if (string.IsNullOrEmpty(target))
            {
                result.Errors.Add($"补丁 {patchIndex} 的Target字段不能为空");
                return;
            }

            // 检查目标素材是否已知
            if (!_knownAssets.ContainsKey(target))
            {
                result.Warnings.Add($"补丁 {patchIndex} 的目标素材 '{target}' 不在已知素材列表中，可能无效");
            }
            else
            {
                var assetInfo = _knownAssets[target];
                if (assetInfo.IsDeprecated)
                {
                    result.Warnings.Add($"补丁 {patchIndex} 的目标素材 '{target}' 已弃用: {assetInfo.DeprecationReason}");
                }
            }
        }

        /// <summary>
        /// 验证Load补丁兼容性
        /// </summary>
        private void ValidateLoadPatchCompatibility(LoadPatch patch, int index, CompatibilityResult result)
        {
            if (string.IsNullOrEmpty(patch.FromFile))
            {
                result.Errors.Add($"Load补丁 {index} 的FromFile字段不能为空");
            }

            // 检查文件扩展名是否支持
            if (!string.IsNullOrEmpty(patch.FromFile))
            {
                var supportedExtensions = new[] { ".png", ".json", ".tbin", ".tmx", ".xnb" };
                var extension = System.IO.Path.GetExtension(patch.FromFile).ToLower();
                if (!supportedExtensions.Contains(extension))
                {
                    result.Warnings.Add($"Load补丁 {index} 的FromFile文件扩展名 '{extension}' 可能不受支持");
                }
            }
        }

        /// <summary>
        /// 验证EditData补丁兼容性
        /// </summary>
        private void ValidateEditDataPatchCompatibility(EditDataPatch patch, int index, CompatibilityResult result)
        {
            // 检查是否有任何操作
            var hasAnyOperation = patch.Fields != null || patch.Entries != null || 
                                 patch.MoveEntries != null || patch.TextOperations != null;

            if (!hasAnyOperation)
            {
                result.Errors.Add($"EditData补丁 {index} 必须包含至少一个操作");
            }

            // 验证MoveEntries
            if (patch.MoveEntries != null)
            {
                foreach (var moveEntry in patch.MoveEntries)
                {
                    if (string.IsNullOrEmpty(moveEntry.ID))
                    {
                        result.Errors.Add($"EditData补丁 {index} 的MoveEntry缺少ID字段");
                    }
                }
            }
        }

        /// <summary>
        /// 验证EditImage补丁兼容性
        /// </summary>
        private void ValidateEditImagePatchCompatibility(EditImagePatch patch, int index, CompatibilityResult result)
        {
            if (string.IsNullOrEmpty(patch.FromFile))
            {
                result.Errors.Add($"EditImage补丁 {index} 的FromFile字段不能为空");
            }

            // 验证补丁模式
            if (!string.IsNullOrEmpty(patch.PatchMode))
            {
                var validModes = new[] { "Replace", "Overlay" };
                if (!validModes.Contains(patch.PatchMode))
                {
                    result.Errors.Add($"EditImage补丁 {index} 的PatchMode字段包含无效值: {patch.PatchMode}");
                }
            }

            // 验证区域坐标
            ValidateAreaCompatibility(patch.FromArea, $"EditImage补丁 {index} 的FromArea", index, result);
            ValidateAreaCompatibility(patch.ToArea, $"EditImage补丁 {index} 的ToArea", index, result);
        }

        /// <summary>
        /// 验证EditMap补丁兼容性
        /// </summary>
        private void ValidateEditMapPatchCompatibility(EditMapPatch patch, int index, CompatibilityResult result)
        {
            // 检查是否有任何操作
            var hasAnyOperation = !string.IsNullOrEmpty(patch.FromFile) || 
                                 patch.MapProperties != null || 
                                 patch.AddNpcWarps != null || 
                                 patch.AddWarps != null || 
                                 patch.MapTiles != null || 
                                 patch.TextOperations != null;

            if (!hasAnyOperation)
            {
                result.Errors.Add($"EditMap补丁 {index} 必须包含至少一个操作");
            }

            // 验证补丁模式
            if (!string.IsNullOrEmpty(patch.PatchMode))
            {
                var validModes = new[] { "Replace", "Overlay", "ReplaceByLayer" };
                if (!validModes.Contains(patch.PatchMode))
                {
                    result.Errors.Add($"EditMap补丁 {index} 的PatchMode字段包含无效值: {patch.PatchMode}");
                }
            }

            // 验证区域坐标
            ValidateAreaCompatibility(patch.FromArea, $"EditMap补丁 {index} 的FromArea", index, result);
            ValidateAreaCompatibility(patch.ToArea, $"EditMap补丁 {index} 的ToArea", index, result);
        }

        /// <summary>
        /// 验证Include补丁兼容性
        /// </summary>
        private void ValidateIncludePatchCompatibility(IncludePatch patch, int index, CompatibilityResult result)
        {
            if (string.IsNullOrEmpty(patch.FromFile))
            {
                result.Errors.Add($"Include补丁 {index} 的FromFile字段不能为空");
            }

            // 检查文件扩展名
            if (!string.IsNullOrEmpty(patch.FromFile))
            {
                var extension = System.IO.Path.GetExtension(patch.FromFile).ToLower();
                if (extension != ".json")
                {
                    result.Warnings.Add($"Include补丁 {index} 的FromFile应该是.json文件");
                }
            }
        }

        /// <summary>
        /// 验证区域兼容性
        /// </summary>
        private void ValidateAreaCompatibility(Area? area, string fieldName, int patchIndex, CompatibilityResult result)
        {
            if (area == null) return;

            if (area.X < 0 || area.Y < 0 || area.Width <= 0 || area.Height <= 0)
            {
                result.Errors.Add($"{fieldName}包含无效的坐标或尺寸");
            }

            // 检查坐标是否过大
            if (area.X > 10000 || area.Y > 10000 || area.Width > 10000 || area.Height > 10000)
            {
                result.Warnings.Add($"{fieldName}的坐标或尺寸过大，可能导致性能问题");
            }
        }

        /// <summary>
        /// 验证自定义地点兼容性
        /// </summary>
        private void ValidateCustomLocationsCompatibility(List<CustomLocation>? customLocations, CompatibilityResult result)
        {
            if (customLocations == null) return;

            foreach (var location in customLocations)
            {
                if (string.IsNullOrEmpty(location.Name))
                {
                    result.Errors.Add("CustomLocation的Name字段不能为空");
                }

                if (string.IsNullOrEmpty(location.FromMapFile) && string.IsNullOrEmpty(location.FromMapData))
                {
                    result.Errors.Add($"CustomLocation '{location.Name}' 必须指定FromMapFile或FromMapData");
                }

                // 检查传送点配置
                ValidateWarpConfiguration(location, result);
            }
        }

        /// <summary>
        /// 验证传送点配置
        /// </summary>
        private void ValidateWarpConfiguration(CustomLocation location, CompatibilityResult result)
        {
            // 检查传送目标配置
            if (!string.IsNullOrEmpty(location.WarpTo))
            {
                if (location.WarpToX == null || location.WarpToY == null)
                {
                    result.Warnings.Add($"CustomLocation '{location.Name}' 指定了WarpTo但缺少坐标");
                }
            }

            // 检查传送源配置
            if (!string.IsNullOrEmpty(location.WarpFrom))
            {
                if (location.WarpFromX == null || location.WarpFromY == null)
                {
                    result.Warnings.Add($"CustomLocation '{location.Name}' 指定了WarpFrom但缺少坐标");
                }
            }
        }

        /// <summary>
        /// 验证动态令牌兼容性
        /// </summary>
        private void ValidateDynamicTokensCompatibility(List<DynamicToken>? dynamicTokens, CompatibilityResult result)
        {
            if (dynamicTokens == null) return;

            foreach (var token in dynamicTokens)
            {
                if (string.IsNullOrEmpty(token.Name))
                {
                    result.Errors.Add("DynamicToken的Name字段不能为空");
                }

                if (string.IsNullOrEmpty(token.Value))
                {
                    result.Errors.Add($"DynamicToken '{token.Name}' 的Value字段不能为空");
                }

                // 检查令牌名称格式
                if (!string.IsNullOrEmpty(token.Name) && !IsValidTokenName(token.Name))
                {
                    result.Warnings.Add($"DynamicToken名称 '{token.Name}' 格式可能无效");
                }
            }
        }

        /// <summary>
        /// 检查令牌名称是否有效
        /// </summary>
        private bool IsValidTokenName(string name)
        {
            // 令牌名称应该只包含字母、数字、下划线和连字符
            var pattern = @"^[a-zA-Z0-9_-]+$";
            return System.Text.RegularExpressions.Regex.IsMatch(name, pattern);
        }

        /// <summary>
        /// 初始化已知素材信息
        /// </summary>
        private Dictionary<string, AssetInfo> InitializeKnownAssets()
        {
            return new Dictionary<string, AssetInfo>
            {
                // 角色相关
                ["Characters/Dialogue/Abigail"] = new AssetInfo { Type = AssetType.Dialogue },
                ["Characters/Dialogue/Penny"] = new AssetInfo { Type = AssetType.Dialogue },
                ["Characters/Dialogue/Sam"] = new AssetInfo { Type = AssetType.Dialogue },
                ["Characters/Dialogue/Sebastian"] = new AssetInfo { Type = AssetType.Dialogue },
                ["Characters/Dialogue/Shane"] = new AssetInfo { Type = AssetType.Dialogue },
                ["Characters/Dialogue/Emily"] = new AssetInfo { Type = AssetType.Dialogue },
                ["Characters/Dialogue/Haley"] = new AssetInfo { Type = AssetType.Dialogue },
                ["Characters/Dialogue/Leah"] = new AssetInfo { Type = AssetType.Dialogue },
                ["Characters/Dialogue/Maru"] = new AssetInfo { Type = AssetType.Dialogue },
                ["Characters/Dialogue/Alex"] = new AssetInfo { Type = AssetType.Dialogue },
                ["Characters/Dialogue/Elliott"] = new AssetInfo { Type = AssetType.Dialogue },
                ["Characters/Dialogue/Harvey"] = new AssetInfo { Type = AssetType.Dialogue },

                // 肖像
                ["Portraits/Abigail"] = new AssetInfo { Type = AssetType.Image },
                ["Portraits/Penny"] = new AssetInfo { Type = AssetType.Image },
                ["Portraits/Sam"] = new AssetInfo { Type = AssetType.Image },
                ["Portraits/Sebastian"] = new AssetInfo { Type = AssetType.Image },
                ["Portraits/Shane"] = new AssetInfo { Type = AssetType.Image },
                ["Portraits/Emily"] = new AssetInfo { Type = AssetType.Image },
                ["Portraits/Haley"] = new AssetInfo { Type = AssetType.Image },
                ["Portraits/Leah"] = new AssetInfo { Type = AssetType.Image },
                ["Portraits/Maru"] = new AssetInfo { Type = AssetType.Image },
                ["Portraits/Alex"] = new AssetInfo { Type = AssetType.Image },
                ["Portraits/Elliott"] = new AssetInfo { Type = AssetType.Image },
                ["Portraits/Harvey"] = new AssetInfo { Type = AssetType.Image },

                // 地图
                ["Maps/Town"] = new AssetInfo { Type = AssetType.Map },
                ["Maps/Farm"] = new AssetInfo { Type = AssetType.Map },
                ["Maps/Mountain"] = new AssetInfo { Type = AssetType.Map },
                ["Maps/Beach"] = new AssetInfo { Type = AssetType.Map },
                ["Maps/Forest"] = new AssetInfo { Type = AssetType.Map },
                ["Maps/Desert"] = new AssetInfo { Type = AssetType.Map },
                ["Maps/Island"] = new AssetInfo { Type = AssetType.Map },

                // 数据
                ["Data/Objects"] = new AssetInfo { Type = AssetType.Data },
                ["Data/Crops"] = new AssetInfo { Type = AssetType.Data },
                ["Data/FruitTrees"] = new AssetInfo { Type = AssetType.Data },
                ["Data/Achievements"] = new AssetInfo { Type = AssetType.Data },
                ["Data/NPCGiftTastes"] = new AssetInfo { Type = AssetType.Data },
                ["Data/Events"] = new AssetInfo { Type = AssetType.Data },

                // 精灵图
                ["Maps/springobjects"] = new AssetInfo { Type = AssetType.Image },
                ["LooseSprites/Cursors"] = new AssetInfo { Type = AssetType.Image },
                ["Characters/Abigail"] = new AssetInfo { Type = AssetType.Image },
                ["Characters/Penny"] = new AssetInfo { Type = AssetType.Image },
                ["Characters/Sam"] = new AssetInfo { Type = AssetType.Image },
                ["Characters/Sebastian"] = new AssetInfo { Type = AssetType.Image },
                ["Characters/Shane"] = new AssetInfo { Type = AssetType.Image },
                ["Characters/Emily"] = new AssetInfo { Type = AssetType.Image },
                ["Characters/Haley"] = new AssetInfo { Type = AssetType.Image },
                ["Characters/Leah"] = new AssetInfo { Type = AssetType.Image },
                ["Characters/Maru"] = new AssetInfo { Type = AssetType.Image },
                ["Characters/Alex"] = new AssetInfo { Type = AssetType.Image },
                ["Characters/Elliott"] = new AssetInfo { Type = AssetType.Image },
                ["Characters/Harvey"] = new AssetInfo { Type = AssetType.Image }
            };
        }
    }

    /// <summary>
    /// 兼容性验证结果
    /// </summary>
    public class CompatibilityResult
    {
        public bool IsCompatible { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();

        public override string ToString()
        {
            var result = $"兼容性验证结果: {(IsCompatible ? "兼容" : "不兼容")}";
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

    /// <summary>
    /// 素材信息
    /// </summary>
    public class AssetInfo
    {
        public AssetType Type { get; set; }
        public bool IsDeprecated { get; set; }
        public string? DeprecationReason { get; set; }
    }

    /// <summary>
    /// 素材类型
    /// </summary>
    public enum AssetType
    {
        Image,
        Map,
        Data,
        Dialogue,
        Audio
    }
}

