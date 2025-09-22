using ContentPatcherMaker.Core.Services.Logging;

namespace ContentPatcherMaker.Core.DataModels;

/// <summary>
/// 数据模型管理器
/// 负责管理所有数据模型的加载和访问
/// </summary>
public class DataModelManager
{
    private readonly JsonDataLoader _jsonLoader;
    private readonly LoggingService _loggingService;
    
    private AchievementDataCollection? _achievements;
    private FarmDataCollection? _farms;
    private CharacterDataCollection? _characters;
    private EventDataCollection? _events;
    private FestivalDataCollection? _festivals;
    private LanguageDataCollection? _languages;

    /// <summary>
    /// 初始化数据模型管理器
    /// </summary>
    /// <param name="jsonLoader">JSON加载器</param>
    /// <param name="loggingService">日志服务</param>
    public DataModelManager(JsonDataLoader jsonLoader, LoggingService loggingService)
    {
        _jsonLoader = jsonLoader ?? throw new ArgumentNullException(nameof(jsonLoader));
        _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
    }

    /// <summary>
    /// 成就数据集合
    /// </summary>
    public AchievementDataCollection Achievements => _achievements ?? throw new InvalidOperationException("成就数据未加载");

    /// <summary>
    /// 农场数据集合
    /// </summary>
    public FarmDataCollection Farms => _farms ?? throw new InvalidOperationException("农场数据未加载");

    /// <summary>
    /// 角色数据集合
    /// </summary>
    public CharacterDataCollection Characters => _characters ?? throw new InvalidOperationException("角色数据未加载");

    /// <summary>
    /// 事件数据集合
    /// </summary>
    public EventDataCollection Events => _events ?? throw new InvalidOperationException("事件数据未加载");

    /// <summary>
    /// 节日数据集合
    /// </summary>
    public FestivalDataCollection Festivals => _festivals ?? throw new InvalidOperationException("节日数据未加载");

    /// <summary>
    /// 语言数据集合
    /// </summary>
    public LanguageDataCollection Languages => _languages ?? throw new InvalidOperationException("语言数据未加载");

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
    /// 加载成就数据
    /// </summary>
    public async Task LoadAchievementsAsync()
    {
        try
        {
            var achievementsData = await _jsonLoader.LoadJsonFileAsync<Dictionary<string, string>>("Data/Achievements.json");
            if (achievementsData == null)
            {
                _loggingService.LogWarning("成就数据文件为空或加载失败", "DataModelManager");
                _achievements = new AchievementDataCollection(Enumerable.Empty<AchievementData>());
                return;
            }

            var achievements = achievementsData.Select(kvp => new AchievementData
            {
                Id = kvp.Key,
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
            _achievements = new AchievementDataCollection(Enumerable.Empty<AchievementData>());
        }
    }

    /// <summary>
    /// 加载农场数据
    /// </summary>
    public async Task LoadFarmsAsync()
    {
        try
        {
            var farmsData = await _jsonLoader.LoadJsonFileAsync<List<FarmData>>("Data/AdditionalFarms.json");
            if (farmsData == null)
            {
                _loggingService.LogWarning("农场数据文件为空或加载失败", "DataModelManager");
                _farms = new FarmDataCollection(Enumerable.Empty<FarmData>());
                return;
            }

            _farms = new FarmDataCollection(farmsData);
            _loggingService.LogInformation($"成功加载 {farmsData.Count} 个农场", "DataModelManager");
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"加载农场数据时发生错误: {ex.Message}", ex, "DataModelManager");
            _farms = new FarmDataCollection(Enumerable.Empty<FarmData>());
        }
    }

    /// <summary>
    /// 加载角色数据
    /// </summary>
    public async Task LoadCharactersAsync()
    {
        try
        {
            var scheduleFiles = _jsonLoader.GetJsonFilePaths("Characters/schedules");
            var characters = new List<CharacterData>();

            foreach (var file in scheduleFiles)
            {
                var characterId = Path.GetFileNameWithoutExtension(file);
                var schedules = await _jsonLoader.LoadJsonFileAsync<Dictionary<string, string>>(file);
                
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
            _characters = new CharacterDataCollection(Enumerable.Empty<CharacterData>());
        }
    }

    /// <summary>
    /// 加载事件数据
    /// </summary>
    public async Task LoadEventsAsync()
    {
        try
        {
            var eventFiles = _jsonLoader.GetJsonFilePaths("Data/Events");
            var events = new List<EventData>();

            foreach (var file in eventFiles)
            {
                var eventId = Path.GetFileNameWithoutExtension(file);
                var eventData = await _jsonLoader.LoadJsonFileAsync<Dictionary<string, string>>(file);
                
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
            _events = new EventDataCollection(Enumerable.Empty<EventData>());
        }
    }

    /// <summary>
    /// 加载节日数据
    /// </summary>
    public async Task LoadFestivalsAsync()
    {
        try
        {
            var festivalFiles = _jsonLoader.GetJsonFilePaths("Data/Festivals");
            var festivals = new List<FestivalData>();

            foreach (var file in festivalFiles)
            {
                var festivalId = Path.GetFileNameWithoutExtension(file);
                var festivalData = await _jsonLoader.LoadJsonFileAsync<Dictionary<string, object>>(file);
                
                if (festivalData != null)
                {
                    var festival = new FestivalData
                    {
                        Id = festivalId,
                        Name = festivalData.TryGetValue("name", out var name) ? name.ToString() ?? festivalId : festivalId,
                        Type = DetermineFestivalType(festivalId),
                        Date = DetermineFestivalDate(festivalId),
                        Location = "Town", // 默认位置
                        Dialogue = ExtractDialogue(festivalData),
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
            _festivals = new FestivalDataCollection(Enumerable.Empty<FestivalData>());
        }
    }

    /// <summary>
    /// 加载语言数据
    /// </summary>
    public async Task LoadLanguagesAsync()
    {
        try
        {
            var languagesData = await _jsonLoader.LoadJsonFileAsync<List<LanguageData>>("Data/AdditionalLanguages.json");
            if (languagesData == null)
            {
                _loggingService.LogWarning("语言数据文件为空或加载失败", "DataModelManager");
                _languages = new LanguageDataCollection(Enumerable.Empty<LanguageData>());
                return;
            }

            _languages = new LanguageDataCollection(languagesData);
            _loggingService.LogInformation($"成功加载 {languagesData.Count} 个语言", "DataModelManager");
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"加载语言数据时发生错误: {ex.Message}", ex, "DataModelManager");
            _languages = new LanguageDataCollection(Enumerable.Empty<LanguageData>());
        }
    }

    #region 私有辅助方法

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

    private static Dictionary<string, string> ExtractDialogue(Dictionary<string, object> festivalData)
    {
        var dialogue = new Dictionary<string, string>();
        
        foreach (var kvp in festivalData)
        {
            if (kvp.Value is string stringValue)
            {
                dialogue[kvp.Key] = stringValue;
            }
        }

        return dialogue;
    }

    #endregion
}
