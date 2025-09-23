# ContentPatcherMakerCore 使用指南

## 概述

ContentPatcherMakerCore 是一个简化的核心系统，专注于 DataModelCollection 管理和文件夹监控功能。

## 主要功能

### 1. DataModelManager 数据模型管理
- 智能加载 Stardew Valley 游戏数据（XNB 和 JSON 格式）
- 支持成就、农场、角色、事件、节日、语言等数据集合
- 提供数据加载状态监控和错误处理
- 自动解析和转换游戏数据格式

### 2. DataModelCollection 管理
- 自动注册各种数据集合到内部字典
- 提供直接访问数据集合的方法
- 支持数据集合的获取、检查等操作

### 3. 文件夹监控服务
- 实时监控指定文件夹的文件变化
- 支持文件过滤和回调处理
- 自动重新加载数据文件

### 4. 数据持久化
- JSON 格式数据序列化
- 文件操作（保存、加载、删除、备份、恢复）
- 自动目录创建

## 使用方法

### 基本初始化

```csharp
// 创建核心实例
var core = new ContentPatcherMakerCore();

// 初始化系统
await core.InitializeAsync();

// 使用完毕后释放资源
core.Dispose();
```

### DataModelManager 操作

```csharp
// 获取数据加载状态
var loadingStatus = core.DataModelManager.GetLoadingStatus();

// 检查数据是否已加载
var isDataLoaded = core.DataModelManager.IsDataLoaded();

// 获取各种数据集合
var achievements = core.DataModelManager.GetCollection<AchievementDataCollection>();
var farms = core.DataModelManager.GetCollection<FarmDataCollection>();
var characters = core.DataModelManager.GetCollection<CharacterDataCollection>();
var events = core.DataModelManager.GetCollection<EventDataCollection>();
var festivals = core.DataModelManager.GetCollection<FestivalDataCollection>();
var languages = core.DataModelManager.GetCollection<LanguageDataCollection>();

// 重新加载所有数据
await core.DataModelManager.ReloadAllDataAsync();
```

### DataModelCollection 操作

```csharp
// 获取已注册的数据集合类型
var types = core.GetRegisteredDataModelCollectionTypes();

// 获取特定的数据集合
var achievementCollection = core.GetDataModelCollection<AchievementDataCollection>();

// 检查数据集合是否已注册
var isRegistered = core.IsDataModelCollectionRegistered<AchievementDataCollection>();

// 获取系统统计信息
var stats = core.GetSystemStatistics();
```

### 文件夹监控

```csharp
// 添加数据路径（自动开始监控）
core.AddDataPath("path/to/data/folder");

// 移除数据路径（停止监控）
core.RemoveDataPath("path/to/data/folder");

// 获取监控的文件夹列表
var watchedFolders = core.FolderWatcherService.GetWatchedFolders();

// 获取监控统计信息
var folderStats = core.FolderWatcherService.GetStatistics();
```

### 数据持久化

```csharp
// 保存数据
await core.PersistenceService.SaveAsync(data, "data.json");

// 加载数据
var loadedData = await core.PersistenceService.LoadAsync<MyData>("data.json");

// 检查文件是否存在
var exists = core.PersistenceService.Exists("data.json");

// 删除文件
await core.PersistenceService.DeleteAsync("data.json");

// 备份文件
await core.PersistenceService.BackupAsync("data.json", "data-backup.json");

// 恢复文件
await core.PersistenceService.RestoreAsync("data-backup.json", "data.json");
```

### 系统配置

```csharp
// 获取系统统计信息
var systemStats = core.GetSystemStatistics();

// 保存系统配置
await core.SaveSystemConfigurationAsync("config.json");

// 加载系统配置
await core.LoadSystemConfigurationAsync("config.json");
```

## 系统架构

### 核心组件

1. **ContentPatcherMakerCore**: 主核心类，协调各个组件
2. **DataModelManager**: 数据模型管理器，负责加载游戏数据
3. **FolderWatcherService**: 文件夹监控服务
4. **JsonDataPersistenceService**: JSON 数据持久化服务
5. **LoggingService**: 日志服务

### 数据流程

1. 系统启动时 DataModelManager 自动加载游戏数据（XNB/JSON 格式）
2. 解析和转换数据为 DataModelCollection 格式
3. 注册到内部字典中供访问
4. 开始监控数据文件夹的变化
5. 当文件发生变化时，自动重新加载对应的数据集合

## 配置说明

### 默认数据路径
- 应用程序目录下的 `Data` 文件夹
- 支持添加自定义数据路径

### 支持的数据类型
- AchievementDataCollection (成就数据)
- FarmDataCollection (农场数据)
- CharacterDataCollection (角色数据)
- EventDataCollection (事件数据)
- FestivalDataCollection (节日数据)
- LanguageDataCollection (语言数据)

### 文件格式
- 所有数据文件使用 JSON 格式
- 支持自动备份和恢复
- 支持文件变化监控

## 示例代码

完整的使用示例请参考 `ContentPatcherMakerCoreExample.cs` 文件。

## 注意事项

1. 使用完毕后请调用 `Dispose()` 方法释放资源
2. 文件夹监控服务会自动处理文件变化事件
3. 数据集合的修改会自动保存到文件
4. 系统支持多个数据路径的监控
5. 所有操作都有详细的日志记录
