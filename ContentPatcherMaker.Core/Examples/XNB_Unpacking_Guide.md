# XNB解包功能使用指南

本指南介绍如何使用基于StardewXnbHack的XNB解包功能来解包Stardew Valley游戏资源。

## 概述

XNB解包服务可以将Stardew Valley游戏中的XNB（XNA Binary）文件解包为可编辑的格式，支持：
- **纹理文件** (.png) - 角色、物品、建筑等图像资源
- **地图文件** (.tmx) - 游戏地图和场景
- **数据文件** (.json) - 游戏数据配置
- **精灵字体** (.json) - 字体资源
- **XML文件** (.xml) - 配置文件

## 快速开始

### 1. 基本解包

```csharp
using ContentPatcherMaker.Core.Services;

// 创建服务
var loggingService = new LoggingService();
var unpackingService = new XnbUnpackingService(loggingService);

// 创建ContentManager
using var game = new Game();
var contentManager = new ContentManager(game.Services, "Content");

// 解包单个文件
var result = await unpackingService.UnpackXnbFileAsync(
    "path/to/file.xnb", 
    "output/path", 
    contentManager);

if (result.IsSuccess)
{
    Console.WriteLine($"解包成功: {result.OutputPath}");
    Console.WriteLine($"资源类型: {result.AssetType}");
}
```

### 2. 批量解包

```csharp
// 解包Content目录下的所有XNB文件
var result = await unpackingService.UnpackAllXnbFilesAsync(
    contentDirectory, 
    outputDirectory, 
    contentManager);

Console.WriteLine($"解包完成: 成功 {result.SuccessCount} 个，失败 {result.FailureCount} 个");
```

### 3. 解包特定文件

```csharp
// 解包特定模式的文件
var xnbFiles = Directory.GetFiles(contentDirectory, "Characters/*.xnb", SearchOption.AllDirectories);
var results = await unpackingService.UnpackXnbFilesAsync(xnbFiles, outputDirectory, contentManager);

foreach (var result in results)
{
    var status = result.IsSuccess ? "✅" : "❌";
    Console.WriteLine($"{status} {Path.GetFileName(result.OutputPath)} - {result.AssetType}");
}
```

## 支持的资源类型

### 纹理资源 (Texture2D)
- **输入**: `.xnb` 文件
- **输出**: `.png` 文件
- **特点**: 自动处理透明度预乘，保持原始图像质量

```csharp
// 解包角色肖像
var result = await unpackingService.UnpackXnbFileAsync(
    "Portraits/Abigail.xnb", 
    "output/Portraits/Abigail", 
    contentManager);
// 输出: output/Portraits/Abigail.png
```

### 地图资源 (Map)
- **输入**: `.xnb` 文件
- **输出**: `.tmx` 文件
- **特点**: 转换为Tiled地图编辑器格式

```csharp
// 解包游戏地图
var result = await unpackingService.UnpackXnbFileAsync(
    "Maps/Town.xnb", 
    "output/Maps/Town", 
    contentManager);
// 输出: output/Maps/Town.tmx
```

### 数据资源 (Dictionary/List)
- **输入**: `.xnb` 文件
- **输出**: `.json` 文件
- **特点**: 保持原始数据结构，便于编辑

```csharp
// 解包游戏数据
var result = await unpackingService.UnpackXnbFileAsync(
    "Data/Objects.xnb", 
    "output/Data/Objects", 
    contentManager);
// 输出: output/Data/Objects.json
```

### 精灵字体 (SpriteFont)
- **输入**: `.xnb` 文件
- **输出**: `.json` 文件
- **特点**: 包含字体信息和字符映射

```csharp
// 解包字体资源
var result = await unpackingService.UnpackXnbFileAsync(
    "Fonts/DefaultFont.xnb", 
    "output/Fonts/DefaultFont", 
    contentManager);
// 输出: output/Fonts/DefaultFont.json
```

### XML资源 (XDocument/XElement)
- **输入**: `.xnb` 文件
- **输出**: `.xml` 文件
- **特点**: 保持原始XML结构

```csharp
// 解包XML配置
var result = await unpackingService.UnpackXnbFileAsync(
    "Data/Config.xnb", 
    "output/Data/Config", 
    contentManager);
// 输出: output/Data/Config.xml
```

## 高级用法

### 自定义资产写入器

```csharp
// 创建自定义写入器
public class CustomAssetWriter : BaseAssetWriter
{
    public override bool CanWrite(object asset)
    {
        return asset is CustomAssetType;
    }

    public override bool TryWriteFile(object asset, string toPathWithoutExtension, string relativePath, Platform platform, out string error)
    {
        // 自定义写入逻辑
        var customAsset = (CustomAssetType)asset;
        // ... 处理逻辑
        error = null;
        return true;
    }
}

// 注册自定义写入器
var unpackingService = new XnbUnpackingService(loggingService);
unpackingService.RegisterWriter(new CustomAssetWriter());
```

### 错误处理

```csharp
try
{
    var result = await unpackingService.UnpackXnbFileAsync(xnbFile, outputPath, contentManager);
    
    if (!result.IsSuccess)
    {
        Console.WriteLine($"解包失败: {result.ErrorMessage}");
        
        // 处理特定错误
        if (result.ErrorMessage.Contains("不支持的资源类型"))
        {
            Console.WriteLine("该文件类型不受支持，尝试使用原始XNB文件");
            File.Copy(xnbFile, $"{outputPath}.xnb");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"解包过程中发生异常: {ex.Message}");
}
```

### 进度监控

```csharp
// 批量解包时监控进度
var xnbFiles = Directory.GetFiles(contentDirectory, "*.xnb", SearchOption.AllDirectories);
var results = new List<XnbUnpackingResult>();

for (int i = 0; i < xnbFiles.Length; i++)
{
    var file = xnbFiles[i];
    var fileName = Path.GetFileName(file);
    var outputPath = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(file));
    
    Console.WriteLine($"解包进度: {i + 1}/{xnbFiles.Length} - {fileName}");
    
    var result = await unpackingService.UnpackXnbFileAsync(file, outputPath, contentManager);
    results.Add(result);
    
    // 显示进度
    var progress = (double)(i + 1) / xnbFiles.Length * 100;
    Console.WriteLine($"进度: {progress:F1}%");
}
```

## 运行示例程序

### 命令行用法

```bash
# 解包模式
dotnet run --project ContentPatcherMaker.Examples -- unpack "C:\Program Files\Steam\steamapps\common\Stardew Valley" unpacked_content

# 数据加载模式
dotnet run --project ContentPatcherMaker.Examples -- data "C:\Program Files\Steam\steamapps\common\Stardew Valley"

# 显示帮助
dotnet run --project ContentPatcherMaker.Examples
```

### 程序化调用

```csharp
var example = new XnbUnpackingExample();

// 自动检测并解包
await example.RunAllExamplesAsync();

// 使用指定路径
await example.RunWithSpecifiedPathAsync(gamePath, outputDirectory);

// 解包特定类型文件
await example.Example4_UnpackSpecificTypesAsync(
    contentDirectory, 
    outputDirectory, 
    new[] { "Characters/*.xnb", "Portraits/*.xnb" });
```

## 注意事项

### 1. 游戏路径要求
- 确保Stardew Valley已正确安装
- 程序会自动检测Content文件夹位置
- 不同平台的内容路径可能不同

### 2. 资源依赖
- 某些资源可能依赖其他资源（如地图依赖瓦片集）
- 解包时保持相对路径结构
- 检查资源引用是否正确

### 3. 文件权限
- 确保有足够的文件读写权限
- 某些文件可能被游戏进程占用
- 建议在游戏关闭时进行解包

### 4. 内存使用
- 大量文件解包时注意内存使用
- 考虑分批处理大文件
- 及时释放不需要的资源

## 故障排除

### 常见问题

1. **"不支持的资源类型"错误**
   - 检查资源类型是否在支持列表中
   - 考虑添加自定义写入器
   - 使用原始XNB文件作为备选

2. **"加载XNB文件失败"错误**
   - 检查文件是否损坏
   - 确认文件路径正确
   - 检查ContentManager配置

3. **"写入文件失败"错误**
   - 检查输出目录权限
   - 确认磁盘空间充足
   - 检查文件是否被占用

4. **纹理显示异常**
   - 检查透明度处理
   - 确认颜色格式正确
   - 验证图像尺寸

### 调试技巧

1. **启用详细日志**
```csharp
var loggingService = new LoggingService();
loggingService.LogLevel = LogLevel.Debug;
```

2. **检查资源类型**
```csharp
var asset = contentManager.Load<object>(relativePath);
Console.WriteLine($"资源类型: {asset.GetType().Name}");
```

3. **验证输出文件**
```csharp
if (File.Exists(outputPath))
{
    var fileInfo = new FileInfo(outputPath);
    Console.WriteLine($"文件大小: {fileInfo.Length} 字节");
}
```

## 扩展功能

### 添加新的资源类型支持

1. 创建新的资产写入器
2. 实现`IAssetWriter`接口
3. 注册到`XnbUnpackingService`
4. 测试新资源类型

### 自定义输出格式

1. 继承`BaseAssetWriter`
2. 重写`TryWriteFile`方法
3. 实现自定义序列化逻辑
4. 配置输出文件扩展名

### 批量处理优化

1. 使用并行处理
2. 实现进度回调
3. 添加取消支持
4. 优化内存使用

这个XNB解包功能完全基于StardewXnbHack的实现，提供了强大而灵活的游戏资源解包能力，让开发者可以轻松地访问和编辑Stardew Valley的游戏资源。
