# DataModelManager 使用指南

本指南介绍如何使用基于StardewXnbHack的DataModelManager来加载Stardew Valley游戏数据。

## 概述

DataModelManager是一个强大的数据管理工具，可以从Stardew Valley游戏目录中加载各种游戏数据，包括：
- 成就数据 (Achievements)
- 农场数据 (Farms)
- 角色数据 (Characters)
- 事件数据 (Events)
- 节日数据 (Festivals)
- 语言数据 (Languages)

## 快速开始

### 1. 自动检测游戏路径

```csharp
using ContentPatcherMaker.Core.Services;
using ContentPatcherMaker.Core.DataModels;

// 创建日志服务
var loggingService = new LoggingService();

// 创建工厂
var factory = new DataModelManagerFactory(loggingService);

// 自动检测并创建DataModelManager
var result = await factory.CreateWithAutoDetectionAsync();

if (result.IsSuccess && result.DataModelManager != null)
{
    Console.WriteLine($"游戏路径: {result.GamePath}");
    Console.WriteLine($"内容路径: {result.ContentPath}");
    Console.WriteLine($"平台: {result.Platform}");
    
    // 使用DataModelManager
    var achievements = result.DataModelManager.Achievements.GetAll();
    foreach (var achievement in achievements)
    {
        Console.WriteLine($"- {achievement.Name}: {achievement.Description}");
    }
}
```

### 2. 指定游戏路径

```csharp
// 指定游戏路径
string gamePath = @"C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley";

// 检测游戏路径
var pathDetectionService = new GamePathDetectionService(loggingService);
var detectionResult = pathDetectionService.DetectGamePaths(gamePath);

if (detectionResult.IsSuccess)
{
    // 创建DataModelManager
    var dataModelManager = factory.CreateFromDetectionResult(detectionResult);
    
    // 加载所有数据
    await dataModelManager.LoadAllDataAsync();
}
```

### 3. 选择性加载数据

```csharp
// 只加载成就和角色数据
var dataModelManager = await factory.CreateWithSelectiveLoadingAsync(
    gamePath, 
    contentPath, 
    DataType.Achievements, 
    DataType.Characters);
```

## 主要功能

### 游戏路径检测

`GamePathDetectionService` 可以自动检测Stardew Valley的安装路径：

```csharp
var pathDetectionService = new GamePathDetectionService(loggingService);
var result = pathDetectionService.DetectGamePaths();

if (result.IsSuccess)
{
    Console.WriteLine($"游戏路径: {result.GamePath}");
    Console.WriteLine($"内容路径: {result.ContentPath}");
    Console.WriteLine($"平台: {result.Platform}");
}
```

支持的平台：
- Windows
- Linux
- macOS

### 数据加载状态

```csharp
// 检查数据是否已加载
bool isLoaded = dataModelManager.IsDataLoaded();

// 获取详细加载状态
var status = dataModelManager.GetLoadingStatus();
Console.WriteLine($"加载进度: {status.LoadingProgress:F1}%");
Console.WriteLine($"已加载: {status.LoadedCount}/{status.TotalCount}");
```

### 重新加载数据

```csharp
// 重新加载所有数据
await dataModelManager.ReloadAllDataAsync();
```

## 数据查询示例

### 成就数据

```csharp
var achievements = dataModelManager.Achievements;

// 获取所有成就
var allAchievements = achievements.GetAll();

// 获取可见成就
var visibleAchievements = achievements.GetVisibleAchievements();

// 获取隐藏成就
var hiddenAchievements = achievements.GetHiddenAchievements();

// 根据ID获取成就
var specificAchievement = achievements.GetById("achievement_id");

// 搜索成就
var searchResults = achievements.Search(a => a.Name.Contains("农场"));
```

### 角色数据

```csharp
var characters = dataModelManager.Characters;

// 获取所有角色
var allCharacters = characters.GetAll();

// 获取可结婚的角色
var marriageableCharacters = characters.Search(c => c.IsMarriageable);

// 获取特定类型的角色
var villagers = characters.Search(c => c.Type == CharacterType.Villager);
```

### 事件数据

```csharp
var events = dataModelManager.Events;

// 获取所有事件
var allEvents = events.GetAll();

// 获取特定类型的事件
var storyEvents = events.Search(e => e.Type == EventType.Story);

// 获取特定位置的事件
var townEvents = events.Search(e => e.Location == "Town");
```

## 错误处理

```csharp
try
{
    var result = await factory.CreateWithAutoDetectionAsync();
    
    if (!result.IsSuccess)
    {
        Console.WriteLine($"创建失败: {result.ErrorMessage}");
        return;
    }
    
    // 使用DataModelManager...
}
catch (Exception ex)
{
    Console.WriteLine($"发生异常: {ex.Message}");
}
```

## 运行示例程序

1. 编译示例项目：
```bash
dotnet build ContentPatcherMaker.Examples
```

2. 运行示例（自动检测）：
```bash
dotnet run --project ContentPatcherMaker.Examples
```

3. 运行示例（指定路径）：
```bash
dotnet run --project ContentPatcherMaker.Examples -- "C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley"
```

## 注意事项

1. **游戏路径检测**：程序会按以下顺序检测游戏路径：
   - 用户指定的路径
   - 当前工作目录
   - 系统注册表（Windows）

2. **内容路径**：不同平台的内容路径不同：
   - Windows/Linux: `{游戏路径}/Content`
   - macOS: `{游戏路径}/../Resources/Content` 或 `{游戏路径}/../../Resources/Content`

3. **数据加载**：所有数据加载都是异步的，支持并行加载以提高性能。

4. **错误处理**：建议始终检查操作结果和异常，确保程序的健壮性。

## 扩展功能

你可以通过以下方式扩展DataModelManager：

1. **添加新的数据类型**：在`DataType`枚举中添加新类型
2. **自定义数据解析**：在DataModelManager中添加新的加载方法
3. **添加数据验证**：在数据模型中添加验证逻辑
4. **性能优化**：使用缓存和延迟加载优化性能

## 故障排除

### 常见问题

1. **找不到游戏路径**
   - 确保Stardew Valley已正确安装
   - 检查游戏文件夹中是否有`Stardew Valley.dll`文件
   - 尝试手动指定游戏路径

2. **内容路径不存在**
   - 确保游戏安装完整
   - 检查Content文件夹是否存在
   - 验证文件权限

3. **数据加载失败**
   - 检查JSON文件格式是否正确
   - 查看日志输出了解详细错误信息
   - 确保有足够的文件访问权限

### 调试技巧

1. **启用详细日志**：配置LoggingService输出详细日志
2. **检查文件路径**：验证所有路径是否正确
3. **逐步加载**：使用选择性加载功能逐步测试
4. **异常处理**：添加try-catch块捕获具体错误
