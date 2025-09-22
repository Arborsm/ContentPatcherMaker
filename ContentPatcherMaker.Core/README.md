# ContentPatcher 后端系统

这是一个基于ContentPatcher规范的完整后端系统，用于可视化创建和编辑Stardew Valley游戏的ContentPatcher模组内容包。

## 功能特性

### ✅ 核心功能
- **完整的ContentPatcher数据模型** - 支持所有ContentPatcher操作类型（Load、EditData、EditImage、EditMap、Include）
- **动态数据模型系统** - 从JSON文件动态加载游戏数据，支持代码补全和前端选择
- **完善的参数验证机制** - 严格验证所有输入参数，确保生成的JSON符合ContentPatcher规范
- **错误处理和日志记录** - 提供详细的错误信息和日志记录，便于调试和问题排查
- **JSON生成和解析** - 自动生成符合ContentPatcher规范的JSON格式输出文件
- **Stardew Valley兼容性验证** - 严格验证生成的内容包与Stardew Valley游戏的兼容性

### ✅ 扩展功能
- **扩展API接口** - 允许第三方插件添加新的配置参数类型和自定义功能
- **自定义令牌支持** - 支持注册自定义令牌提供者
- **自定义验证规则** - 支持注册自定义验证规则
- **自定义输出格式化器** - 支持注册自定义输出格式化器
- **数据模型加载器** - 支持注册自定义数据模型加载器

## 架构设计

### 核心组件

```
Core/
├── Models/                    # 数据模型
│   └── ContentPatcherModels.cs
├── DataModels/               # 动态数据模型系统
│   ├── IDataModel.cs         # 数据模型接口
│   ├── Enums.cs              # 枚举定义
│   ├── AchievementData.cs    # 成就数据模型
│   ├── FarmData.cs           # 农场数据模型
│   ├── CharacterData.cs      # 角色数据模型
│   ├── EventData.cs          # 事件数据模型
│   ├── FestivalData.cs       # 节日数据模型
│   ├── LanguageData.cs       # 语言数据模型
│   ├── JsonDataLoader.cs     # JSON加载器
│   ├── DataModelManager.cs   # 数据模型管理器
│   └── ValidationResult.cs   # 验证结果
├── Validation/                # 验证逻辑
│   └── ContentPatcherValidator.cs
├── Services/                  # 核心服务
│   ├── ContentPatcherService.cs      # 主服务类
│   ├── JsonGeneratorService.cs       # JSON生成服务
│   ├── StardewValleyCompatibilityService.cs  # 兼容性验证服务
│   ├── ErrorHandling/         # 错误处理
│   │   └── ErrorHandlingService.cs
│   └── Logging/               # 日志服务
│       └── LoggingService.cs
└── Extensions/                # 扩展API
    ├── IExtensionApi.cs      # 扩展接口定义
    └── ExtensionApiService.cs # 扩展服务实现
```

## 快速开始

### 1. 创建内容包

```csharp
var service = new ContentPatcherService();

// 创建新的内容包
var contentPack = service.CreateContentPack();
```

### 2. 添加补丁

```csharp
// 添加Load补丁 - 替换角色肖像
var loadPatch = new LoadPatch
{
    Target = "Portraits/Abigail",
    FromFile = "assets/abigail.png",
    LogName = "替换阿比盖尔肖像"
};
service.AddPatch(contentPack, loadPatch);

// 添加EditData补丁 - 修改物品数据
var editDataPatch = new EditDataPatch
{
    Target = "Data/Objects",
    Entries = new Dictionary<string, object>
    {
        ["MossSoup"] = new Dictionary<string, object>
        {
            ["Price"] = 80,
            ["Description"] = "A delicious soup made from moss."
        }
    }
};
service.AddPatch(contentPack, editDataPatch);
```

### 3. 验证和生成

```csharp
// 验证内容包
var validationResult = service.ValidateContentPack(contentPack);
if (!validationResult.IsValid)
{
    Console.WriteLine($"验证失败: {string.Join(", ", validationResult.Errors)}");
    return;
}

// 检查兼容性
var compatibilityResult = service.CheckCompatibility(contentPack);
if (!compatibilityResult.IsCompatible)
{
    Console.WriteLine($"兼容性检查失败: {string.Join(", ", compatibilityResult.Errors)}");
    return;
}

// 生成JSON
var json = service.GenerateJson(contentPack);

// 保存到文件
service.SaveContentPack(contentPack, "my-content-pack.json");
```

## 数据模型系统

### 动态数据加载

系统支持从JSON文件动态加载游戏数据，提供类型安全和代码补全支持：

```csharp
// 创建数据模型管理器
var jsonLoader = new JsonDataLoader(loggingService, contentPath);
var dataManager = new DataModelManager(jsonLoader, loggingService);

// 加载所有数据
await dataManager.LoadAllDataAsync();

// 获取成就数据
var achievements = dataManager.GetModels<AchievementData>("Achievements");
var allAchievements = achievements?.Models;
var specificAchievement = achievements?.GetById("0");
```

### 支持的数据类型

- **成就数据** (`AchievementData`) - 游戏成就信息
- **农场数据** (`FarmData`) - 农场类型信息
- **角色数据** (`CharacterData`) - 角色信息和日程
- **事件数据** (`EventData`) - 游戏事件信息
- **节日数据** (`FestivalData`) - 节日和庆典信息
- **语言数据** (`LanguageData`) - 多语言支持

### 枚举类型

系统提供了完整的枚举类型支持，确保类型安全：

- `PatchActionType` - 补丁操作类型
- `PatchUpdateFrequency` - 补丁更新频率
- `PatchPriority` - 补丁优先级
- `PatchMode` - 补丁模式
- `TextOperationType` - 文本操作类型
- `MapLayer` - 地图图层
- `CharacterType` - 角色类型
- `EventType` - 事件类型
- `FestivalType` - 节日类型

## 支持的补丁类型

### Load 补丁
用于替换整个素材文件。

```csharp
var loadPatch = new LoadPatch
{
    Target = "Portraits/Abigail",
    FromFile = "assets/abigail.png",
    Priority = PatchPriority.High,
    When = new Dictionary<string, string>
    {
        ["Season"] = "Spring"
    }
};
```

### EditData 补丁
用于编辑数据素材中的字段和条目。

```csharp
var editDataPatch = new EditDataPatch
{
    Target = "Data/Objects",
    Entries = new Dictionary<string, object>
    {
        ["CustomItem"] = new Dictionary<string, object>
        {
            ["Name"] = "Custom Item",
            ["Price"] = 100,
            ["Type"] = "Basic"
        }
    },
    Fields = new Dictionary<string, object>
    {
        ["MossSoup"] = new Dictionary<string, object>
        {
            ["Price"] = 80
        }
    }
};
```

### EditImage 补丁
用于编辑图像素材的一部分。

```csharp
var editImagePatch = new EditImagePatch
{
    Target = "Maps/springobjects",
    FromFile = "assets/fish-object.png",
    FromArea = new Area { X = 0, Y = 0, Width = 16, Height = 16 },
    ToArea = new Area { X = 256, Y = 96, Width = 16, Height = 16 },
    PatchMode = PatchMode.Replace
};
```

### EditMap 补丁
用于编辑地图素材。

```csharp
var editMapPatch = new EditMapPatch
{
    Target = "Maps/Town",
    FromFile = "assets/town.tmx",
    MapProperties = new Dictionary<string, string>
    {
        ["Outdoors"] = "T"
    },
    AddWarps = new List<string>
    {
        "10 10 Town 0 30"
    }
};
```

### Include 补丁
用于包含其他JSON文件中的补丁。

```csharp
var includePatch = new IncludePatch
{
    FromFile = "assets/dialogue-changes.json",
    When = new Dictionary<string, string>
    {
        ["EnableDialogue"] = "true"
    }
};
```

## 扩展API使用

### 注册自定义令牌提供者

```csharp
var extensionApi = service.GetExtensionApi();

var customTokenProvider = new CustomTokenProvider();
extensionApi.RegisterTokenProvider("CustomPlayerLevel", customTokenProvider);
```

### 注册自定义验证规则

```csharp
var customValidationRule = new CustomValidationRule();
extensionApi.RegisterValidationRule("CustomRule", customValidationRule);
```

### 注册自定义数据模型加载器

```csharp
var customLoader = new CustomDataModelLoader();
extensionApi.RegisterDataModelLoader<CustomData>("CustomData", customLoader);

// 获取数据模型
var customModel = extensionApi.GetDataModel<CustomData>("CustomData");
```

## 验证规则

系统会自动验证以下内容：

### 基本验证
- Format字段格式和版本
- 补丁的必填字段
- 操作类型的有效性
- 文件路径格式

### ContentPatcher特定验证
- 目标素材的有效性
- 补丁操作的正确性
- 条件表达式的语法
- 令牌使用的有效性

### Stardew Valley兼容性验证
- 格式版本兼容性
- 已知素材的验证
- 弃用功能的警告
- 游戏版本兼容性

## 技术规范

### ContentPatcher格式支持
- 自动使用最新格式版本
- 自动验证版本兼容性

### JSON输出格式
- 使用Newtonsoft.Json进行序列化
- 支持缩进格式化
- 自动忽略null值
- 符合ContentPatcher规范
- 枚举类型序列化为字符串

### 错误处理
- 详细的错误信息
- 异常堆栈跟踪
- 上下文信息
- 错误分类和严重程度

## 许可证

本项目基于ContentPatcher规范开发，遵循相应的开源许可证。

## 贡献

欢迎提交Issue和Pull Request来改进这个系统。

## 更新日志

### v2.0.0
- 添加动态数据模型系统
- 支持从JSON文件加载游戏数据
- 增强类型安全性和代码补全
- 优化枚举类型支持
- 改进扩展API功能

### v1.0.0
- 初始版本发布
- 支持所有ContentPatcher操作类型
- 完整的验证和错误处理机制
- 扩展API接口
- Stardew Valley兼容性验证