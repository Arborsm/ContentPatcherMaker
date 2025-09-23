using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Newtonsoft.Json;
using StardewValley;

namespace ContentPatcherMaker.Core.DataModels.Datas;

/// <summary>
/// 对话情绪枚举
/// </summary>
public enum DialogueEmotion
{
    /// <summary>
    /// 中性
    /// </summary>
    [Display(Name = "中性", Description = "中性情绪")]
    Neutral,

    /// <summary>
    /// 开心
    /// </summary>
    [Display(Name = "开心", Description = "开心情绪")]
    Happy,

    /// <summary>
    /// 悲伤
    /// </summary>
    [Display(Name = "悲伤", Description = "悲伤情绪")]
    Sad,

    /// <summary>
    /// 独特
    /// </summary>
    [Display(Name = "独特", Description = "独特情绪")]
    Unique,

    /// <summary>
    /// 爱意
    /// </summary>
    [Display(Name = "爱意", Description = "爱意情绪")]
    Love,

    /// <summary>
    /// 愤怒
    /// </summary>
    [Display(Name = "愤怒", Description = "愤怒情绪")]
    Angry
}

/// <summary>
/// 对话命令类型枚举
/// </summary>
public enum DialogueCommandType
{
    /// <summary>
    /// 无命令
    /// </summary>
    [Display(Name = "无", Description = "无特殊命令")]
    None,

    /// <summary>
    /// 中断对话
    /// </summary>
    [Display(Name = "中断", Description = "中断当前对话")]
    Break,

    /// <summary>
    /// 结束对话
    /// </summary>
    [Display(Name = "结束", Description = "结束对话")]
    End,

    /// <summary>
    /// 杀死对话
    /// </summary>
    [Display(Name = "杀死", Description = "杀死对话")]
    Kill,

    /// <summary>
    /// 概率对话
    /// </summary>
    [Display(Name = "概率", Description = "基于概率的对话")]
    Chance,

    /// <summary>
    /// 条件对话
    /// </summary>
    [Display(Name = "条件", Description = "基于条件的对话")]
    Conditional,

    /// <summary>
    /// 事件对话
    /// </summary>
    [Display(Name = "事件", Description = "触发事件的对话")]
    Event,

    /// <summary>
    /// 快速响应
    /// </summary>
    [Display(Name = "快速响应", Description = "快速响应对话")]
    QuickResponse,

    /// <summary>
    /// 前置条件
    /// </summary>
    [Display(Name = "前置条件", Description = "前置条件对话")]
    Prerequisite,

    /// <summary>
    /// 单一对话
    /// </summary>
    [Display(Name = "单一", Description = "单一对话")]
    Single,

    /// <summary>
    /// 游戏状态查询
    /// </summary>
    [Display(Name = "状态查询", Description = "游戏状态查询")]
    GameStateQuery,

    /// <summary>
    /// 性别切换
    /// </summary>
    [Display(Name = "性别切换", Description = "基于性别的对话切换")]
    GenderSwitch,

    /// <summary>
    /// 执行动作
    /// </summary>
    [Display(Name = "执行动作", Description = "执行特定动作")]
    RunAction,

    /// <summary>
    /// 开始话题
    /// </summary>
    [Display(Name = "开始话题", Description = "开始对话话题")]
    StartConversationTopic,

    /// <summary>
    /// 问题
    /// </summary>
    [Display(Name = "问题", Description = "提出问题")]
    Question,

    /// <summary>
    /// 响应
    /// </summary>
    [Display(Name = "响应", Description = "对话响应")]
    Response
}

/// <summary>
/// 对话特殊字符类型枚举
/// </summary>
public enum DialogueSpecialCharacterType
{
    /// <summary>
    /// 无特殊字符
    /// </summary>
    [Display(Name = "无", Description = "无特殊字符")]
    None,

    /// <summary>
    /// 中断特殊字符
    /// </summary>
    [Display(Name = "中断", Description = "中断特殊字符")]
    BreakSpecialCharacter,

    /// <summary>
    /// 玩家名称特殊字符
    /// </summary>
    [Display(Name = "玩家名称", Description = "玩家名称特殊字符")]
    PlayerNameSpecialCharacter,

    /// <summary>
    /// 性别对话分割字符
    /// </summary>
    [Display(Name = "性别分割", Description = "性别对话分割字符")]
    GenderDialogueSplitCharacter,

    /// <summary>
    /// 性别对话分割字符2
    /// </summary>
    [Display(Name = "性别分割2", Description = "性别对话分割字符2")]
    GenderDialogueSplitCharacter2,

    /// <summary>
    /// 快速响应分隔符
    /// </summary>
    [Display(Name = "快速响应分隔符", Description = "快速响应分隔符")]
    QuickResponseDelineator,

    /// <summary>
    /// 无肖像前缀
    /// </summary>
    [Display(Name = "无肖像", Description = "无肖像前缀")]
    NoPortraitPrefix
}

/// <summary>
/// 对话特殊标记类型枚举
/// </summary>
public enum DialogueSpecialTokenType
{
    /// <summary>
    /// 无特殊标记
    /// </summary>
    [Display(Name = "无", Description = "无特殊标记")]
    None,

    /// <summary>
    /// 随机形容词
    /// </summary>
    [Display(Name = "随机形容词", Description = "随机形容词特殊标记")]
    RandomAdjective,

    /// <summary>
    /// 随机名词
    /// </summary>
    [Display(Name = "随机名词", Description = "随机名词特殊标记")]
    RandomNoun,

    /// <summary>
    /// 随机地点
    /// </summary>
    [Display(Name = "随机地点", Description = "随机地点特殊标记")]
    RandomPlace,

    /// <summary>
    /// 配偶
    /// </summary>
    [Display(Name = "配偶", Description = "配偶特殊标记")]
    Spouse,

    /// <summary>
    /// 随机名称
    /// </summary>
    [Display(Name = "随机名称", Description = "随机名称特殊标记")]
    RandomName,

    /// <summary>
    /// 名字首字母
    /// </summary>
    [Display(Name = "名字首字母", Description = "名字首字母特殊标记")]
    FirstNameLetters,

    /// <summary>
    /// 时间
    /// </summary>
    [Display(Name = "时间", Description = "时间特殊标记")]
    Time,

    /// <summary>
    /// 乐队名称
    /// </summary>
    [Display(Name = "乐队名称", Description = "乐队名称特殊标记")]
    BandName,

    /// <summary>
    /// 书籍名称
    /// </summary>
    [Display(Name = "书籍名称", Description = "书籍名称特殊标记")]
    BookName,

    /// <summary>
    /// 宠物
    /// </summary>
    [Display(Name = "宠物", Description = "宠物特殊标记")]
    Pet,

    /// <summary>
    /// 农场名称
    /// </summary>
    [Display(Name = "农场名称", Description = "农场名称特殊标记")]
    FarmName,

    /// <summary>
    /// 最爱物品
    /// </summary>
    [Display(Name = "最爱物品", Description = "最爱物品特殊标记")]
    FavoriteThing,

    /// <summary>
    /// 事件分支
    /// </summary>
    [Display(Name = "事件分支", Description = "事件分支特殊标记")]
    EventFork,

    /// <summary>
    /// 年份
    /// </summary>
    [Display(Name = "年份", Description = "年份特殊标记")]
    Year,

    /// <summary>
    /// 孩子1
    /// </summary>
    [Display(Name = "孩子1", Description = "孩子1特殊标记")]
    Kid1,

    /// <summary>
    /// 孩子2
    /// </summary>
    [Display(Name = "孩子2", Description = "孩子2特殊标记")]
    Kid2,

    /// <summary>
    /// 揭示品味
    /// </summary>
    [Display(Name = "揭示品味", Description = "揭示品味特殊标记")]
    RevealTaste,

    /// <summary>
    /// 季节
    /// </summary>
    [Display(Name = "季节", Description = "季节特殊标记")]
    Season,

    /// <summary>
    /// 不面向农民
    /// </summary>
    [Display(Name = "不面向农民", Description = "不面向农民特殊标记")]
    DontFaceFarmer
}

/// <summary>
/// 对话行数据模型
/// </summary>
public record DialogueLineData
{
    /// <summary>
    /// 对话文本
    /// </summary>
    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// 是否有文本
    /// </summary>
    [JsonIgnore]
    public bool HasText => !string.IsNullOrWhiteSpace(Text);

    /// <summary>
    /// 副作用动作（序列化时忽略）
    /// </summary>
    [JsonIgnore]
    public Action? SideEffects { get; set; }

    /// <summary>
    /// 情绪
    /// </summary>
    [JsonProperty("emotion")]
    public DialogueEmotion Emotion { get; set; } = DialogueEmotion.Neutral;

    /// <summary>
    /// 是否显示肖像
    /// </summary>
    [JsonProperty("showPortrait")]
    public bool ShowPortrait { get; set; } = true;

    /// <summary>
    /// 是否面向农民
    /// </summary>
    [JsonProperty("faceFarmer")]
    public bool FaceFarmer { get; set; } = true;

    /// <summary>
    /// 是否在下一屏继续
    /// </summary>
    [JsonProperty("continuedOnNextScreen")]
    public bool ContinuedOnNextScreen { get; set; }

    /// <summary>
    /// 命令类型
    /// </summary>
    [JsonProperty("commandType")]
    public DialogueCommandType CommandType { get; set; } = DialogueCommandType.None;

    /// <summary>
    /// 命令参数
    /// </summary>
    [JsonProperty("commandArgs")]
    public string? CommandArgs { get; set; }
}

/// <summary>
/// NPC对话响应数据模型
/// </summary>
public record NPCDialogueResponseData
{
    /// <summary>
    /// 响应文本
    /// </summary>
    [JsonProperty("responseText")]
    public string ResponseText { get; set; } = string.Empty;

    /// <summary>
    /// 友谊值变化
    /// </summary>
    [JsonProperty("friendshipChange")]
    public int FriendshipChange { get; set; }

    /// <summary>
    /// 响应键
    /// </summary>
    [JsonProperty("responseKey")]
    public string? ResponseKey { get; set; }

    /// <summary>
    /// 额外参数
    /// </summary>
    [JsonProperty("extraArgument")]
    public string? ExtraArgument { get; set; }

    /// <summary>
    /// 响应ID
    /// </summary>
    [JsonProperty("id")]
    public string? Id { get; set; }
}

/// <summary>
/// 对话数据模型
/// 基于Stardew Valley的Dialogue类
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Local")]
public record DialogueData
{
    /// <summary>
    /// 对话ID
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 说话者ID
    /// </summary>
    [JsonProperty("speakerId")]
    public string SpeakerId { get; set; } = string.Empty;

    /// <summary>
    /// 对话名称
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 对话描述
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 翻译键
    /// </summary>
    [JsonProperty("translationKey")]
    public string? TranslationKey { get; set; }

    /// <summary>
    /// 对话行列表
    /// </summary>
    [JsonProperty("dialogueLines")]
    public List<DialogueLineData> DialogueLines { get; set; } = new();

    /// <summary>
    /// 玩家响应选项
    /// </summary>
    [JsonProperty("playerResponses")]
    public List<NPCDialogueResponseData> PlayerResponses { get; set; } = new();

    /// <summary>
    /// 快速响应列表
    /// </summary>
    [JsonProperty("quickResponses")]
    public List<string> QuickResponses { get; set; } = new();

    /// <summary>
    /// 是否最后对话是交互式的
    /// </summary>
    [JsonProperty("isLastDialogueInteractive")]
    public bool IsLastDialogueInteractive { get; set; }

    /// <summary>
    /// 是否快速响应
    /// </summary>
    [JsonProperty("isQuickResponse")]
    public bool IsQuickResponse { get; set; }

    /// <summary>
    /// 是否完成最后对话
    /// </summary>
    [JsonProperty("finishedLastDialogue")]
    public bool FinishedLastDialogue { get; set; }

    /// <summary>
    /// 是否显示肖像
    /// </summary>
    [JsonProperty("showPortrait")]
    public bool ShowPortrait { get; set; } = true;

    /// <summary>
    /// 是否在下次移动时移除
    /// </summary>
    [JsonProperty("removeOnNextMove")]
    public bool RemoveOnNextMove { get; set; }

    /// <summary>
    /// 是否不面向农民
    /// </summary>
    [JsonProperty("dontFaceFarmer")]
    public bool DontFaceFarmer { get; set; }

    /// <summary>
    /// 临时对话键
    /// </summary>
    [JsonProperty("temporaryDialogueKey")]
    public string? TemporaryDialogueKey { get; set; }

    /// <summary>
    /// 当前对话索引
    /// </summary>
    [JsonProperty("currentDialogueIndex")]
    public int CurrentDialogueIndex { get; set; }

    /// <summary>
    /// 当前情绪
    /// </summary>
    [JsonProperty("currentEmotion")]
    public DialogueEmotion CurrentEmotion { get; set; } = DialogueEmotion.Neutral;

    /// <summary>
    /// 是否当前情绪被明确设置
    /// </summary>
    [JsonProperty("currentEmotionSetExplicitly")]
    public bool CurrentEmotionSetExplicitly { get; set; }

    /// <summary>
    /// 没有肖像的对话索引
    /// </summary>
    [JsonProperty("indexesWithoutPortrait")]
    public HashSet<int> IndexesWithoutPortrait { get; set; } = new();

    /// <summary>
    /// 是否当前字符串在下一屏继续
    /// </summary>
    [JsonProperty("isCurrentStringContinuedOnNextScreen")]
    public bool IsCurrentStringContinuedOnNextScreen { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    [JsonProperty("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 验证对话数据
    /// </summary>
    /// <returns>验证结果</returns>
    public ValidationResult Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Id))
            errors.Add("对话ID不能为空");

        if (string.IsNullOrWhiteSpace(SpeakerId))
            errors.Add("说话者ID不能为空");

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("对话名称不能为空");

        if (DialogueLines.Count == 0)
            errors.Add("对话行不能为空");

        // 验证对话行
        for (int i = 0; i < DialogueLines.Count; i++)
        {
            var line = DialogueLines[i];
            if (string.IsNullOrWhiteSpace(line.Text) && line.CommandType == DialogueCommandType.None)
            {
                errors.Add($"第{i + 1}行对话文本不能为空");
            }
        }

        // 验证当前对话索引
        if (CurrentDialogueIndex < 0 || CurrentDialogueIndex >= DialogueLines.Count)
        {
            errors.Add("当前对话索引超出范围");
        }

        return new ValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors
        };
    }

    /// <summary>
    /// 获取当前对话文本
    /// </summary>
    /// <returns>当前对话文本</returns>
    public string GetCurrentDialogue()
    {
        if (CurrentDialogueIndex >= DialogueLines.Count || FinishedLastDialogue)
            return string.Empty;

        return DialogueLines.Count <= 0 ? "..." : DialogueLines[CurrentDialogueIndex].Text;
    }

    /// <summary>
    /// 是否在最终对话
    /// </summary>
    /// <returns>是否在最终对话</returns>
    public bool IsOnFinalDialogue()
    {
        for (int i = CurrentDialogueIndex + 1; i < DialogueLines.Count; i++)
        {
            if (DialogueLines[i].HasText)
                return false;
        }
        return true;
    }

    /// <summary>
    /// 对话是否完成
    /// </summary>
    /// <returns>对话是否完成</returns>
    public bool IsDialogueFinished() => FinishedLastDialogue;

    /// <summary>
    /// 退出当前对话
    /// </summary>
    /// <returns>下一段对话文本</returns>
    public string? ExitCurrentDialogue()
    {
        if (IsOnFinalDialogue())
        {
            CurrentDialogueIndex++;
            FinishedLastDialogue = true;
        }

        bool wasContinued = IsCurrentStringContinuedOnNextScreen;
        if (CurrentDialogueIndex < DialogueLines.Count - 1)
        {
            CurrentDialogueIndex++;
            CheckForSpecialDialogueAttributes();
        }
        else
        {
            FinishedLastDialogue = true;
        }

        return wasContinued ? GetCurrentDialogue() : null;
    }

    /// <summary>
    /// 检查特殊对话属性
    /// </summary>
    private void CheckForSpecialDialogueAttributes()
    {
        CurrentEmotion = DialogueEmotion.Neutral;
        IsCurrentStringContinuedOnNextScreen = false;
        DontFaceFarmer = false;

        if (CurrentDialogueIndex >= DialogueLines.Count)
            return;

        var dialogue = DialogueLines[CurrentDialogueIndex];
        if (dialogue.Text.Contains("{"))
        {
            dialogue.Text = dialogue.Text.Replace("{", "");
            IsCurrentStringContinuedOnNextScreen = true;
        }

        if (dialogue.Text.Contains("%noturn"))
        {
            dialogue.Text = dialogue.Text.Replace("%noturn", "");
            DontFaceFarmer = true;
        }

        CheckEmotions();
    }

    /// <summary>
    /// 检查情绪
    /// </summary>
    private void CheckEmotions()
    {
        CurrentEmotion = DialogueEmotion.Neutral;
        if (CurrentDialogueIndex >= DialogueLines.Count)
            return;

        var dialogue = DialogueLines[CurrentDialogueIndex];
        string text = dialogue.Text;

        if (text.Contains(Dialogue.dialogueHappy))
        {
            CurrentEmotion = DialogueEmotion.Happy;
            dialogue.Text = text.Replace(Dialogue.dialogueHappy, "");
            CurrentEmotionSetExplicitly = true;
        }
        else if (text.Contains(Dialogue.dialogueSad))
        {
            CurrentEmotion = DialogueEmotion.Sad;
            dialogue.Text = text.Replace(Dialogue.dialogueSad, "");
            CurrentEmotionSetExplicitly = true;
        }
        else if (text.Contains(Dialogue.dialogueUnique))
        {
            CurrentEmotion = DialogueEmotion.Unique;
            dialogue.Text = text.Replace(Dialogue.dialogueUnique, "");
            CurrentEmotionSetExplicitly = true;
        }
        else if (text.Contains(Dialogue.dialogueLove))
        {
            CurrentEmotion = DialogueEmotion.Love;
            dialogue.Text = text.Replace(Dialogue.dialogueLove, "");
            CurrentEmotionSetExplicitly = true;
        }
        else if (text.Contains(Dialogue.dialogueAngry))
        {
            CurrentEmotion = DialogueEmotion.Angry;
            dialogue.Text = text.Replace(Dialogue.dialogueAngry, "");
            CurrentEmotionSetExplicitly = true;
        }
    }

    /// <summary>
    /// 添加对话行
    /// </summary>
    /// <param name="text">对话文本</param>
    /// <param name="emotion">情绪</param>
    /// <param name="commandType">命令类型</param>
    /// <param name="commandArgs">命令参数</param>
    public void AddDialogueLine(string text, DialogueEmotion emotion = DialogueEmotion.Neutral, 
        DialogueCommandType commandType = DialogueCommandType.None, string? commandArgs = null)
    {
        DialogueLines.Add(new DialogueLineData
        {
            Text = text,
            Emotion = emotion,
            CommandType = commandType,
            CommandArgs = commandArgs
        });
    }

    /// <summary>
    /// 添加玩家响应
    /// </summary>
    /// <param name="responseText">响应文本</param>
    /// <param name="friendshipChange">友谊值变化</param>
    /// <param name="responseKey">响应键</param>
    /// <param name="extraArgument">额外参数</param>
    /// <param name="id">响应ID</param>
    public void AddPlayerResponse(string responseText, int friendshipChange = 0, 
        string? responseKey = null, string? extraArgument = null, string? id = null)
    {
        PlayerResponses.Add(new NPCDialogueResponseData
        {
            ResponseText = responseText,
            FriendshipChange = friendshipChange,
            ResponseKey = responseKey,
            ExtraArgument = extraArgument,
            Id = id
        });
    }

    /// <summary>
    /// 添加快速响应
    /// </summary>
    /// <param name="response">快速响应文本</param>
    public void AddQuickResponse(string response)
    {
        QuickResponses.Add(response);
    }

    /// <summary>
    /// 获取肖像索引
    /// </summary>
    /// <returns>肖像索引</returns>
    public int GetPortraitIndex()
    {
        return CurrentEmotion switch
        {
            DialogueEmotion.Neutral => 0,
            DialogueEmotion.Happy => 1,
            DialogueEmotion.Sad => 2,
            DialogueEmotion.Unique => 3,
            DialogueEmotion.Love => 4,
            DialogueEmotion.Angry => 5,
            _ => 0
        };
    }

    /// <summary>
    /// 从Stardew Valley对话常量获取情绪
    /// </summary>
    /// <param name="emotionConstant">情绪常量</param>
    /// <returns>对应的情绪枚举</returns>
    public static DialogueEmotion GetEmotionFromConstant(string emotionConstant)
    {
        return emotionConstant switch
        {
            Dialogue.dialogueHappy => DialogueEmotion.Happy,
            Dialogue.dialogueSad => DialogueEmotion.Sad,
            Dialogue.dialogueUnique => DialogueEmotion.Unique,
            Dialogue.dialogueLove => DialogueEmotion.Love,
            Dialogue.dialogueAngry => DialogueEmotion.Angry,
            _ => DialogueEmotion.Neutral
        };
    }

    /// <summary>
    /// 从情绪枚举获取Stardew Valley对话常量
    /// </summary>
    /// <param name="emotion">情绪枚举</param>
    /// <returns>对应的对话常量</returns>
    public static string GetConstantFromEmotion(DialogueEmotion emotion)
    {
        return emotion switch
        {
            DialogueEmotion.Happy => Dialogue.dialogueHappy,
            DialogueEmotion.Sad => Dialogue.dialogueSad,
            DialogueEmotion.Unique => Dialogue.dialogueUnique,
            DialogueEmotion.Love => Dialogue.dialogueLove,
            DialogueEmotion.Angry => Dialogue.dialogueAngry,
            _ => Dialogue.dialogueNeutral
        };
    }

    /// <summary>
    /// 从Stardew Valley对话常量获取命令类型
    /// </summary>
    /// <param name="commandConstant">命令常量</param>
    /// <returns>对应的命令类型枚举</returns>
    public static DialogueCommandType GetCommandTypeFromConstant(string commandConstant)
    {
        return commandConstant switch
        {
            Dialogue.dialogueBreak => DialogueCommandType.Break,
            Dialogue.dialogueEnd => DialogueCommandType.End,
            Dialogue.dialogueKill => DialogueCommandType.Kill,
            Dialogue.dialogueChance => DialogueCommandType.Chance,
            Dialogue.dialogueDependingOnWorldState => DialogueCommandType.Conditional,
            Dialogue.dialogueEvent => DialogueCommandType.Event,
            Dialogue.dialogueQuickResponse => DialogueCommandType.QuickResponse,
            Dialogue.dialoguePrerequisite => DialogueCommandType.Prerequisite,
            Dialogue.dialogueSingle => DialogueCommandType.Single,
            Dialogue.dialogueGameStateQuery => DialogueCommandType.GameStateQuery,
            Dialogue.dialogueGenderSwitch_startBlock => DialogueCommandType.GenderSwitch,
            Dialogue.dialogueRunAction => DialogueCommandType.RunAction,
            Dialogue.dialogueStartConversationTopic => DialogueCommandType.StartConversationTopic,
            Dialogue.dialogueQuestion => DialogueCommandType.Question,
            Dialogue.dialogueResponse => DialogueCommandType.Response,
            _ => DialogueCommandType.None
        };
    }

    /// <summary>
    /// 从Stardew Valley特殊字符获取特殊字符类型
    /// </summary>
    /// <param name="character">特殊字符</param>
    /// <returns>对应的特殊字符类型枚举</returns>
    public static DialogueSpecialCharacterType GetSpecialCharacterTypeFromConstant(char character)
    {
        return character switch
        {
            Dialogue.dialogueCommandPrefix => DialogueSpecialCharacterType.None, // 命令前缀
            Dialogue.genderDialogueSplitCharacter => DialogueSpecialCharacterType.GenderDialogueSplitCharacter,
            Dialogue.genderDialogueSplitCharacter2 => DialogueSpecialCharacterType.GenderDialogueSplitCharacter2,
            Dialogue.noPortraitPrefix => DialogueSpecialCharacterType.NoPortraitPrefix,
            _ => DialogueSpecialCharacterType.None
        };
    }

    /// <summary>
    /// 从Stardew Valley特殊标记获取特殊标记类型
    /// </summary>
    /// <param name="token">特殊标记</param>
    /// <returns>对应的特殊标记类型枚举</returns>
    public static DialogueSpecialTokenType GetSpecialTokenTypeFromConstant(string token)
    {
        return token switch
        {
            Dialogue.randomAdjectiveSpecialCharacter => DialogueSpecialTokenType.RandomAdjective,
            Dialogue.randomNounSpecialCharacter => DialogueSpecialTokenType.RandomNoun,
            Dialogue.randomPlaceSpecialCharacter => DialogueSpecialTokenType.RandomPlace,
            Dialogue.spouseSpecialCharacter => DialogueSpecialTokenType.Spouse,
            Dialogue.randomNameSpecialCharacter => DialogueSpecialTokenType.RandomName,
            Dialogue.firstNameLettersSpecialCharacter => DialogueSpecialTokenType.FirstNameLetters,
            Dialogue.timeSpecialCharacter => DialogueSpecialTokenType.Time,
            Dialogue.bandNameSpecialCharacter => DialogueSpecialTokenType.BandName,
            Dialogue.bookNameSpecialCharacter => DialogueSpecialTokenType.BookName,
            Dialogue.petSpecialCharacter => DialogueSpecialTokenType.Pet,
            Dialogue.farmNameSpecialCharacter => DialogueSpecialTokenType.FarmName,
            Dialogue.favoriteThingSpecialCharacter => DialogueSpecialTokenType.FavoriteThing,
            Dialogue.eventForkSpecialCharacter => DialogueSpecialTokenType.EventFork,
            Dialogue.yearSpecialCharacter => DialogueSpecialTokenType.Year,
            Dialogue.kid1specialCharacter => DialogueSpecialTokenType.Kid1,
            Dialogue.kid2SpecialCharacter => DialogueSpecialTokenType.Kid2,
            Dialogue.revealTasteCharacter => DialogueSpecialTokenType.RevealTaste,
            Dialogue.seasonCharacter => DialogueSpecialTokenType.Season,
            Dialogue.dontfacefarmer => DialogueSpecialTokenType.DontFaceFarmer,
            _ => DialogueSpecialTokenType.None
        };
    }

    /// <summary>
    /// 获取所有Stardew Valley对话常量
    /// </summary>
    /// <returns>所有对话常量的字典</returns>
    public static Dictionary<string, string> GetAllDialogueConstants()
    {
        return new Dictionary<string, string>
        {
            // 情绪常量
            ["dialogueHappy"] = Dialogue.dialogueHappy,
            ["dialogueSad"] = Dialogue.dialogueSad,
            ["dialogueUnique"] = Dialogue.dialogueUnique,
            ["dialogueNeutral"] = Dialogue.dialogueNeutral,
            ["dialogueLove"] = Dialogue.dialogueLove,
            ["dialogueAngry"] = Dialogue.dialogueAngry,
            ["dialogueEnd"] = Dialogue.dialogueEnd,

            // 命令常量
            ["dialogueBreak"] = Dialogue.dialogueBreak,
            ["dialogueBreakDelimited"] = Dialogue.dialogueBreakDelimited,
            ["dialogueKill"] = Dialogue.dialogueKill,
            ["dialogueChance"] = Dialogue.dialogueChance,
            ["dialogueDependingOnWorldState"] = Dialogue.dialogueDependingOnWorldState,
            ["dialogueEvent"] = Dialogue.dialogueEvent,
            ["dialogueQuickResponse"] = Dialogue.dialogueQuickResponse,
            ["dialoguePrerequisite"] = Dialogue.dialoguePrerequisite,
            ["dialogueSingle"] = Dialogue.dialogueSingle,
            ["dialogueGameStateQuery"] = Dialogue.dialogueGameStateQuery,
            ["dialogueGenderSwitch_startBlock"] = Dialogue.dialogueGenderSwitch_startBlock,
            ["dialogueGenderSwitch_endBlock"] = Dialogue.dialogueGenderSwitch_endBlock,
            ["dialogueRunAction"] = Dialogue.dialogueRunAction,
            ["dialogueStartConversationTopic"] = Dialogue.dialogueStartConversationTopic,
            ["dialogueQuestion"] = Dialogue.dialogueQuestion,
            ["dialogueResponse"] = Dialogue.dialogueResponse,

            // 特殊字符常量
            ["breakSpecialCharacter"] = Dialogue.breakSpecialCharacter,
            ["playerNameSpecialCharacter"] = Dialogue.playerNameSpecialCharacter,
            ["quickResponseDelineator"] = Dialogue.quickResponseDelineator,
            ["multipleDialogueDelineator"] = Dialogue.multipleDialogueDelineator,

            // 特殊标记常量
            ["randomAdjectiveSpecialCharacter"] = Dialogue.randomAdjectiveSpecialCharacter,
            ["randomNounSpecialCharacter"] = Dialogue.randomNounSpecialCharacter,
            ["randomPlaceSpecialCharacter"] = Dialogue.randomPlaceSpecialCharacter,
            ["spouseSpecialCharacter"] = Dialogue.spouseSpecialCharacter,
            ["randomNameSpecialCharacter"] = Dialogue.randomNameSpecialCharacter,
            ["firstNameLettersSpecialCharacter"] = Dialogue.firstNameLettersSpecialCharacter,
            ["timeSpecialCharacter"] = Dialogue.timeSpecialCharacter,
            ["bandNameSpecialCharacter"] = Dialogue.bandNameSpecialCharacter,
            ["bookNameSpecialCharacter"] = Dialogue.bookNameSpecialCharacter,
            ["petSpecialCharacter"] = Dialogue.petSpecialCharacter,
            ["farmNameSpecialCharacter"] = Dialogue.farmNameSpecialCharacter,
            ["favoriteThingSpecialCharacter"] = Dialogue.favoriteThingSpecialCharacter,
            ["eventForkSpecialCharacter"] = Dialogue.eventForkSpecialCharacter,
            ["yearSpecialCharacter"] = Dialogue.yearSpecialCharacter,
            ["kid1specialCharacter"] = Dialogue.kid1specialCharacter,
            ["kid2SpecialCharacter"] = Dialogue.kid2SpecialCharacter,
            ["revealTasteCharacter"] = Dialogue.revealTasteCharacter,
            ["seasonCharacter"] = Dialogue.seasonCharacter,
            ["dontfacefarmer"] = Dialogue.dontfacefarmer,

            // 其他常量
            ["FallbackDialogueForErrorKey"] = Dialogue.FallbackDialogueForErrorKey
        };
    }

    /// <summary>
    /// 获取所有Stardew Valley特殊字符
    /// </summary>
    /// <returns>所有特殊字符的字典</returns>
    public static Dictionary<string, char> GetAllSpecialCharacters()
    {
        return new Dictionary<string, char>
        {
            ["dialogueCommandPrefix"] = Dialogue.dialogueCommandPrefix,
            ["genderDialogueSplitCharacter"] = Dialogue.genderDialogueSplitCharacter,
            ["genderDialogueSplitCharacter2"] = Dialogue.genderDialogueSplitCharacter2,
            ["noPortraitPrefix"] = Dialogue.noPortraitPrefix
        };
    }

    /// <summary>
    /// 获取所有Stardew Valley数组常量
    /// </summary>
    /// <returns>所有数组常量的字典</returns>
    public static Dictionary<string, string[]> GetAllArrayConstants()
    {
        return new Dictionary<string, string[]>
        {
            ["percentTokens"] = Dialogue.percentTokens,
            ["adjectives"] = Dialogue.adjectives,
            ["nouns"] = Dialogue.nouns,
            ["verbs"] = Dialogue.verbs,
            ["positional"] = Dialogue.positional,
            ["places"] = Dialogue.places,
            ["colors"] = Dialogue.colors
        };
    }

    /// <summary>
    /// 解析Stardew Valley对话字符串
    /// </summary>
    /// <param name="dialogueString">对话字符串</param>
    /// <param name="speakerId">说话者ID</param>
    /// <param name="translationKey">翻译键</param>
    /// <returns>解析后的对话数据</returns>
    public static DialogueData ParseDialogueString(string dialogueString, string speakerId, string? translationKey = null)
    {
        return DialogueParser.ParseDialogueString(dialogueString, speakerId, translationKey);
    }

    /// <summary>
    /// 将对话数据转换为Stardew Valley对话字符串
    /// </summary>
    /// <returns>Stardew Valley格式的对话字符串</returns>
    public string ToDialogueString()
    {
        var parts = new List<string>();

        foreach (var line in DialogueLines)
        {
            var lineText = line.Text;

            // 添加命令前缀
            if (line.CommandType != DialogueCommandType.None)
            {
                var commandConstant = GetConstantFromCommandType(line.CommandType);
                if (!string.IsNullOrEmpty(commandConstant))
                {
                    lineText = $"{commandConstant}{line.CommandArgs}{lineText}";
                }
            }

            // 添加情绪前缀
            if (line.Emotion != DialogueEmotion.Neutral)
            {
                var emotionConstant = GetConstantFromEmotion(line.Emotion);
                lineText = $"{emotionConstant}{lineText}";
            }

            parts.Add(lineText);
        }

        return string.Join("#", parts);
    }

    /// <summary>
    /// 从文本中提取情绪
    /// </summary>
    /// <param name="text">文本</param>
    /// <returns>情绪</returns>
    private static DialogueEmotion GetEmotionFromText(string text) => text switch
    {
        var t when t.Contains(Dialogue.dialogueHappy) => DialogueEmotion.Happy,
        var t when t.Contains(Dialogue.dialogueSad) => DialogueEmotion.Sad,
        var t when t.Contains(Dialogue.dialogueUnique) => DialogueEmotion.Unique,
        var t when t.Contains(Dialogue.dialogueLove) => DialogueEmotion.Love,
        var t when t.Contains(Dialogue.dialogueAngry) => DialogueEmotion.Angry,
        _ => DialogueEmotion.Neutral
    };

    /// <summary>
    /// 从文本中提取命令类型
    /// </summary>
    /// <param name="text">文本</param>
    /// <returns>命令类型</returns>
    private static DialogueCommandType GetCommandTypeFromText(string text) => text switch
    {
        var t when t.StartsWith(Dialogue.dialogueBreak) => DialogueCommandType.Break,
        var t when t.StartsWith(Dialogue.dialogueEnd) => DialogueCommandType.End,
        var t when t.StartsWith(Dialogue.dialogueKill) => DialogueCommandType.Kill,
        var t when t.StartsWith(Dialogue.dialogueChance) => DialogueCommandType.Chance,
        var t when t.StartsWith(Dialogue.dialogueDependingOnWorldState) => DialogueCommandType.Conditional,
        var t when t.StartsWith(Dialogue.dialogueEvent) => DialogueCommandType.Event,
        var t when t.StartsWith(Dialogue.dialogueQuickResponse) => DialogueCommandType.QuickResponse,
        var t when t.StartsWith(Dialogue.dialoguePrerequisite) => DialogueCommandType.Prerequisite,
        var t when t.StartsWith(Dialogue.dialogueSingle) => DialogueCommandType.Single,
        var t when t.StartsWith(Dialogue.dialogueGameStateQuery) => DialogueCommandType.GameStateQuery,
        var t when t.StartsWith(Dialogue.dialogueGenderSwitch_startBlock) => DialogueCommandType.GenderSwitch,
        var t when t.StartsWith(Dialogue.dialogueRunAction) => DialogueCommandType.RunAction,
        var t when t.StartsWith(Dialogue.dialogueStartConversationTopic) => DialogueCommandType.StartConversationTopic,
        var t when t.StartsWith(Dialogue.dialogueQuestion) => DialogueCommandType.Question,
        var t when t.StartsWith(Dialogue.dialogueResponse) => DialogueCommandType.Response,
        _ => DialogueCommandType.None
    };

    /// <summary>
    /// 从文本中提取命令参数
    /// </summary>
    /// <param name="text">文本</param>
    /// <param name="commandType">命令类型</param>
    /// <returns>命令参数</returns>
    private static string? ExtractCommandArgs(string text, DialogueCommandType commandType)
    {
        var commandConstant = GetConstantFromCommandType(commandType);
        if (string.IsNullOrEmpty(commandConstant) || !text.StartsWith(commandConstant))
            return null;

        var args = text[commandConstant.Length..];
        // 移除情绪前缀
        var emotionConstants = new[] { Dialogue.dialogueHappy, Dialogue.dialogueSad, Dialogue.dialogueUnique, Dialogue.dialogueLove, Dialogue.dialogueAngry };
        foreach (var emotionConstant in emotionConstants)
        {
            if (!args.StartsWith(emotionConstant)) continue;
            args = args[emotionConstant.Length..];
            break;
        }
        return args;
    }

    /// <summary>
    /// 从文本中清理命令
    /// </summary>
    /// <param name="text">文本</param>
    /// <returns>清理后的文本</returns>
    private static string CleanTextFromCommands(string text)
    {
        var cleanText = text;

        // 移除所有命令前缀
        var commands = new[] 
        { 
            Dialogue.dialogueBreak, Dialogue.dialogueEnd, Dialogue.dialogueKill, 
            Dialogue.dialogueChance, Dialogue.dialogueDependingOnWorldState, 
            Dialogue.dialogueEvent, Dialogue.dialogueQuickResponse, 
            Dialogue.dialoguePrerequisite, Dialogue.dialogueSingle, 
            Dialogue.dialogueGameStateQuery, Dialogue.dialogueGenderSwitch_startBlock, 
            Dialogue.dialogueRunAction, Dialogue.dialogueStartConversationTopic, 
            Dialogue.dialogueQuestion, Dialogue.dialogueResponse 
        };

        foreach (var command in commands)
        {
            if (cleanText.StartsWith(command))
            {
                cleanText = cleanText[command.Length..];
                break;
            }
        }

        // 移除情绪前缀
        var emotions = new[] { 
            Dialogue.dialogueHappy, Dialogue.dialogueSad, Dialogue.dialogueUnique, 
            Dialogue.dialogueLove, Dialogue.dialogueAngry, Dialogue.dialogueNeutral 
        };
        foreach (var emotion in emotions)
        {
            if (cleanText.StartsWith(emotion))
            {
                cleanText = cleanText[emotion.Length..];
                break;
            }
        }

        // 移除特殊标记
        var specialTokens = new[] 
        {
            Dialogue.randomAdjectiveSpecialCharacter, Dialogue.randomNounSpecialCharacter,
            Dialogue.randomPlaceSpecialCharacter, Dialogue.spouseSpecialCharacter,
            Dialogue.randomNameSpecialCharacter, Dialogue.firstNameLettersSpecialCharacter,
            Dialogue.timeSpecialCharacter, Dialogue.bandNameSpecialCharacter,
            Dialogue.bookNameSpecialCharacter, Dialogue.petSpecialCharacter,
            Dialogue.farmNameSpecialCharacter, Dialogue.favoriteThingSpecialCharacter,
            Dialogue.eventForkSpecialCharacter, Dialogue.yearSpecialCharacter,
            Dialogue.kid1specialCharacter, Dialogue.kid2SpecialCharacter,
            Dialogue.revealTasteCharacter, Dialogue.seasonCharacter,
            Dialogue.dontfacefarmer
        };

        foreach (var token in specialTokens)
        {
            cleanText = cleanText.Replace(token, "");
        }

        // 移除特殊字符
        var specialChars = new[] 
        {
            Dialogue.breakSpecialCharacter, Dialogue.playerNameSpecialCharacter,
            Dialogue.quickResponseDelineator, Dialogue.multipleDialogueDelineator
        };

        foreach (var specialChar in specialChars)
        {
            cleanText = cleanText.Replace(specialChar, "");
        }

        return cleanText.Trim();
    }

    /// <summary>
    /// 从命令类型获取常量
    /// </summary>
    /// <param name="commandType">命令类型</param>
    /// <returns>对应的常量</returns>
    private static string GetConstantFromCommandType(DialogueCommandType commandType)
    {
        return commandType switch
        {
            DialogueCommandType.Break => Dialogue.dialogueBreak,
            DialogueCommandType.End => Dialogue.dialogueEnd,
            DialogueCommandType.Kill => Dialogue.dialogueKill,
            DialogueCommandType.Chance => Dialogue.dialogueChance,
            DialogueCommandType.Conditional => Dialogue.dialogueDependingOnWorldState,
            DialogueCommandType.Event => Dialogue.dialogueEvent,
            DialogueCommandType.QuickResponse => Dialogue.dialogueQuickResponse,
            DialogueCommandType.Prerequisite => Dialogue.dialoguePrerequisite,
            DialogueCommandType.Single => Dialogue.dialogueSingle,
            DialogueCommandType.GameStateQuery => Dialogue.dialogueGameStateQuery,
            DialogueCommandType.GenderSwitch => Dialogue.dialogueGenderSwitch_startBlock,
            DialogueCommandType.RunAction => Dialogue.dialogueRunAction,
            DialogueCommandType.StartConversationTopic => Dialogue.dialogueStartConversationTopic,
            DialogueCommandType.Question => Dialogue.dialogueQuestion,
            DialogueCommandType.Response => Dialogue.dialogueResponse,
            _ => ""
        };
    }

    /// <summary>
    /// 是否当前对话是问题
    /// </summary>
    /// <returns>是否当前对话是问题</returns>
    public bool IsCurrentDialogueAQuestion()
    {
        return IsLastDialogueInteractive && CurrentDialogueIndex == DialogueLines.Count - 1;
    }

    /// <summary>
    /// 获取NPC响应选项
    /// </summary>
    /// <returns>NPC响应选项列表</returns>
    public List<NPCDialogueResponseData> GetNPCResponseOptions() => PlayerResponses;

    /// <summary>
    /// 克隆对话数据
    /// </summary>
    /// <returns>克隆的对话数据</returns>
    public DialogueData CloneData()
    {
        return new DialogueData
        {
            Id = Id,
            SpeakerId = SpeakerId,
            Name = Name,
            Description = Description,
            TranslationKey = TranslationKey,
            DialogueLines = DialogueLines.Select(dl => new DialogueLineData
            {
                Text = dl.Text,
                Emotion = dl.Emotion,
                ShowPortrait = dl.ShowPortrait,
                FaceFarmer = dl.FaceFarmer,
                ContinuedOnNextScreen = dl.ContinuedOnNextScreen,
                CommandType = dl.CommandType,
                CommandArgs = dl.CommandArgs
            }).ToList(),
            PlayerResponses = PlayerResponses.Select(pr => new NPCDialogueResponseData
            {
                ResponseText = pr.ResponseText,
                FriendshipChange = pr.FriendshipChange,
                ResponseKey = pr.ResponseKey,
                ExtraArgument = pr.ExtraArgument,
                Id = pr.Id
            }).ToList(),
            QuickResponses = [..QuickResponses],
            IsLastDialogueInteractive = IsLastDialogueInteractive,
            IsQuickResponse = IsQuickResponse,
            FinishedLastDialogue = FinishedLastDialogue,
            ShowPortrait = ShowPortrait,
            RemoveOnNextMove = RemoveOnNextMove,
            DontFaceFarmer = DontFaceFarmer,
            TemporaryDialogueKey = TemporaryDialogueKey,
            CurrentDialogueIndex = CurrentDialogueIndex,
            CurrentEmotion = CurrentEmotion,
            CurrentEmotionSetExplicitly = CurrentEmotionSetExplicitly,
            IndexesWithoutPortrait = [..IndexesWithoutPortrait],
            IsCurrentStringContinuedOnNextScreen = IsCurrentStringContinuedOnNextScreen,
            CreatedAt = CreatedAt,
            UpdatedAt = DateTime.Now
        };
    }
}

/// <summary>
/// 对话数据集合
/// </summary>
public class DialogueDataCollection : DataModelCollection<DialogueData>
{
    private readonly Dictionary<string, DialogueData> _dialogues = new();

    /// <summary>
    /// 初始化对话数据集合
    /// </summary>
    /// <param name="dialogues">对话数据列表</param>
    public DialogueDataCollection(IEnumerable<DialogueData> dialogues)
    {
        foreach (var dialogue in dialogues)
        {
            if (!string.IsNullOrWhiteSpace(dialogue.Id))
            {
                _dialogues[dialogue.Id] = dialogue;
            }
        }
    }

    /// <summary>
    /// 获取所有对话
    /// </summary>
    public override IEnumerable<DialogueData> GetAll() => _dialogues.Values;

    /// <summary>
    /// 根据ID获取对话
    /// </summary>
    /// <param name="id">对话ID</param>
    /// <returns>对话数据，如果不存在则返回null</returns>
    public override DialogueData? GetById(string id) => _dialogues.GetValueOrDefault(id);

    /// <summary>
    /// 检查对话是否存在
    /// </summary>
    /// <param name="id">对话ID</param>
    /// <returns>是否存在</returns>
    public override bool Exists(string id) => _dialogues.ContainsKey(id);

    /// <summary>
    /// 搜索对话
    /// </summary>
    /// <param name="predicate">搜索条件</param>
    /// <returns>匹配的对话列表</returns>
    public override IEnumerable<DialogueData> Search(Func<DialogueData, bool> predicate) => _dialogues.Values.Where(predicate);

    /// <summary>
    /// 根据说话者获取对话
    /// </summary>
    /// <param name="speakerId">说话者ID</param>
    /// <returns>指定说话者的对话列表</returns>
    public IEnumerable<DialogueData> GetDialoguesBySpeaker(string speakerId) => 
        _dialogues.Values.Where(d => d.SpeakerId == speakerId);

    /// <summary>
    /// 获取交互式对话
    /// </summary>
    /// <returns>交互式对话列表</returns>
    public IEnumerable<DialogueData> GetInteractiveDialogues() => 
        _dialogues.Values.Where(d => d.IsLastDialogueInteractive);

    /// <summary>
    /// 获取快速响应对话
    /// </summary>
    /// <returns>快速响应对话列表</returns>
    public IEnumerable<DialogueData> GetQuickResponseDialogues() => 
        _dialogues.Values.Where(d => d.IsQuickResponse);

    /// <summary>
    /// 获取包含特定情绪的对话
    /// </summary>
    /// <param name="emotion">情绪</param>
    /// <returns>包含指定情绪的对话列表</returns>
    public IEnumerable<DialogueData> GetDialoguesByEmotion(DialogueEmotion emotion) => 
        _dialogues.Values.Where(d => d.CurrentEmotion == emotion || d.DialogueLines.Any(dl => dl.Emotion == emotion));

    /// <summary>
    /// 获取包含特定命令的对话
    /// </summary>
    /// <param name="commandType">命令类型</param>
    /// <returns>包含指定命令的对话列表</returns>
    public IEnumerable<DialogueData> GetDialoguesByCommandType(DialogueCommandType commandType) => 
        _dialogues.Values.Where(d => d.DialogueLines.Any(dl => dl.CommandType == commandType));

    /// <summary>
    /// 搜索包含特定文本的对话
    /// </summary>
    /// <param name="text">搜索文本</param>
    /// <returns>包含指定文本的对话列表</returns>
    public IEnumerable<DialogueData> SearchByText(string text) => 
        _dialogues.Values.Where(d => d.DialogueLines.Any(dl => dl.Text.Contains(text, StringComparison.OrdinalIgnoreCase)));

    /// <summary>
    /// 获取最近创建的对话
    /// </summary>
    /// <param name="count">数量</param>
    /// <returns>最近创建的对话列表</returns>
    public IEnumerable<DialogueData> GetRecentDialogues(int count = 10) => 
        _dialogues.Values.OrderByDescending(d => d.CreatedAt).Take(count);

    /// <summary>
    /// 获取最近更新的对话
    /// </summary>
    /// <param name="count">数量</param>
    /// <returns>最近更新的对话列表</returns>
    public IEnumerable<DialogueData> GetRecentlyUpdatedDialogues(int count = 10) => 
        _dialogues.Values.OrderByDescending(d => d.UpdatedAt).Take(count);

    /// <summary>
    /// 添加对话
    /// </summary>
    /// <param name="dialogue">对话数据</param>
    public void AddDialogue(DialogueData dialogue)
    {
        if (!string.IsNullOrWhiteSpace(dialogue.Id))
        {
            dialogue.UpdatedAt = DateTime.Now;
            _dialogues[dialogue.Id] = dialogue;
        }
    }

    /// <summary>
    /// 更新对话
    /// </summary>
    /// <param name="dialogue">对话数据</param>
    public void UpdateDialogue(DialogueData dialogue)
    {
        if (_dialogues.ContainsKey(dialogue.Id))
        {
            dialogue.UpdatedAt = DateTime.Now;
            _dialogues[dialogue.Id] = dialogue;
        }
    }

    /// <summary>
    /// 删除对话
    /// </summary>
    /// <param name="id">对话ID</param>
    /// <returns>是否删除成功</returns>
    public bool RemoveDialogue(string id)
    {
        return _dialogues.Remove(id);
    }

    /// <summary>
    /// 清空所有对话
    /// </summary>
    public void Clear()
    {
        _dialogues.Clear();
    }

    /// <summary>
    /// 获取对话数量
    /// </summary>
    /// <returns>对话数量</returns>
    public int Count => _dialogues.Count;
}

/// <summary>
/// 对话构建器
/// 用于构建复杂的对话条件
/// </summary>
public class DialogueBuilder
{
    private readonly DialogueData _dialogue;

    /// <summary>
    /// 初始化对话构建器
    /// </summary>
    /// <param name="speakerId">说话者ID</param>
    /// <param name="name">对话名称</param>
    public DialogueBuilder(string speakerId, string name)
    {
        _dialogue = new DialogueData
        {
            Id = Guid.NewGuid().ToString(),
            SpeakerId = speakerId,
            Name = name
        };
    }

    /// <summary>
    /// 添加简单对话行
    /// </summary>
    /// <param name="text">对话文本</param>
    /// <param name="emotion">情绪</param>
    /// <returns>构建器实例</returns>
    public DialogueBuilder AddLine(string text, DialogueEmotion emotion = DialogueEmotion.Neutral)
    {
        _dialogue.AddDialogueLine(text, emotion);
        return this;
    }

    /// <summary>
    /// 添加条件对话
    /// </summary>
    /// <param name="condition">条件</param>
    /// <param name="trueText">条件为真时的文本</param>
    /// <param name="falseText">条件为假时的文本</param>
    /// <param name="emotion">情绪</param>
    /// <returns>构建器实例</returns>
    public DialogueBuilder AddConditionalLine(DialogueCondition condition, string trueText, string falseText, DialogueEmotion emotion = DialogueEmotion.Neutral)
    {
        // 添加条件对话
        _dialogue.AddDialogueLine($"{Dialogue.dialogueDependingOnWorldState} {condition.ConditionKey} {trueText}|{falseText}", emotion, DialogueCommandType.Conditional, condition.ConditionKey);
        
        return this;
    }

    /// <summary>
    /// 添加概率对话
    /// </summary>
    /// <param name="text">对话文本</param>
    /// <param name="probability">概率（0.0-1.0）</param>
    /// <param name="emotion">情绪</param>
    /// <returns>构建器实例</returns>
    public DialogueBuilder AddChanceLine(string text, double probability, DialogueEmotion emotion = DialogueEmotion.Neutral)
    {
        _dialogue.AddDialogueLine($"{Dialogue.dialogueChance}{probability} {text}", emotion, DialogueCommandType.Chance, probability.ToString(CultureInfo.InvariantCulture));
        return this;
    }

    /// <summary>
    /// 添加事件触发对话
    /// </summary>
    /// <param name="eventId">事件ID</param>
    /// <param name="emotion">情绪</param>
    /// <returns>构建器实例</returns>
    public DialogueBuilder AddEventLine(string eventId, DialogueEmotion emotion = DialogueEmotion.Neutral)
    {
        _dialogue.AddDialogueLine($"{Dialogue.dialogueEvent} {eventId}", emotion, DialogueCommandType.Event, eventId);
        return this;
    }

    /// <summary>
    /// 添加问题对话
    /// </summary>
    /// <param name="questionId">问题ID</param>
    /// <param name="text">问题文本</param>
    /// <param name="emotion">情绪</param>
    /// <returns>构建器实例</returns>
    public DialogueBuilder AddQuestionLine(string questionId, string text, DialogueEmotion emotion = DialogueEmotion.Neutral)
    {
        _dialogue.AddDialogueLine($"{Dialogue.dialogueQuestion} {questionId} {text}", emotion, DialogueCommandType.Question, questionId);
        _dialogue.IsLastDialogueInteractive = true;
        return this;
    }

    /// <summary>
    /// 添加玩家响应
    /// </summary>
    /// <param name="responseText">响应文本</param>
    /// <param name="friendshipChange">友谊值变化</param>
    /// <param name="responseKey">响应键</param>
    /// <param name="extraArgument">额外参数</param>
    /// <param name="id">响应ID</param>
    /// <returns>构建器实例</returns>
    public DialogueBuilder AddPlayerResponse(string responseText, int friendshipChange = 0, string? responseKey = null, string? extraArgument = null, string? id = null)
    {
        _dialogue.AddPlayerResponse(responseText, friendshipChange, responseKey, extraArgument, id);
        return this;
    }

    /// <summary>
    /// 添加快速响应
    /// </summary>
    /// <param name="response">快速响应文本</param>
    /// <returns>构建器实例</returns>
    public DialogueBuilder AddQuickResponse(string response)
    {
        _dialogue.AddQuickResponse(response);
        _dialogue.IsQuickResponse = true;
        return this;
    }

    /// <summary>
    /// 添加中断对话
    /// </summary>
    /// <returns>构建器实例</returns>
    public DialogueBuilder AddBreak()
    {
        _dialogue.AddDialogueLine(Dialogue.dialogueBreak, DialogueEmotion.Neutral, DialogueCommandType.Break);
        return this;
    }

    /// <summary>
    /// 添加结束对话
    /// </summary>
    /// <returns>构建器实例</returns>
    public DialogueBuilder AddEnd()
    {
        _dialogue.AddDialogueLine(Dialogue.dialogueEnd, DialogueEmotion.Neutral, DialogueCommandType.End);
        return this;
    }

    /// <summary>
    /// 设置对话属性
    /// </summary>
    /// <param name="showPortrait">是否显示肖像</param>
    /// <param name="faceFarmer">是否面向农民</param>
    /// <param name="removeOnNextMove">是否在下次移动时移除</param>
    /// <returns>构建器实例</returns>
    public DialogueBuilder SetProperties(bool showPortrait = true, bool faceFarmer = true, bool removeOnNextMove = false)
    {
        _dialogue.ShowPortrait = showPortrait;
        _dialogue.DontFaceFarmer = !faceFarmer;
        _dialogue.RemoveOnNextMove = removeOnNextMove;
        return this;
    }

    /// <summary>
    /// 构建对话数据
    /// </summary>
    /// <returns>构建的对话数据</returns>
    public DialogueData Build()
    {
        return _dialogue.CloneData();
    }

    /// <summary>
    /// 构建并添加到集合
    /// </summary>
    /// <param name="collection">对话集合</param>
    /// <returns>构建的对话数据</returns>
    public DialogueData BuildAndAdd(DialogueDataCollection collection)
    {
        var dialogue = _dialogue.CloneData();
        collection.AddDialogue(dialogue);
        return dialogue;
    }
}

/// <summary>
/// 对话条件
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class DialogueCondition
{
    /// <summary>
    /// 条件键
    /// </summary>
    public string ConditionKey { get; init; } = string.Empty;

    /// <summary>
    /// 条件描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 条件类型
    /// </summary>
    public ConditionType Type { get; set; }
}

/// <summary>
/// 条件类型
/// </summary>
public enum ConditionType
{
    /// <summary>
    /// 社区中心
    /// </summary>
    CommunityCenter,

    /// <summary>
    /// Joja超市
    /// </summary>
    JojaMart,

    /// <summary>
    /// 公交车
    /// </summary>
    Bus,

    /// <summary>
    /// 肯特
    /// </summary>
    Kent,

    /// <summary>
    /// 自定义条件
    /// </summary>
    Custom
}

/// <summary>
/// 对话解析器
/// 处理与Stardew Valley Dialogue类的交互
/// </summary>
public static class DialogueParser
{
    /// <summary>
    /// 解析Stardew Valley对话字符串
    /// </summary>
    /// <param name="dialogueString">对话字符串</param>
    /// <param name="speakerId">说话者ID</param>
    /// <param name="translationKey">翻译键</param>
    /// <returns>解析后的对话数据</returns>
    public static DialogueData ParseDialogueString(string dialogueString, string speakerId, string? translationKey = null)
    {
        var dialogue = new DialogueData
        {
            SpeakerId = speakerId,
            TranslationKey = translationKey,
            Id = Guid.NewGuid().ToString()
        };

        // 使用Stardew Valley的解析逻辑
        var stardewDialogue = new Dialogue(null, translationKey ?? "", dialogueString);
        
        // 转换Stardew Valley的对话行到我们的格式
        foreach (var stardewLine in stardewDialogue.dialogues)
        {
            var emotion = GetEmotionFromText(stardewLine.Text);
            var commandType = GetCommandTypeFromText(stardewLine.Text);
            var commandArgs = ExtractCommandArgs(stardewLine.Text, commandType);
            var cleanText = CleanTextFromCommands(stardewLine.Text);

            dialogue.AddDialogueLine(cleanText, emotion, commandType, commandArgs);
        }

        // 转换玩家响应 - 使用公共方法
        var playerResponses = stardewDialogue.getNPCResponseOptions();
        if (playerResponses != null)
        {
            foreach (var response in playerResponses)
            {
                dialogue.AddPlayerResponse(
                    response.responseText,
                    response.friendshipChange,
                    response.responseKey,
                    response.extraArgument,
                    response.id
                );
            }
        }

        // 转换快速响应 - 通过反射获取私有字段
        try
        {
            var quickResponsesField = typeof(Dialogue).GetField("quickResponses", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (quickResponsesField?.GetValue(stardewDialogue) is List<string> quickResponses)
            {
                foreach (var quickResponse in quickResponses)
                {
                    dialogue.AddQuickResponse(quickResponse);
                }
            }
        }
        catch
        {
            // 如果无法访问私有字段，跳过快速响应
        }

        // 复制公共属性
        dialogue.ShowPortrait = stardewDialogue.showPortrait;
        dialogue.RemoveOnNextMove = stardewDialogue.removeOnNextMove;
        dialogue.DontFaceFarmer = stardewDialogue.dontFaceFarmer;
        dialogue.TemporaryDialogueKey = stardewDialogue.temporaryDialogueKey;
        dialogue.CurrentDialogueIndex = stardewDialogue.currentDialogueIndex;
        dialogue.CurrentEmotion = DialogueData.GetEmotionFromConstant(stardewDialogue.CurrentEmotion);
        dialogue.CurrentEmotionSetExplicitly = stardewDialogue.CurrentEmotionSetExplicitly;
        dialogue.IsCurrentStringContinuedOnNextScreen = stardewDialogue.isCurrentStringContinuedOnNextScreen;

        // 通过反射获取私有字段
        SetPrivateFields(dialogue, stardewDialogue);

        return dialogue;
    }

    /// <summary>
    /// 设置私有字段
    /// </summary>
    /// <param name="dialogue">对话数据</param>
    /// <param name="stardewDialogue">Stardew Valley对话</param>
    private static void SetPrivateFields(DialogueData dialogue, Dialogue stardewDialogue)
    {
        try
        {
            var isLastDialogueInteractiveField = typeof(Dialogue).GetField("isLastDialogueInteractive", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (isLastDialogueInteractiveField?.GetValue(stardewDialogue) is bool isLastDialogueInteractive)
            {
                dialogue.IsLastDialogueInteractive = isLastDialogueInteractive;
            }

            var quickResponseField = typeof(Dialogue).GetField("quickResponse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (quickResponseField?.GetValue(stardewDialogue) is bool quickResponse)
            {
                dialogue.IsQuickResponse = quickResponse;
            }

            var finishedLastDialogueField = typeof(Dialogue).GetField("finishedLastDialogue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (finishedLastDialogueField?.GetValue(stardewDialogue) is bool finishedLastDialogue)
            {
                dialogue.FinishedLastDialogue = finishedLastDialogue;
            }

            var indexesWithoutPortraitField = typeof(Dialogue).GetField("indexesWithoutPortrait", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (indexesWithoutPortraitField?.GetValue(stardewDialogue) is HashSet<int> indexesWithoutPortrait)
            {
                dialogue.IndexesWithoutPortrait = new HashSet<int>(indexesWithoutPortrait);
            }
        }
        catch
        {
            // 如果无法访问私有字段，使用默认值
        }
    }

    /// <summary>
    /// 从文本中提取情绪
    /// </summary>
    /// <param name="text">文本</param>
    /// <returns>情绪</returns>
    private static DialogueEmotion GetEmotionFromText(string text) => text switch
    {
        var t when t.Contains(Dialogue.dialogueHappy) => DialogueEmotion.Happy,
        var t when t.Contains(Dialogue.dialogueSad) => DialogueEmotion.Sad,
        var t when t.Contains(Dialogue.dialogueUnique) => DialogueEmotion.Unique,
        var t when t.Contains(Dialogue.dialogueLove) => DialogueEmotion.Love,
        var t when t.Contains(Dialogue.dialogueAngry) => DialogueEmotion.Angry,
        _ => DialogueEmotion.Neutral
    };

    /// <summary>
    /// 从文本中提取命令类型
    /// </summary>
    /// <param name="text">文本</param>
    /// <returns>命令类型</returns>
    private static DialogueCommandType GetCommandTypeFromText(string text) => text switch
    {
        var t when t.StartsWith(Dialogue.dialogueBreak) => DialogueCommandType.Break,
        var t when t.StartsWith(Dialogue.dialogueEnd) => DialogueCommandType.End,
        var t when t.StartsWith(Dialogue.dialogueKill) => DialogueCommandType.Kill,
        var t when t.StartsWith(Dialogue.dialogueChance) => DialogueCommandType.Chance,
        var t when t.StartsWith(Dialogue.dialogueDependingOnWorldState) => DialogueCommandType.Conditional,
        var t when t.StartsWith(Dialogue.dialogueEvent) => DialogueCommandType.Event,
        var t when t.StartsWith(Dialogue.dialogueQuickResponse) => DialogueCommandType.QuickResponse,
        var t when t.StartsWith(Dialogue.dialoguePrerequisite) => DialogueCommandType.Prerequisite,
        var t when t.StartsWith(Dialogue.dialogueSingle) => DialogueCommandType.Single,
        var t when t.StartsWith(Dialogue.dialogueGameStateQuery) => DialogueCommandType.GameStateQuery,
        var t when t.StartsWith(Dialogue.dialogueGenderSwitch_startBlock) => DialogueCommandType.GenderSwitch,
        var t when t.StartsWith(Dialogue.dialogueRunAction) => DialogueCommandType.RunAction,
        var t when t.StartsWith(Dialogue.dialogueStartConversationTopic) => DialogueCommandType.StartConversationTopic,
        var t when t.StartsWith(Dialogue.dialogueQuestion) => DialogueCommandType.Question,
        var t when t.StartsWith(Dialogue.dialogueResponse) => DialogueCommandType.Response,
        _ => DialogueCommandType.None
    };

    /// <summary>
    /// 从文本中提取命令参数
    /// </summary>
    /// <param name="text">文本</param>
    /// <param name="commandType">命令类型</param>
    /// <returns>命令参数</returns>
    private static string? ExtractCommandArgs(string text, DialogueCommandType commandType)
    {
        var commandConstant = GetConstantFromCommandType(commandType);
        if (string.IsNullOrEmpty(commandConstant) || !text.StartsWith(commandConstant))
            return null;

        var args = text[commandConstant.Length..];
        // 移除情绪前缀
        var emotionConstants = new[] { Dialogue.dialogueHappy, Dialogue.dialogueSad, Dialogue.dialogueUnique, Dialogue.dialogueLove, Dialogue.dialogueAngry };
        foreach (var emotionConstant in emotionConstants)
        {
            if (args.StartsWith(emotionConstant))
            {
                args = args[emotionConstant.Length..];
                break;
            }
        }
        return args;
    }

    /// <summary>
    /// 从文本中清理命令
    /// </summary>
    /// <param name="text">文本</param>
    /// <returns>清理后的文本</returns>
    private static string CleanTextFromCommands(string text)
    {
        var cleanText = text;

        // 移除所有命令前缀
        var commands = new[] 
        { 
            Dialogue.dialogueBreak, Dialogue.dialogueEnd, Dialogue.dialogueKill, 
            Dialogue.dialogueChance, Dialogue.dialogueDependingOnWorldState, 
            Dialogue.dialogueEvent, Dialogue.dialogueQuickResponse, 
            Dialogue.dialoguePrerequisite, Dialogue.dialogueSingle, 
            Dialogue.dialogueGameStateQuery, Dialogue.dialogueGenderSwitch_startBlock, 
            Dialogue.dialogueRunAction, Dialogue.dialogueStartConversationTopic, 
            Dialogue.dialogueQuestion, Dialogue.dialogueResponse 
        };

        foreach (var command in commands)
        {
            if (cleanText.StartsWith(command))
            {
                cleanText = cleanText[command.Length..];
                break;
            }
        }

        // 移除情绪前缀
        var emotions = new[] { 
            Dialogue.dialogueHappy, Dialogue.dialogueSad, Dialogue.dialogueUnique, 
            Dialogue.dialogueLove, Dialogue.dialogueAngry, Dialogue.dialogueNeutral 
        };
        foreach (var emotion in emotions)
        {
            if (cleanText.StartsWith(emotion))
            {
                cleanText = cleanText[emotion.Length..];
                break;
            }
        }

        // 移除特殊标记
        var specialTokens = new[] 
        {
            Dialogue.randomAdjectiveSpecialCharacter, Dialogue.randomNounSpecialCharacter,
            Dialogue.randomPlaceSpecialCharacter, Dialogue.spouseSpecialCharacter,
            Dialogue.randomNameSpecialCharacter, Dialogue.firstNameLettersSpecialCharacter,
            Dialogue.timeSpecialCharacter, Dialogue.bandNameSpecialCharacter,
            Dialogue.bookNameSpecialCharacter, Dialogue.petSpecialCharacter,
            Dialogue.farmNameSpecialCharacter, Dialogue.favoriteThingSpecialCharacter,
            Dialogue.eventForkSpecialCharacter, Dialogue.yearSpecialCharacter,
            Dialogue.kid1specialCharacter, Dialogue.kid2SpecialCharacter,
            Dialogue.revealTasteCharacter, Dialogue.seasonCharacter,
            Dialogue.dontfacefarmer
        };

        foreach (var token in specialTokens)
        {
            cleanText = cleanText.Replace(token, "");
        }

        // 移除特殊字符
        var specialChars = new[] 
        {
            Dialogue.breakSpecialCharacter, Dialogue.playerNameSpecialCharacter,
            Dialogue.quickResponseDelineator, Dialogue.multipleDialogueDelineator
        };

        foreach (var specialChar in specialChars)
        {
            cleanText = cleanText.Replace(specialChar, "");
        }

        return cleanText.Trim();
    }

    /// <summary>
    /// 从命令类型获取常量
    /// </summary>
    /// <param name="commandType">命令类型</param>
    /// <returns>对应的常量</returns>
    private static string GetConstantFromCommandType(DialogueCommandType commandType)
    {
        return commandType switch
        {
            DialogueCommandType.Break => Dialogue.dialogueBreak,
            DialogueCommandType.End => Dialogue.dialogueEnd,
            DialogueCommandType.Kill => Dialogue.dialogueKill,
            DialogueCommandType.Chance => Dialogue.dialogueChance,
            DialogueCommandType.Conditional => Dialogue.dialogueDependingOnWorldState,
            DialogueCommandType.Event => Dialogue.dialogueEvent,
            DialogueCommandType.QuickResponse => Dialogue.dialogueQuickResponse,
            DialogueCommandType.Prerequisite => Dialogue.dialoguePrerequisite,
            DialogueCommandType.Single => Dialogue.dialogueSingle,
            DialogueCommandType.GameStateQuery => Dialogue.dialogueGameStateQuery,
            DialogueCommandType.GenderSwitch => Dialogue.dialogueGenderSwitch_startBlock,
            DialogueCommandType.RunAction => Dialogue.dialogueRunAction,
            DialogueCommandType.StartConversationTopic => Dialogue.dialogueStartConversationTopic,
            DialogueCommandType.Question => Dialogue.dialogueQuestion,
            DialogueCommandType.Response => Dialogue.dialogueResponse,
            _ => ""
        };
    }
}
