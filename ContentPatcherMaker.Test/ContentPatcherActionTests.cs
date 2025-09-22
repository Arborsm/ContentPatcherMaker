using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using ContentPatcherMaker.Core.Models;
using ContentPatcherMaker.Core.Services;

namespace ContentPatcherMaker.Test
{
    /// <summary>
    /// 基于md文档中具体操作示例的测试
    /// </summary>
    public class ContentPatcherActionTests
    {
        private readonly ContentPatcherService _service;

        public ContentPatcherActionTests()
        {
            _service = new ContentPatcherService();
        }

        #region Load Action Examples from action-load.md

        /// <summary>
        /// 测试 action-load.md 中的第一个示例：替换阿比盖尔肖像
        /// </summary>
        [Fact]
        public void LoadAction_AbigailPortraitExample_ShouldMatchDocumentation()
        {
            // Arrange - 基于 action-load.md 中的示例
            var contentPack = _service.CreateContentPack("2.8.0");
            var loadPatch = new LoadPatch
            {
                Target = "Portraits/Abigail",
                FromFile = "assets/abigail.png"
            };

            // Act
            _service.AddPatch(contentPack, loadPatch);
            var json = _service.GenerateJson(contentPack);

            // Assert - 验证生成的JSON与文档示例一致
            Assert.Contains("\"Format\": \"2.8.0\"", json);
            Assert.Contains("\"Action\": \"Load\"", json);
            Assert.Contains("\"Target\": \"Portraits/Abigail\"", json);
            Assert.Contains("\"FromFile\": \"assets/abigail.png\"", json);
        }

        /// <summary>
        /// 测试 action-load.md 中的多个补丁示例
        /// </summary>
        [Fact]
        public void LoadAction_MultiplePatchesExample_ShouldMatchDocumentation()
        {
            // Arrange - 基于 action-load.md 中的多个补丁示例
            var contentPack = _service.CreateContentPack("2.8.0");
            
            var abigailPatch = new LoadPatch
            {
                Target = "Portraits/Abigail",
                FromFile = "assets/abigail.png"
            };
            
            var pennyPatch = new LoadPatch
            {
                Target = "Portraits/Penny",
                FromFile = "assets/penny.png"
            };

            // Act
            _service.AddPatch(contentPack, abigailPatch);
            _service.AddPatch(contentPack, pennyPatch);
            var json = _service.GenerateJson(contentPack);

            // Assert
            Assert.Contains("Portraits/Abigail", json);
            Assert.Contains("Portraits/Penny", json);
            Assert.Contains("assets/abigail.png", json);
            Assert.Contains("assets/penny.png", json);
        }

        /// <summary>
        /// 测试 action-load.md 中的优先级示例
        /// </summary>
        [Fact]
        public void LoadAction_PriorityExample_ShouldMatchDocumentation()
        {
            // Arrange - 基于 action-load.md 中的优先级示例
            var contentPack = _service.CreateContentPack("2.8.0");
            var loadPatch = new LoadPatch
            {
                Target = "Data/Events/AdventureGuild",
                FromFile = "assets/empty-event-file.json",
                Priority = "Low"
            };

            // Act
            _service.AddPatch(contentPack, loadPatch);
            var json = _service.GenerateJson(contentPack);

            // Assert
            Assert.Contains("\"Priority\": \"Low\"", json);
            Assert.Contains("Data/Events/AdventureGuild", json);
            Assert.Contains("assets/empty-event-file.json", json);
        }

        #endregion

        #region EditData Action Examples from action-editdata.md

        /// <summary>
        /// 测试 action-editdata.md 中的基本示例
        /// </summary>
        [Fact]
        public void EditDataAction_BasicExample_ShouldMatchDocumentation()
        {
            // Arrange - 基于 action-editdata.md 中的示例
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
            var json = _service.GenerateJson(contentPack);

            // Assert
            Assert.Contains("\"Action\": \"EditData\"", json);
            Assert.Contains("\"Target\": \"Data/Objects\"", json);
            Assert.Contains("MossSoup", json);
            Assert.Contains("\"Price\": 80", json);
        }

        /// <summary>
        /// 测试 action-editdata.md 中的Fields示例
        /// </summary>
        [Fact]
        public void EditDataAction_FieldsExample_ShouldMatchDocumentation()
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
            var json = _service.GenerateJson(contentPack);

            // Assert
            Assert.Contains("\"Fields\":", json);
            Assert.Contains("MossSoup", json);
            Assert.Contains("A delicious soup made from moss", json);
        }

        /// <summary>
        /// 测试 action-editdata.md 中的TextOperations示例
        /// </summary>
        [Fact]
        public void EditDataAction_TextOperationsExample_ShouldMatchDocumentation()
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
            var json = _service.GenerateJson(contentPack);

            // Assert
            Assert.Contains("\"TextOperations\":", json);
            Assert.Contains("\"Operation\": \"Append\"", json);
            Assert.Contains("Universal_Love", json);
            Assert.Contains("\"Value\": \"127\"", json);
        }

        #endregion

        #region EditImage Action Examples from action-editimage.md

        /// <summary>
        /// 测试 action-editimage.md 中的基本示例
        /// </summary>
        [Fact]
        public void EditImageAction_BasicExample_ShouldMatchDocumentation()
        {
            // Arrange - 基于 action-editimage.md 中的示例
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
            var json = _service.GenerateJson(contentPack);

            // Assert
            Assert.Contains("\"Action\": \"EditImage\"", json);
            Assert.Contains("\"Target\": \"Maps/springobjects\"", json);
            Assert.Contains("assets/fish-object.png", json);
            Assert.Contains("\"PatchMode\": \"Replace\"", json);
        }

        /// <summary>
        /// 测试 action-editimage.md 中的Overlay示例
        /// </summary>
        [Fact]
        public void EditImageAction_OverlayExample_ShouldMatchDocumentation()
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
            var json = _service.GenerateJson(contentPack);

            // Assert
            Assert.Contains("\"PatchMode\": \"Overlay\"", json);
            Assert.Contains("assets/overlay.png", json);
        }

        #endregion

        #region EditMap Action Examples from action-editmap.md

        /// <summary>
        /// 测试 action-editmap.md 中的基本示例
        /// </summary>
        [Fact]
        public void EditMapAction_BasicExample_ShouldMatchDocumentation()
        {
            // Arrange - 基于 action-editmap.md 中的示例
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
            var json = _service.GenerateJson(contentPack);

            // Assert
            Assert.Contains("\"Action\": \"EditMap\"", json);
            Assert.Contains("\"Target\": \"Maps/Town\"", json);
            Assert.Contains("assets/town.tmx", json);
            Assert.Contains("\"PatchMode\": \"Replace\"", json);
        }

        /// <summary>
        /// 测试 action-editmap.md 中的地图属性示例
        /// </summary>
        [Fact]
        public void EditMapAction_MapPropertiesExample_ShouldMatchDocumentation()
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
            var json = _service.GenerateJson(contentPack);

            // Assert
            Assert.Contains("\"MapProperties\":", json);
            Assert.Contains("\"Outdoors\": \"T\"", json);
        }

        /// <summary>
        /// 测试 action-editmap.md 中的地图图块示例
        /// </summary>
        [Fact]
        public void EditMapAction_MapTilesExample_ShouldMatchDocumentation()
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
            var json = _service.GenerateJson(contentPack);

            // Assert
            Assert.Contains("\"MapTiles\":", json);
            Assert.Contains("\"Layer\": \"Back\"", json);
            Assert.Contains("\"SetIndex\": \"622\"", json);
        }

        #endregion

        #region Include Action Examples from action-include.md

        /// <summary>
        /// 测试 action-include.md 中的基本示例
        /// </summary>
        [Fact]
        public void IncludeAction_BasicExample_ShouldMatchDocumentation()
        {
            // Arrange - 基于 action-include.md 中的示例
            var contentPack = _service.CreateContentPack("2.8.0");
            var includePatch = new IncludePatch
            {
                FromFile = "assets/dialogue-changes.json"
            };

            // Act
            _service.AddPatch(contentPack, includePatch);
            var json = _service.GenerateJson(contentPack);

            // Assert
            Assert.Contains("\"Action\": \"Include\"", json);
            Assert.Contains("assets/dialogue-changes.json", json);
        }

        #endregion

        #region Complex Examples from author-guide.md

        /// <summary>
        /// 测试 author-guide.md 中的复杂示例组合
        /// </summary>
        [Fact]
        public void ComplexExample_MultipleActions_ShouldWork()
        {
            // Arrange - 创建一个包含多种操作的内容包
            var contentPack = _service.CreateContentPack("2.8.0");
            
            // Load操作
            _service.AddPatch(contentPack, new LoadPatch
            {
                Target = "Portraits/Abigail",
                FromFile = "assets/abigail.png",
                LogName = "替换阿比盖尔肖像"
            });

            // EditData操作
            _service.AddPatch(contentPack, new EditDataPatch
            {
                Target = "Data/Objects",
                Entries = new Dictionary<string, object>
                {
                    ["CustomItem"] = new Dictionary<string, object>
                    {
                        ["Price"] = 100,
                        ["Description"] = "A custom item"
                    }
                }
            });

            // EditImage操作
            _service.AddPatch(contentPack, new EditImagePatch
            {
                Target = "Maps/springobjects",
                FromFile = "assets/custom-object.png",
                PatchMode = "Replace"
            });

            // Include操作
            _service.AddPatch(contentPack, new IncludePatch
            {
                FromFile = "assets/additional-changes.json"
            });

            // Act
            var validationResult = _service.ValidateContentPack(contentPack);
            var json = _service.GenerateJson(contentPack);

            // Assert
            Assert.True(validationResult.IsValid);
            Assert.Contains("\"Action\": \"Load\"", json);
            Assert.Contains("\"Action\": \"EditData\"", json);
            Assert.Contains("\"Action\": \"EditImage\"", json);
            Assert.Contains("\"Action\": \"Include\"", json);
            Assert.Equal(4, contentPack.Changes.Count);
        }

        #endregion

        #region Validation Tests

        /// <summary>
        /// 测试所有操作类型的验证
        /// </summary>
        [Theory]
        [InlineData("Load")]
        [InlineData("EditData")]
        [InlineData("EditImage")]
        [InlineData("EditMap")]
        [InlineData("Include")]
        public void Validation_AllActionTypes_ShouldPassValidation(string actionType)
        {
            // Arrange
            var contentPack = _service.CreateContentPack("2.8.0");
            Patch patch = actionType switch
            {
                "Load" => new LoadPatch
                {
                    Target = "Test/Target",
                    FromFile = "test.png"
                },
                "EditData" => new EditDataPatch
                {
                    Target = "Data/Test",
                    Entries = new Dictionary<string, object>
                    {
                        ["TestItem"] = new Dictionary<string, object>
                        {
                            ["Price"] = 100
                        }
                    }
                },
                "EditImage" => new EditImagePatch
                {
                    Target = "Maps/Test",
                    FromFile = "test.png"
                },
                "EditMap" => new EditMapPatch
                {
                    Target = "Maps/Test",
                    MapProperties = new Dictionary<string, string>
                    {
                        ["TestProp"] = "TestValue"
                    }
                },
                "Include" => new IncludePatch
                {
                    FromFile = "test.json"
                },
                _ => throw new ArgumentException($"Unknown action type: {actionType}")
            };

            // Act
            _service.AddPatch(contentPack, patch);
            var validationResult = _service.ValidateContentPack(contentPack);

            // Assert
            Assert.True(validationResult.IsValid, $"Action type {actionType} should pass validation");
        }

        #endregion
    }
}
