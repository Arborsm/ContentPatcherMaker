# ContentPatcher 后端系统

这是一个基于ContentPatcher规范的完整后端系统，用于可视化创建和编辑Stardew Valley游戏的ContentPatcher模组内容包。

## 功能特性

### ✅ 核心功能
- **完整的ContentPatcher数据模型** - 支持所有ContentPatcher操作类型（Load、EditData、EditImage、EditMap、Include）
- **完善的参数验证机制** - 严格验证所有输入参数，确保生成的JSON符合ContentPatcher规范
- **错误处理和日志记录** - 提供详细的错误信息和日志记录，便于调试和问题排查
- **JSON生成和解析** - 自动生成符合ContentPatcher规范的JSON格式输出文件
- **Stardew Valley兼容性验证** - 严格验证生成的内容包与Stardew Valley游戏的兼容性

### ✅ 扩展功能
- **扩展API接口** - 允许第三方插件添加新的配置参数类型和自定义功能
- **自定义令牌支持** - 支持注册自定义令牌提供者
- **自定义验证规则** - 支持注册自定义验证规则
- **自定义输出格式化器** - 支持注册自定义输出格式化器

## 架构设计

### 核心组件

```
Core/
├── Models/                    # 数据模型
│   └── ContentPatcherModels.cs
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
├── Extensions/                # 扩展API
│   ├── IExtensionApi.cs      # 扩展接口定义
│   └── ExtensionApiService.cs # 扩展服务实现
└── Examples/                  # 使用示例
    └── ContentPatcherExample.cs
```

## 快速开始

### 1. 创建内容包

```csharp
var service = new ContentPatcherService();

// 创建新的内容包
var contentPack = service.CreateContentPack("2.8.0");
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

## 支持的补丁类型

### Load 补丁
用于替换整个素材文件。

```csharp
var loadPatch = new LoadPatch
{
    Target = "Portraits/Abigail",
    FromFile = "assets/abigail.png",
    Priority = "High",
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
    PatchMode = "Replace"
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

### 注册自定义补丁类型

```csharp
var customValidator = new CustomPatchValidator();
var customProcessor = new CustomPatchProcessor();
extensionApi.RegisterCustomPatchType("CustomAction", customValidator, customProcessor);
```

## 错误处理和日志

### 错误处理

```csharp
var errorService = service.GetErrorHandlingService();

// 检查是否有错误
if (errorService.HasErrors())
{
    var errors = errorService.GetErrors();
    foreach (var error in errors)
    {
        Console.WriteLine($"错误: {error}");
    }
}

// 获取错误摘要
var summary = errorService.GetSummary();
Console.WriteLine($"错误数量: {summary.ErrorCount}");
```

### 日志记录

```csharp
var loggingService = service.GetLoggingService();

// 设置日志级别
loggingService.SetMinimumLevel(LogLevel.Information);

// 获取最近的日志
var recentLogs = loggingService.GetRecentLogs(50);
foreach (var log in recentLogs)
{
    Console.WriteLine(log.ToString());
}

// 导出日志到文件
loggingService.ExportToFile("logs.txt", LogLevel.Debug);
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

## 示例程序

运行示例程序来了解系统的使用方法：

```csharp
var example = new ContentPatcherExample();
example.RunAllExamples();
example.ExtensionApiExample();
example.ErrorHandlingExample();
```

## 技术规范

### ContentPatcher格式版本支持
- 支持格式版本 1.0.0 到 2.8.0
- 推荐使用最新版本 2.8.0
- 自动验证版本兼容性

### JSON输出格式
- 使用System.Text.Json进行序列化
- 支持缩进格式化
- 自动忽略null值
- 符合ContentPatcher规范

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

### v1.0.0
- 初始版本发布
- 支持所有ContentPatcher操作类型
- 完整的验证和错误处理机制
- 扩展API接口
- Stardew Valley兼容性验证

