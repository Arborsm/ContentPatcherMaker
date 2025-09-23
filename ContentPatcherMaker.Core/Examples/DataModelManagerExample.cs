using ContentPatcherMaker.Core.DataModels;
using ContentPatcherMaker.Core.Services;
using ContentPatcherMaker.Core.Services.Logging;

namespace ContentPatcherMaker.Core.Examples;

/// <summary>
/// DataModelManager使用示例
/// 展示如何从Stardew Valley游戏目录加载数据
/// </summary>
public class DataModelManagerExample
{
    private readonly LoggingService _loggingService;
    private readonly DataModelManagerFactory _factory;

    /// <summary>
    /// 初始化示例
    /// </summary>
    public DataModelManagerExample()
    {
        _loggingService = new LoggingService();
        _factory = new DataModelManagerFactory(_loggingService);
    }

    /// <summary>
    /// 示例1：自动检测游戏路径并创建DataModelManager（智能模式）
    /// </summary>
    public async Task<DataModelManager?> Example1_AutoDetectionAsync()
    {
        Console.WriteLine("=== 示例1：自动检测游戏路径并创建DataModelManager (智能模式) ===");
        
        try
        {
            // 自动检测游戏路径并创建DataModelManager
            var result = await _factory.CreateWithAutoDetectionAsync();
            
            if (result.IsSuccess && result.DataModelManager != null)
            {
                Console.WriteLine("✅ 成功创建DataModelManager (智能模式)");
                Console.WriteLine($"游戏路径: {result.GamePath}");
                Console.WriteLine($"内容路径: {result.ContentPath}");
                Console.WriteLine($"平台: {result.Platform}");
                
                // 显示加载状态
                var status = result.DataModelManager.GetLoadingStatus();
                Console.WriteLine($"加载状态: {status}");
                
                return result.DataModelManager;
            }
            else
            {
                Console.WriteLine($"❌ 创建失败: {result.ErrorMessage}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 发生异常: {ex.Message}");
            return null;
        }
    }


    /// <summary>
    /// 示例2：指定游戏路径创建DataModelManager（智能模式）
    /// </summary>
    public async Task<DataModelManager?> Example2_SpecifiedPathAsync(string gamePath)
    {
        Console.WriteLine("=== 示例2：指定游戏路径创建DataModelManager (智能模式) ===");
        
        try
        {
            // 检测游戏路径
            var pathDetectionService = new GamePathDetectionService(_loggingService);
            var detectionResult = pathDetectionService.DetectGamePaths(gamePath);
            
            if (!detectionResult.IsSuccess)
            {
                Console.WriteLine($"❌ 路径检测失败: {detectionResult.ErrorMessage}");
                return null;
            }

            // 创建DataModelManager（智能模式）
            var dataModelManager = _factory.CreateFromGameDirectory(detectionResult.GamePath!, detectionResult.ContentPath!);
            
            // 加载所有数据
            await dataModelManager.LoadAllDataAsync();
            
            Console.WriteLine("✅ 成功创建DataModelManager (智能模式)");
            Console.WriteLine($"游戏路径: {detectionResult.GamePath}");
            Console.WriteLine($"内容路径: {detectionResult.ContentPath}");
            
            return dataModelManager;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 发生异常: {ex.Message}");
            return null;
        }
    }


    /// <summary>
    /// 示例3：选择性加载数据
    /// </summary>
    public async Task<DataModelManager?> Example3_SelectiveLoadingAsync(string gamePath, string contentPath)
    {
        Console.WriteLine("=== 示例3：选择性加载数据 ===");
        
        try
        {
            // 只加载成就和角色数据
            var dataModelManager = await _factory.CreateWithSelectiveLoadingAsync(
                gamePath, 
                contentPath, 
                DataType.Achievements, 
                DataType.Characters);

            Console.WriteLine("✅ 成功创建DataModelManager（选择性加载）");
            
            // 显示加载状态
            var status = dataModelManager.GetLoadingStatus();
            Console.WriteLine($"加载状态: {status}");
            
            return dataModelManager;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 发生异常: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 示例4：使用DataModelManager查询数据
    /// </summary>
    public void Example4_QueryDataAsync(DataModelManager dataModelManager)
    {
        Console.WriteLine("=== 示例4：查询数据 ===");
        
        try
        {
            // 查询成就数据
            if (dataModelManager.IsDataLoaded())
            {
                Console.WriteLine("\n--- 成就数据 ---");
                var achievements = dataModelManager.GetCollection<AchievementDataCollection>().GetAll().Take(5);
                foreach (var achievement in achievements)
                {
                    Console.WriteLine($"- {achievement.Name}: {achievement.Description}");
                }

                Console.WriteLine("\n--- 角色数据 ---");
                var characters = dataModelManager.GetCollection<CharacterDataCollection>().GetAll().Take(5);
                foreach (var character in characters)
                {
                    Console.WriteLine($"- {character.Name} ({character.Type}): 可结婚={character.IsMarriageable}");
                }

                Console.WriteLine("\n--- 事件数据 ---");
                var events = dataModelManager.GetCollection<EventDataCollection>().GetAll().Take(5);
                foreach (var evt in events)
                {
                    Console.WriteLine($"- {evt.Name} ({evt.Type}): {evt.Location}");
                }
            }
            else
            {
                Console.WriteLine("❌ 数据未完全加载");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 查询数据时发生异常: {ex.Message}");
        }
    }

    /// <summary>
    /// 示例5：重新加载数据
    /// </summary>
    public async Task Example5_ReloadDataAsync(DataModelManager dataModelManager)
    {
        Console.WriteLine("=== 示例5：重新加载数据 ===");
        
        try
        {
            Console.WriteLine("重新加载前状态:");
            Console.WriteLine(dataModelManager.GetLoadingStatus());

            // 重新加载所有数据
            await dataModelManager.ReloadAllDataAsync();

            Console.WriteLine("重新加载后状态:");
            Console.WriteLine(dataModelManager.GetLoadingStatus());
            
            Console.WriteLine("✅ 数据重新加载完成");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 重新加载数据时发生异常: {ex.Message}");
        }
    }

    /// <summary>
    /// 运行所有示例（智能模式）
    /// </summary>
    public async Task RunAllExamplesAsync()
    {
        Console.WriteLine("开始运行DataModelManager示例 (智能模式)...\n");

        // 示例1：自动检测
        var dataModelManager = await Example1_AutoDetectionAsync();
        
        if (dataModelManager != null)
        {
            // 示例4：查询数据
            Example4_QueryDataAsync(dataModelManager);
            // 示例5：重新加载数据
            await Example5_ReloadDataAsync(dataModelManager);
        }

        Console.WriteLine("\n所有示例运行完成！");
    }


    /// <summary>
    /// 运行指定游戏路径的示例（智能模式）
    /// </summary>
    public async Task RunWithSpecifiedPathAsync(string gamePath)
    {
        Console.WriteLine($"使用指定游戏路径运行示例 (智能模式): {gamePath}\n");

        // 示例2：指定路径
        var dataModelManager = await Example2_SpecifiedPathAsync(gamePath);
        
        if (dataModelManager != null)
        {
            // 示例3：选择性加载
            var pathDetectionService = new GamePathDetectionService(_loggingService);
            var detectionResult = pathDetectionService.DetectGamePaths(gamePath);
            
            if (detectionResult.IsSuccess)
            {
                var selectiveManager = await Example3_SelectiveLoadingAsync(
                    detectionResult.GamePath!, 
                    detectionResult.ContentPath!);
                
                if (selectiveManager != null)
                {
                    Example4_QueryDataAsync(selectiveManager);
                }
            }
        }

        Console.WriteLine("\n指定路径示例运行完成！");
    }

}
