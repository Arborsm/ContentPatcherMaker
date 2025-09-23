using System.Diagnostics.CodeAnalysis;
using ContentPatcherMaker.Core.Services;
using ContentPatcherMaker.Core.Services.Logging;
using StardewValley.GameData;

namespace ContentPatcherMaker.Core.DataModels;

/// <summary>
/// 数据模型管理器
/// 负责管理所有数据模型的加载和访问
/// </summary>
public class DataModelManager
{
    /// <summary>
    /// 所有数据模型集合
    /// </summary>
    private static List<DataModelCollectionBase> Collections { get; } = [];
    
    private readonly JsonDataLoader _jsonLoader;
    private readonly XnbDataLoader _xnbLoader;
    private readonly LoggingService _loggingService;
    private readonly string _contentPath;
    
    private AchievementDataCollection? _achievements;
    private FarmDataCollection? _farms;
    private CharacterDataCollection? _characters;
    private EventDataCollection? _events;
    private FestivalDataCollection? _festivals;
    private LanguageDataCollection? _languages;
    
    /// <summary>
    /// 注册数据模型集合
    /// </summary>
    /// <param name="collection"></param>
    /// <typeparam name="T"></typeparam>
    public static void RegisterCollection<T>(T collection) where T : DataModelCollectionBase
    {
        Collections.Add(collection);
    }
    
    /// <summary>
    /// 获取指定数据模型集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetCollection<T>() where T : DataModelCollectionBase
    {
        return Collections.OfType<T>().FirstOrDefault() ?? throw new InvalidOperationException($"未找到名称为{typeof(T).Name}的数据模型集合");
    }

    /// <summary>
    /// 初始化数据模型管理器（智能模式）
    /// </summary>
    /// <param name="contentPath">内容路径</param>
    /// <param name="loggingService">日志服务</param>
    public DataModelManager(string contentPath, LoggingService loggingService)
    {
        _contentPath = contentPath ?? throw new ArgumentNullException(nameof(contentPath));
        _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        
        // 创建两个加载器
        _jsonLoader = new JsonDataLoader(_loggingService, contentPath);
        _xnbLoader = new XnbDataLoader(_loggingService, contentPath);
    }

    /// <summary>
    /// 初始化数据模型管理器（JSON模式）- 内部使用
    /// </summary>
    /// <param name="jsonLoader">JSON加载器</param>
    /// <param name="loggingService">日志服务</param>
    public DataModelManager(JsonDataLoader jsonLoader, LoggingService loggingService)
    {
        _jsonLoader = jsonLoader ?? throw new ArgumentNullException(nameof(jsonLoader));
        _xnbLoader = null!;
        _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        _contentPath = jsonLoader.ContentPath;
    }

    /// <summary>
    /// 初始化数据模型管理器（XNB模式）- 内部使用
    /// </summary>
    /// <param name="xnbLoader">XNB加载器</param>
    /// <param name="loggingService">日志服务</param>
    public DataModelManager(XnbDataLoader xnbLoader, LoggingService loggingService)
    {
        _jsonLoader = null!;
        _xnbLoader = xnbLoader ?? throw new ArgumentNullException(nameof(xnbLoader));
        _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        _contentPath = xnbLoader.ContentPath;
    }

    /// <summary>
    /// 加载所有数据模型
    /// </summary>
    public async Task LoadAllDataAsync()
    {
        _loggingService.LogInformation("开始加载所有数据模型", "DataModelManager");

        try
        {
            // 并行加载所有数据
            var tasks = new[]
            {
                LoadAchievementsAsync(),
                LoadFarmsAsync(),
                LoadCharactersAsync(),
                LoadEventsAsync(),
                LoadFestivalsAsync(),
                LoadLanguagesAsync()
            };

            await Task.WhenAll(tasks);
            _loggingService.LogInformation("所有数据模型加载完成", "DataModelManager");
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"加载数据模型时发生错误: {ex.Message}", ex, "DataModelManager");
            throw;
        }
    }

    /// <summary>
    /// 检查数据是否已加载
    /// </summary>
    /// <returns>是否已加载</returns>
    public bool IsDataLoaded()
    {
        return _achievements != null && _farms != null && _characters != null && 
               _events != null && _festivals != null && _languages != null;
    }

    /// <summary>
    /// 获取加载状态信息
    /// </summary>
    /// <returns>加载状态信息</returns>
    public DataLoadingStatus GetLoadingStatus()
    {
        return new DataLoadingStatus
        {
            AchievementsLoaded = _achievements != null,
            FarmsLoaded = _farms != null,
            CharactersLoaded = _characters != null,
            EventsLoaded = _events != null,
            FestivalsLoaded = _festivals != null,
            LanguagesLoaded = _languages != null,
            AllLoaded = IsDataLoaded()
        };
    }

    /// <summary>
    /// 重新加载所有数据
    /// </summary>
    public async Task ReloadAllDataAsync()
    {
        _loggingService.LogInformation("开始重新加载所有数据模型", "DataModelManager");
        
        // 清空现有数据
        _achievements = null;
        _farms = null;
        _characters = null;
        _events = null;
        _festivals = null;
        _languages = null;

        // 重新加载
        await LoadAllDataAsync();
    }

    /// <summary>
    /// 加载成就数据
    /// </summary>
    public async Task LoadAchievementsAsync()
    {
        try
        {
            // 智能加载数据文件 - Stardew Valley XNB文件中成就数据是Dictionary<Int32, String>类型
            var achievementsData = await LoadDataFileAsync<Dictionary<int, string>>("Data/Achievements");

            if (achievementsData == null)
            {
                _loggingService.LogWarning("成就数据文件为空或加载失败", "DataModelManager");
                _achievements = new AchievementDataCollection([]);
                return;
            }

            var achievements = achievementsData.Select(kvp => new AchievementData
            {
                Id = kvp.Key.ToString(), // 将int键转换为string
                Name = ExtractAchievementName(kvp.Value),
                Description = ExtractAchievementDescription(kvp.Value),
                IsHidden = ExtractAchievementIsHidden(kvp.Value),
                PrerequisiteId = ExtractAchievementPrerequisite(kvp.Value),
                IconId = ExtractAchievementIcon(kvp.Value)
            }).ToList();

            _achievements = new AchievementDataCollection(achievements);
            _loggingService.LogInformation($"成功加载 {achievements.Count} 个成就", "DataModelManager");
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"加载成就数据时发生错误: {ex.Message}", ex, "DataModelManager");
            _achievements = new AchievementDataCollection([]);
        }
    }

    /// <summary>
    /// 加载农场数据
    /// </summary>
    public async Task LoadFarmsAsync()
    {
        try
        {
            var farmsData = await LoadDataFileAsync<List<ModFarmType>>("Data/AdditionalFarms");

            if (farmsData == null)
            {
                _loggingService.LogWarning("农场数据文件为空或加载失败", "DataModelManager");
                _farms = new FarmDataCollection([]);
                return;
            }

            _farms = new FarmDataCollection(farmsData);
            _loggingService.LogInformation($"成功加载 {farmsData.Count} 个农场", "DataModelManager");
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"加载农场数据时发生错误: {ex.Message}", ex, "DataModelManager");
            _farms = new FarmDataCollection([]);
        }
    }

    /// <summary>
    /// 加载角色数据
    /// </summary>
    public async Task LoadCharactersAsync()
    {
        try
        {
            // 智能获取文件路径
            var scheduleFiles = GetDataFilePaths("Characters/schedules");
            var characters = new List<CharacterData>();

            foreach (var file in scheduleFiles)
            {
                var characterId = Path.GetFileNameWithoutExtension(file);
                var schedules = await LoadDataFileAsync<Dictionary<string, string>>(file);
                
                if (schedules != null)
                {
                    var character = new CharacterData
                    {
                        Id = characterId,
                        Name = characterId, // 默认使用ID作为名称
                        Type = DetermineCharacterType(characterId),
                        IsMarriageable = IsMarriageableCharacter(characterId),
                        Schedules = schedules
                    };
                    characters.Add(character);
                }
            }

            _characters = new CharacterDataCollection(characters);
            _loggingService.LogInformation($"成功加载 {characters.Count} 个角色", "DataModelManager");
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"加载角色数据时发生错误: {ex.Message}", ex, "DataModelManager");
            _characters = new CharacterDataCollection([]);
        }
    }

    /// <summary>
    /// 加载事件数据
    /// </summary>
    public async Task LoadEventsAsync()
    {
        try
        {
            // 智能获取文件路径
            var eventFiles = GetDataFilePaths("Data/Events");
            var events = new List<EventData>();

            foreach (var file in eventFiles)
            {
                var eventId = Path.GetFileNameWithoutExtension(file);
                var eventData = await LoadDataFileAsync<Dictionary<string, string>>(file);
                
                if (eventData != null)
                {
                    foreach (var kvp in eventData)
                    {
                        var evt = new EventData
                        {
                            Id = $"{eventId}_{kvp.Key}",
                            Name = kvp.Key,
                            Script = kvp.Value,
                            Type = DetermineEventType(eventId),
                            Location = DetermineEventLocation(eventId),
                            IsRepeatable = true
                        };
                        events.Add(evt);
                    }
                }
            }

            _events = new EventDataCollection(events);
            _loggingService.LogInformation($"成功加载 {events.Count} 个事件", "DataModelManager");
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"加载事件数据时发生错误: {ex.Message}", ex, "DataModelManager");
            _events = new EventDataCollection([]);
        }
    }

    /// <summary>
    /// 加载节日数据
    /// </summary>
    public async Task LoadFestivalsAsync()
    {
        try
        {
            // 智能获取文件路径
            var festivalFiles = GetDataFilePaths("Data/Festivals");
            var festivals = new List<FestivalData>();

            foreach (var file in festivalFiles)
            {
                var festivalId = Path.GetFileNameWithoutExtension(file);
                var festivalData = await LoadDataFileAsync<Dictionary<string, string>>(file);
                
                if (festivalData != null)
                {
                    var festival = new FestivalData
                    {
                        Id = festivalId,
                        Name = festivalData.TryGetValue("name", out var name) ? name ?? festivalId : festivalId,
                        Type = DetermineFestivalType(festivalId),
                        Date = DetermineFestivalDate(festivalId),
                        Location = "Town", // 默认位置
                        Dialogue = ExtractDialogueFromStringDict(festivalData),
                        IsYearly = true,
                        IsSkippable = false
                    };
                    festivals.Add(festival);
                }
            }

            _festivals = new FestivalDataCollection(festivals);
            _loggingService.LogInformation($"成功加载 {festivals.Count} 个节日", "DataModelManager");
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"加载节日数据时发生错误: {ex.Message}", ex, "DataModelManager");
            _festivals = new FestivalDataCollection([]);
        }
    }

    /// <summary>
    /// 加载语言数据
    /// </summary>
    public async Task LoadLanguagesAsync()
    {
        try
        {
            var languagesData = await LoadDataFileAsync<List<ModLanguage>>("Data/AdditionalLanguages");

            if (languagesData == null)
            {
                _loggingService.LogWarning("语言数据文件为空或加载失败", "DataModelManager");
                _languages = new LanguageDataCollection([]);
                return;
            }

            _languages = new LanguageDataCollection(languagesData);
            _loggingService.LogInformation($"成功加载 {languagesData.Count} 个语言", "DataModelManager");
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"加载语言数据时发生错误: {ex.Message}", ex, "DataModelManager");
            _languages = new LanguageDataCollection([]);
        }
    }

    #region 私有辅助方法

    /// <summary>
    /// 智能判断使用哪种加载模式
    /// </summary>
    /// <param name="relativePath">相对路径（不含扩展名）</param>
    /// <returns>true使用XNB模式，false使用JSON模式</returns>
    private bool ShouldUseXnbMode(string relativePath)
    {
        try
        {
            // 检查XNB文件是否存在
            var xnbPath = Path.Combine(_contentPath, $"{relativePath}.xnb");
            var xnbExists = File.Exists(xnbPath);

            // 检查JSON文件是否存在
            var jsonPath = Path.Combine(_contentPath, $"{relativePath}.json");
            var jsonExists = File.Exists(jsonPath);

            _loggingService.LogDebug($"文件存在检查 - XNB: {xnbExists}, JSON: {jsonExists}, 路径: {relativePath}", "DataModelManager");

            // 如果XNB文件存在且JSON文件不存在，使用XNB模式
            if (xnbExists && !jsonExists)
            {
                _loggingService.LogDebug($"选择XNB模式: {relativePath}", "DataModelManager");
                return true;
            }

            // 如果JSON文件存在，使用JSON模式
            if (jsonExists)
            {
                _loggingService.LogDebug($"选择JSON模式: {relativePath}", "DataModelManager");
                return false;
            }

            // 如果都不存在，默认尝试XNB模式
            _loggingService.LogDebug($"文件都不存在，默认选择XNB模式: {relativePath}", "DataModelManager");
            return true;
        }
        catch (Exception ex)
        {
            _loggingService.LogWarning($"判断加载模式时发生异常: {ex.Message}，默认选择XNB模式", "DataModelManager");
            return true;
        }
    }

    /// <summary>
    /// 智能加载数据文件
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="relativePath">相对路径（不含扩展名）</param>
    /// <returns>加载的数据，如果失败则返回null</returns>
    private async Task<T?> LoadDataFileAsync<T>(string relativePath)
    {
        try
        {
            // 智能判断使用哪种模式
            var useXnb = ShouldUseXnbMode(relativePath);

            if (useXnb && _xnbLoader != null)
            {
                // 使用XNB模式
                _loggingService.LogDebug($"使用XNB模式加载: {relativePath}", "DataModelManager");
                return _xnbLoader.LoadXnbFile<T>(relativePath);
            }
            else if (_jsonLoader != null)
            {
                // 使用JSON模式
                _loggingService.LogDebug($"使用JSON模式加载: {relativePath}", "DataModelManager");
                return await _jsonLoader.LoadJsonFileAsync<T>(relativePath);
            }
            else
            {
                _loggingService.LogError($"无法加载文件，缺少相应的加载器: {relativePath}", context: "DataModelManager");
                return default;
            }
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"加载数据文件失败: {relativePath}, 错误: {ex.Message}", ex, "DataModelManager");
            return default;
        }
    }

    /// <summary>
    /// 智能获取文件路径列表
    /// </summary>
    /// <param name="relativeDirectory">相对目录路径</param>
    /// <returns>文件路径列表（不含扩展名）</returns>
    private IEnumerable<string> GetDataFilePaths(string relativeDirectory)
    {
        try
        {
            var jsonPaths = new List<string>();
            var xnbPaths = new List<string>();

            // 获取JSON文件路径
            if (_jsonLoader != null)
            {
                jsonPaths = _jsonLoader.GetJsonFilePaths(relativeDirectory).ToList();
            }

            // 获取XNB文件路径
            if (_xnbLoader != null)
            {
                xnbPaths = _xnbLoader.GetXnbFilePaths(relativeDirectory).ToList();
            }

            // 合并路径，优先使用JSON文件
            var allPaths = new HashSet<string>(jsonPaths);
            allPaths.UnionWith(xnbPaths);

            _loggingService.LogDebug($"获取文件路径 - JSON: {jsonPaths.Count}, XNB: {xnbPaths.Count}, 总计: {allPaths.Count}, 目录: {relativeDirectory}", "DataModelManager");
            return allPaths;
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"获取文件路径失败: {relativeDirectory}, 错误: {ex.Message}", ex, "DataModelManager");
            return [];
        }
    }

    private static string ExtractAchievementName(string value)
    {
        var parts = value.Split('^');
        return parts.Length > 0 ? parts[0] : value;
    }

    private static string ExtractAchievementDescription(string value)
    {
        var parts = value.Split('^');
        return parts.Length > 1 ? parts[1] : string.Empty;
    }

    private static bool ExtractAchievementIsHidden(string value)
    {
        var parts = value.Split('^');
        return parts.Length > 2 && bool.TryParse(parts[2], out var isHidden) && isHidden;
    }

    private static int ExtractAchievementPrerequisite(string value)
    {
        var parts = value.Split('^');
        return parts.Length > 3 && int.TryParse(parts[3], out var prereq) ? prereq : -1;
    }

    private static int ExtractAchievementIcon(string value)
    {
        var parts = value.Split('^');
        return parts.Length > 4 && int.TryParse(parts[4], out var icon) ? icon : 0;
    }

    private static CharacterType DetermineCharacterType(string characterId)
    {
        var specialCharacters = new[] { "Wizard", "Dwarf", "Krobus", "Leo", "Marlon", "Morris", "MrQi", "TrashBear" };
        var animalCharacters = new[] { "Bear", "Crow", "Junimo", "IslandParrot", "Raccoon" };
        var monsterCharacters = new[] { "Monster" };

        if (specialCharacters.Contains(characterId))
            return CharacterType.Special;
        if (animalCharacters.Contains(characterId))
            return CharacterType.Animal;
        if (monsterCharacters.Contains(characterId))
            return CharacterType.Monster;

        return CharacterType.Villager;
    }

    private static bool IsMarriageableCharacter(string characterId)
    {
        var marriageableCharacters = new[]
        {
            "Abigail", "Alex", "Elliott", "Emily", "Haley", "Harvey", "Leah", "Maru", "Penny", "Sam", "Sebastian", "Shane"
        };

        return marriageableCharacters.Contains(characterId);
    }

    private static EventType DetermineEventType(string eventId)
    {
        if (eventId.Contains("Festival"))
            return EventType.Festival;
        if (eventId.Contains("Story"))
            return EventType.Story;
        if (eventId.Contains("Random"))
            return EventType.Random;

        return EventType.Character;
    }

    private static string DetermineEventLocation(string eventId)
    {
        if (eventId.Contains("Town"))
            return "Town";
        if (eventId.Contains("Farm"))
            return "Farm";
        if (eventId.Contains("Beach"))
            return "Beach";
        if (eventId.Contains("Mountain"))
            return "Mountain";

        return "Town";
    }

    private static FestivalType DetermineFestivalType(string festivalId)
    {
        if (festivalId.Contains("spring"))
            return FestivalType.Spring;
        if (festivalId.Contains("summer"))
            return FestivalType.Summer;
        if (festivalId.Contains("fall"))
            return FestivalType.Fall;
        if (festivalId.Contains("winter"))
            return FestivalType.Winter;

        return FestivalType.Special;
    }

    private static string DetermineFestivalDate(string festivalId)
    {
        // 这里需要根据实际的节日ID来确定日期
        // 暂时返回默认值
        return "spring-13";
    }

    /// <summary>
    /// 从字符串字典中提取对话数据
    /// </summary>
    /// <param name="data">字符串字典</param>
    /// <returns>对话字典</returns>
    private Dictionary<string, string> ExtractDialogueFromStringDict(Dictionary<string, string> data)
    {
        var dialogue = new Dictionary<string, string>();

        foreach (var kvp in data)
        {
            if (kvp.Key.StartsWith("dialogue", StringComparison.OrdinalIgnoreCase) ||
                kvp.Key.StartsWith("conversation", StringComparison.OrdinalIgnoreCase))
            {
                dialogue[kvp.Key] = kvp.Value;
            }
        }

        return dialogue;
    }
    #endregion
}

/// <summary>
/// 数据加载状态
/// </summary>
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class DataLoadingStatus
{
    /// <summary>
    /// 成就数据是否已加载
    /// </summary>
    public bool AchievementsLoaded { get; set; }

    /// <summary>
    /// 农场数据是否已加载
    /// </summary>
    public bool FarmsLoaded { get; set; }

    /// <summary>
    /// 角色数据是否已加载
    /// </summary>
    public bool CharactersLoaded { get; set; }

    /// <summary>
    /// 事件数据是否已加载
    /// </summary>
    public bool EventsLoaded { get; set; }

    /// <summary>
    /// 节日数据是否已加载
    /// </summary>
    public bool FestivalsLoaded { get; set; }

    /// <summary>
    /// 语言数据是否已加载
    /// </summary>
    public bool LanguagesLoaded { get; set; }

    /// <summary>
    /// 所有数据是否已加载
    /// </summary>
    public bool AllLoaded { get; set; }

    /// <summary>
    /// 获取已加载的数据类型数量
    /// </summary>
    public int LoadedCount => 
        (AchievementsLoaded ? 1 : 0) +
        (FarmsLoaded ? 1 : 0) +
        (CharactersLoaded ? 1 : 0) +
        (EventsLoaded ? 1 : 0) +
        (FestivalsLoaded ? 1 : 0) +
        (LanguagesLoaded ? 1 : 0);

    /// <summary>
    /// 获取总数据类型数量
    /// </summary>
    public int TotalCount => 6;

    /// <summary>
    /// 获取加载进度百分比
    /// </summary>
    public double LoadingProgress => (double)LoadedCount / TotalCount * 100;

    /// <summary>
    /// 返回加载状态的字符串表示
    /// </summary>
    public override string ToString()
    {
        return $"数据加载状态: {LoadedCount}/{TotalCount} ({LoadingProgress:F1}%) - " +
               $"成就:{AchievementsLoaded}, 农场:{FarmsLoaded}, 角色:{CharactersLoaded}, " +
               $"事件:{EventsLoaded}, 节日:{FestivalsLoaded}, 语言:{LanguagesLoaded}";
    }
}
