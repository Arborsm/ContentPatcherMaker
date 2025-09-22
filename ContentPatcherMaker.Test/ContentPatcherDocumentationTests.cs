using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using ContentPatcherMaker.Core.Models;
using ContentPatcherMaker.Core.Services;
using ContentPatcherMaker.Core.Validation;
using ContentPatcherMaker.Core.Extensions;
using ValidationResult = ContentPatcherMaker.Core.Validation.ValidationResult;

namespace ContentPatcherMaker.Test
{
    /// <summary>
    /// 基于md文档内容的ContentPatcher测试
    /// </summary>
    public class ContentPatcherDocumentationTests
    {
        private readonly ContentPatcherService _service;

        public ContentPatcherDocumentationTests()
        {
            _service = new ContentPatcherService();
        }

        #region Load Action Tests (基于 action-load.md)

        /// <summary>
        /// 测试Load操作的基本功能 - 替换阿比盖尔肖像
        /// 基于 action-load.md 中的示例
        /// </summary>
        [Fact]
        public void LoadAction_ReplaceAbigailPortrait_ShouldWork()
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            var loadPatch = new LoadPatch
            {
                Target = "Portraits/Abigail",
                FromFile = "assets/abigail.png"
            };

            // Act
            _service.AddPatch(contentPack, loadPatch);
            var validationResult = _service.ValidateContentPack(contentPack);
            var json = _service.GenerateJson(contentPack);

            // Assert
            Assert.True(validationResult.IsValid);
            Assert.Contains("Portraits/Abigail", json);
            Assert.Contains("assets/abigail.png", json);
            Assert.Contains("\"Action\": \"Load\"", json);
        }

        /// <summary>
        /// 测试Load操作的多个目标 - 同时替换多个肖像
        /// 基于 action-load.md 中的示例
        /// </summary>
        [Fact]
        public void LoadAction_MultipleTargets_ShouldWork()
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            var loadPatch1 = new LoadPatch
            {
                Target = "Portraits/Abigail",
                FromFile = "assets/abigail.png"
            };
            var loadPatch2 = new LoadPatch
            {
                Target = "Portraits/Penny",
                FromFile = "assets/penny.png"
            };

            // Act
            _service.AddPatch(contentPack, loadPatch1);
            _service.AddPatch(contentPack, loadPatch2);
            var validationResult = _service.ValidateContentPack(contentPack);
            var json = _service.GenerateJson(contentPack);

            // Assert
            Assert.True(validationResult.IsValid);
            Assert.Contains("Portraits/Abigail", json);
            Assert.Contains("Portraits/Penny", json);
            Assert.Contains("assets/abigail.png", json);
            Assert.Contains("assets/penny.png", json);
        }

        /// <summary>
        /// 测试Load操作的优先级设置
        /// 基于 action-load.md 中的示例
        /// </summary>
        [Fact]
        public void LoadAction_WithPriority_ShouldWork()
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            var loadPatch = new LoadPatch
            {
                Target = "Data/Events/AdventureGuild",
                FromFile = "assets/empty-event-file.json",
                Priority = "Low"
            };

            // Act
            _service.AddPatch(contentPack, loadPatch);
            var validationResult = _service.ValidateContentPack(contentPack);
            var json = _service.GenerateJson(contentPack);

            // Assert
            Assert.True(validationResult.IsValid);
            Assert.Contains("\"Priority\": \"Low\"", json);
        }

        /// <summary>
        /// 测试Load操作的必填字段验证
        /// </summary>
        [Fact]
        public void LoadAction_MissingRequiredFields_ShouldFailValidation()
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            var invalidPatch = new LoadPatch
            {
                // Missing Target
                FromFile = "assets/test.png"
            };

            // Act
            _service.AddPatch(contentPack, invalidPatch);
            var validationResult = _service.ValidateContentPack(contentPack);

            // Assert
            Assert.False(validationResult.IsValid);
            Assert.Contains("Target字段不能为空", validationResult.Errors);
        }

        #endregion

        #region EditData Action Tests (基于 action-editdata.md)

        /// <summary>
        /// 测试EditData操作的基本功能
        /// </summary>
        [Fact]
        public void EditDataAction_BasicEdit_ShouldWork()
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            var editDataPatch = new EditDataPatch
            {
                Target = "Data/Objects",
                Entries = new Dictionary<string, object>
                {
                    ["MossSoup"] = new Dictionary<string, object>
                    {
                        ["Price"] = 80,
                        ["Description"] = "Maybe a pufferchick would like this."
                    }
                }
            };

            // Act
            _service.AddPatch(contentPack, editDataPatch);
            var validationResult = _service.ValidateContentPack(contentPack);
            var json = _service.GenerateJson(contentPack);

            // Assert
            Assert.True(validationResult.IsValid);
            Assert.Contains("\"Action\": \"EditData\"", json);
            Assert.Contains("Data/Objects", json);
        }

        /// <summary>
        /// 测试EditData操作的Fields功能
        /// </summary>
        [Fact]
        public void EditDataAction_WithFields_ShouldWork()
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            var editDataPatch = new EditDataPatch
            {
                Target = "Data/Objects",
                Fields = new Dictionary<string, object>
                {
                    ["MossSoup"] = new Dictionary<string, object>
                    {
                        ["Description"] = "A delicious soup made from moss."
                    }
                }
            };

            // Act
            _service.AddPatch(contentPack, editDataPatch);
            var validationResult = _service.ValidateContentPack(contentPack);

            // Assert
            Assert.True(validationResult.IsValid);
        }

        /// <summary>
        /// 测试EditData操作的TextOperations功能
        /// </summary>
        [Fact]
        public void EditDataAction_WithTextOperations_ShouldWork()
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            var editDataPatch = new EditDataPatch
            {
                Target = "Data/Objects",
                TextOperations = new List<TextOperation>
                {
                    new TextOperation
                    {
                        Operation = "Append",
                        Target = new List<string> { "Entries", "Universal_Love" },
                        Value = "127",
                        Delimiter = " "
                    }
                }
            };

            // Act
            _service.AddPatch(contentPack, editDataPatch);
            var validationResult = _service.ValidateContentPack(contentPack);

            // Assert
            Assert.True(validationResult.IsValid);
        }

        #endregion

        #region EditImage Action Tests (基于 action-editimage.md)

        /// <summary>
        /// 测试EditImage操作的基本功能
        /// </summary>
        [Fact]
        public void EditImageAction_BasicEdit_ShouldWork()
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            var editImagePatch = new EditImagePatch
            {
                Target = "Maps/springobjects",
                FromFile = "assets/fish-object.png",
                FromArea = new Area { X = 0, Y = 0, Width = 16, Height = 16 },
                ToArea = new Area { X = 256, Y = 96, Width = 16, Height = 16 },
                PatchMode = "Replace"
            };

            // Act
            _service.AddPatch(contentPack, editImagePatch);
            var validationResult = _service.ValidateContentPack(contentPack);

            // Assert
            Assert.True(validationResult.IsValid);
        }

        /// <summary>
        /// 测试EditImage操作的Overlay模式
        /// </summary>
        [Fact]
        public void EditImageAction_OverlayMode_ShouldWork()
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            var editImagePatch = new EditImagePatch
            {
                Target = "Maps/springobjects",
                FromFile = "assets/overlay.png",
                PatchMode = "Overlay"
            };

            // Act
            _service.AddPatch(contentPack, editImagePatch);
            var validationResult = _service.ValidateContentPack(contentPack);

            // Assert
            Assert.True(validationResult.IsValid);
        }

        #endregion

        #region EditMap Action Tests (基于 action-editmap.md)

        /// <summary>
        /// 测试EditMap操作的基本功能
        /// </summary>
        [Fact]
        public void EditMapAction_BasicEdit_ShouldWork()
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            var editMapPatch = new EditMapPatch
            {
                Target = "Maps/Town",
                FromFile = "assets/town.tmx",
                FromArea = new Area { X = 22, Y = 61, Width = 16, Height = 13 },
                ToArea = new Area { X = 22, Y = 61, Width = 16, Height = 13 },
                PatchMode = "Replace"
            };

            // Act
            _service.AddPatch(contentPack, editMapPatch);
            var validationResult = _service.ValidateContentPack(contentPack);

            // Assert
            Assert.True(validationResult.IsValid);
        }

        /// <summary>
        /// 测试EditMap操作的地图属性修改
        /// </summary>
        [Fact]
        public void EditMapAction_WithMapProperties_ShouldWork()
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            var editMapPatch = new EditMapPatch
            {
                Target = "Maps/Town",
                MapProperties = new Dictionary<string, string>
                {
                    ["Outdoors"] = "T"
                }
            };

            // Act
            _service.AddPatch(contentPack, editMapPatch);
            var validationResult = _service.ValidateContentPack(contentPack);

            // Assert
            Assert.True(validationResult.IsValid);
        }

        /// <summary>
        /// 测试EditMap操作的地图图块修改
        /// </summary>
        [Fact]
        public void EditMapAction_WithMapTiles_ShouldWork()
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            var editMapPatch = new EditMapPatch
            {
                Target = "Maps/Town",
                MapTiles = new List<MapTile>
                {
                    new MapTile
                    {
                        Layer = "Back",
                        Position = new Area { X = 72, Y = 15 },
                        SetIndex = "622"
                    }
                }
            };

            // Act
            _service.AddPatch(contentPack, editMapPatch);
            var validationResult = _service.ValidateContentPack(contentPack);

            // Assert
            Assert.True(validationResult.IsValid);
        }

        #endregion

        #region Include Action Tests (基于 action-include.md)

        /// <summary>
        /// 测试Include操作的基本功能
        /// </summary>
        [Fact]
        public void IncludeAction_BasicInclude_ShouldWork()
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            var includePatch = new IncludePatch
            {
                FromFile = "assets/dialogue-changes.json"
            };

            // Act
            _service.AddPatch(contentPack, includePatch);
            var validationResult = _service.ValidateContentPack(contentPack);

            // Assert
            Assert.True(validationResult.IsValid);
        }

        #endregion

        #region Format Version Tests (基于 author-guide.md)

        /// <summary>
        /// 测试不同格式版本的兼容性
        /// </summary>
        [Theory]
        [InlineData("2.8.0")]
        [InlineData("2.7.0")]
        [InlineData("2.6.0")]
        [InlineData("2.5.0")]
        public void FormatVersion_VariousVersions_ShouldWork(string formatVersion)
        {
            // Arrange
            var contentPack = _service.CreateContentPack(formatVersion);
            var loadPatch = new LoadPatch
            {
                Target = "Test/Target",
                FromFile = "test.png"
            };

            // Act
            _service.AddPatch(contentPack, loadPatch);
            var validationResult = _service.ValidateContentPack(contentPack);

            // Assert
            Assert.True(validationResult.IsValid);
            Assert.Equal(formatVersion, contentPack.Format);
        }

        /// <summary>
        /// 测试无效格式版本的验证
        /// </summary>
        [Fact]
        public void FormatVersion_InvalidVersion_ShouldFailValidation()
        {
            // Arrange
            var contentPack = _service.CreateContentPack("invalid-version");
            var loadPatch = new LoadPatch
            {
                Target = "Test/Target",
                FromFile = "test.png"
            };

            // Act
            _service.AddPatch(contentPack, loadPatch);
            var validationResult = _service.ValidateContentPack(contentPack);

            // Assert
            Assert.False(validationResult.IsValid);
            Assert.Contains("Format版本格式无效", validationResult.Errors.First());
        }

        #endregion

        #region Priority Tests (基于 author-guide.md)

        /// <summary>
        /// 测试各种优先级设置
        /// </summary>
        [Theory]
        [InlineData("Low")]
        [InlineData("Medium")]
        [InlineData("High")]
        [InlineData("Exclusive")]
        public void Priority_VariousPriorities_ShouldWork(string priority)
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            var loadPatch = new LoadPatch
            {
                Target = "Test/Target",
                FromFile = "test.png",
                Priority = priority
            };

            // Act
            _service.AddPatch(contentPack, loadPatch);
            var validationResult = _service.ValidateContentPack(contentPack);

            // Assert
            Assert.True(validationResult.IsValid);
        }

        /// <summary>
        /// 测试无效优先级的验证
        /// </summary>
        [Fact]
        public void Priority_InvalidPriority_ShouldFailValidation()
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            var loadPatch = new LoadPatch
            {
                Target = "Test/Target",
                FromFile = "test.png",
                Priority = "InvalidPriority"
            };

            // Act
            _service.AddPatch(contentPack, loadPatch);
            var validationResult = _service.ValidateContentPack(contentPack);

            // Assert
            Assert.False(validationResult.IsValid);
            Assert.Contains("Priority字段包含无效值", validationResult.Errors.First());
        }

        #endregion

        #region Update Frequency Tests (基于 author-guide.md)

        /// <summary>
        /// 测试更新频率设置
        /// </summary>
        [Theory]
        [InlineData("OnDayStart")]
        [InlineData("OnLocationChange")]
        [InlineData("OnTimeChange")]
        [InlineData("OnDayStart,OnLocationChange")]
        public void UpdateFrequency_VariousFrequencies_ShouldWork(string updateFrequency)
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            var loadPatch = new LoadPatch
            {
                Target = "Test/Target",
                FromFile = "test.png",
                Update = updateFrequency
            };

            // Act
            _service.AddPatch(contentPack, loadPatch);
            var validationResult = _service.ValidateContentPack(contentPack);

            // Assert
            Assert.True(validationResult.IsValid);
        }

        #endregion

        #region Custom Locations Tests (基于 custom-locations.md)

        /// <summary>
        /// 测试自定义地点功能
        /// </summary>
        [Fact]
        public void CustomLocations_BasicCustomLocation_ShouldWork()
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            contentPack.CustomLocations = new List<CustomLocation>
            {
                new CustomLocation
                {
                    Name = "CustomFarm",
                    FromMapFile = "assets/custom-farm.tmx",
                    WarpTo = "Farm",
                    WarpToX = 10,
                    WarpToY = 10
                }
            };

            // Act
            var validationResult = _service.ValidateContentPack(contentPack);

            // Assert
            Assert.True(validationResult.IsValid);
        }

        #endregion

        #region Config Schema Tests (基于 config.md)

        /// <summary>
        /// 测试配置模式功能
        /// </summary>
        [Fact]
        public void ConfigSchema_BasicConfigSchema_ShouldWork()
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            contentPack.ConfigSchema = new Dictionary<string, ConfigSchemaField>
            {
                ["EnableFeature"] = new ConfigSchemaField
                {
                    AllowValues = "true, false",
                    Default = true,
                    Description = "Enable or disable the feature"
                }
            };

            // Act
            var validationResult = _service.ValidateContentPack(contentPack);

            // Assert
            Assert.True(validationResult.IsValid);
        }

        #endregion

        #region Dynamic Tokens Tests (基于 tokens.md)

        /// <summary>
        /// 测试动态令牌功能
        /// </summary>
        [Fact]
        public void DynamicTokens_BasicDynamicToken_ShouldWork()
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            contentPack.DynamicTokens = new List<DynamicToken>
            {
                new DynamicToken
                {
                    Name = "CustomToken",
                    Value = "CustomValue",
                    When = new Dictionary<string, string>
                    {
                        ["Season"] = "spring"
                    }
                }
            };

            // Act
            var validationResult = _service.ValidateContentPack(contentPack);

            // Assert
            Assert.True(validationResult.IsValid);
        }

        #endregion

        #region JSON Generation Tests

        /// <summary>
        /// 测试JSON生成的完整性
        /// </summary>
        [Fact]
        public void JsonGeneration_CompleteContentPack_ShouldGenerateValidJson()
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            
            // 添加各种类型的补丁
            _service.AddPatch(contentPack, new LoadPatch
            {
                Target = "Portraits/Abigail",
                FromFile = "assets/abigail.png",
                LogName = "替换阿比盖尔肖像"
            });

            _service.AddPatch(contentPack, new EditDataPatch
            {
                Target = "Data/Objects",
                Entries = new Dictionary<string, object>
                {
                    ["TestItem"] = new Dictionary<string, object>
                    {
                        ["Price"] = 100,
                        ["Description"] = "Test item"
                    }
                }
            });

            // Act
            var json = _service.GenerateJson(contentPack);
            var validationResult = _service.ValidateContentPack(contentPack);

            // Assert
            Assert.True(validationResult.IsValid);
            Assert.Contains("\"Format\": \"2.8.0\"", json);
            Assert.Contains("\"Changes\":", json);
            Assert.Contains("\"Action\": \"Load\"", json);
            Assert.Contains("\"Action\": \"EditData\"", json);
        }

        #endregion

        #region Compatibility Tests

        /// <summary>
        /// 测试Stardew Valley兼容性检查
        /// </summary>
        [Fact]
        public void Compatibility_ValidContentPack_ShouldPassCompatibilityCheck()
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            var loadPatch = new LoadPatch
            {
                Target = "Portraits/Abigail",
                FromFile = "assets/abigail.png"
            };

            // Act
            _service.AddPatch(contentPack, loadPatch);
            var compatibilityResult = _service.CheckCompatibility(contentPack);

            // Assert
            Assert.True(compatibilityResult.IsCompatible);
        }

        #endregion

        #region Extension API Tests (基于 extensibility.md)

        /// <summary>
        /// 测试扩展API的基本功能
        /// </summary>
        [Fact]
        public void ExtensionApi_RegisterTokenProvider_ShouldWork()
        {
            // Arrange
            var extensionApi = _service.GetExtensionApi();
            var mockTokenProvider = new Mock<ITokenProvider>();
            mockTokenProvider.Setup(x => x.TokenName).Returns("TestToken");
            mockTokenProvider.Setup(x => x.GetValue(It.IsAny<TokenContext>())).Returns("TestValue");
            mockTokenProvider.Setup(x => x.IsAvailable(It.IsAny<TokenContext>())).Returns(true);

            // Act
            extensionApi.RegisterTokenProvider("TestToken", mockTokenProvider.Object);
            var statistics = extensionApi.GetStatistics();

            // Assert
            Assert.Equal(1, statistics.RegisteredTokenProviders);
        }

        /// <summary>
        /// 测试扩展API的验证规则注册
        /// </summary>
        [Fact]
        public void ExtensionApi_RegisterValidationRule_ShouldWork()
        {
            // Arrange
            var extensionApi = _service.GetExtensionApi();
            var mockValidationRule = new Mock<IValidationRule>();
            mockValidationRule.Setup(x => x.RuleName).Returns("TestRule");
            mockValidationRule.Setup(x => x.Validate(It.IsAny<object>()))
                .Returns(new ValidationResult(isValid: true));

            // Act
            extensionApi.RegisterValidationRule("TestRule", mockValidationRule.Object);
            var statistics = extensionApi.GetStatistics();

            // Assert
            Assert.Equal(1, statistics.RegisteredValidationRules);
        }

        #endregion

        #region Error Handling Tests

        /// <summary>
        /// 测试错误处理服务
        /// </summary>
        [Fact]
        public void ErrorHandling_AddError_ShouldTrackError()
        {
            // Arrange
            var errorService = _service.GetErrorHandlingService();

            // Act
            errorService.AddError("Test error", context: "TestContext");
            var errors = errorService.GetErrors();
            var summary = errorService.GetSummary();

            // Assert
            Assert.Single(errors);
            Assert.Equal(1, summary.ErrorCount);
            Assert.True(summary.HasErrors);
        }

        /// <summary>
        /// 测试警告处理服务
        /// </summary>
        [Fact]
        public void ErrorHandling_AddWarning_ShouldTrackWarning()
        {
            // Arrange
            var errorService = _service.GetErrorHandlingService();

            // Act
            errorService.AddWarning("Test warning", context: "TestContext");
            var warnings = errorService.GetWarnings();
            var summary = errorService.GetSummary();

            // Assert
            Assert.Single(warnings);
            Assert.Equal(1, summary.WarningCount);
            Assert.True(summary.HasWarnings);
        }

        #endregion
    }
}
