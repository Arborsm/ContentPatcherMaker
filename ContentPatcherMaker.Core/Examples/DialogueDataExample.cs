using ContentPatcherMaker.Core.DataModels.Datas;
using Newtonsoft.Json;
using StardewValley;

namespace ContentPatcherMaker.Core.Examples;

/// <summary>
/// DialogueData使用示例
/// 展示如何创建、管理和序列化对话数据
/// </summary>
public class DialogueDataExample
{
    /// <summary>
    /// 创建示例对话数据
    /// </summary>
    /// <returns>示例对话数据</returns>
    public static DialogueData CreateExampleDialogue()
    {
        var dialogue = new DialogueData
        {
            Id = "example_dialogue_001",
            SpeakerId = "Abigail",
            Name = "阿比盖尔的问候",
            Description = "阿比盖尔在农场门口的问候对话",
            TranslationKey = "Characters/Dialogue/Abigail:Greeting"
        };

        // 添加对话行
        dialogue.AddDialogueLine("你好！今天天气真不错呢！", DialogueEmotion.Happy);
        dialogue.AddDialogueLine("你最近在农场忙些什么呢？", DialogueEmotion.Neutral);
        dialogue.AddDialogueLine("我听说你种了一些新的作物，能告诉我是什么吗？", DialogueEmotion.Unique);

        // 添加玩家响应选项
        dialogue.AddPlayerResponse("我在种草莓！", 10, "strawberry_response", "strawberry_10", "response_001");
        dialogue.AddPlayerResponse("我在种土豆。", 5, "potato_response", "potato_5", "response_002");
        dialogue.AddPlayerResponse("这是秘密！", -5, "secret_response", "secret_-5", "response_003");

        // 添加快速响应
        dialogue.AddQuickResponse("谢谢你的关心！");
        dialogue.AddQuickResponse("我们改天再聊吧！");

        // 设置对话属性
        dialogue.IsLastDialogueInteractive = true;
        dialogue.ShowPortrait = true;
        dialogue.CurrentDialogueIndex = 0;

        return dialogue;
    }

    /// <summary>
    /// 创建复杂的对话数据（使用构建器模式）
    /// </summary>
    /// <returns>复杂对话数据</returns>
    public static DialogueData CreateComplexDialogue()
    {
        // 创建条件
        var communityCenterCondition = new DialogueCondition
        {
            ConditionKey = "cc",
            Description = "社区中心是否完成",
            Type = ConditionType.CommunityCenter
        };

        // 使用构建器创建复杂对话
        var dialogue = new DialogueBuilder("Penny", "佩妮的复杂对话")
            .AddLine("你好！今天心情很好！", DialogueEmotion.Happy)
            .AddChanceLine("让我们来玩个游戏吧！", 0.5, DialogueEmotion.Happy)
            .AddConditionalLine(communityCenterCondition, "你完成了社区中心吗？太棒了！", "你还没有完成社区中心呢。")
            .AddEventLine("festival_001", DialogueEmotion.Happy)
            .AddQuestionLine("question_001", "你想和我一起学习吗？", DialogueEmotion.Unique)
            .AddPlayerResponse("当然！我很乐意学习。", 10, "learn_yes", "learn_10", "response_001")
            .AddPlayerResponse("抱歉，我现在很忙。", -5, "learn_no", "learn_-5", "response_002")
            .AddQuickResponse("我们改天再聊吧！")
            .AddBreak()
            .AddEnd()
            .SetProperties(showPortrait: true, faceFarmer: true, removeOnNextMove: false)
            .Build();

        dialogue.Description = "包含多种命令和条件的复杂对话";
        return dialogue;
    }

    /// <summary>
    /// 创建使用解析方法的对话
    /// </summary>
    /// <returns>解析的对话数据</returns>
    public static DialogueData CreateParsedDialogue()
    {
        // 原始Stardew Valley对话字符串
        var dialogueString = $"{Dialogue.dialogueHappy}你好！{Dialogue.dialogueBreak}{Dialogue.dialogueChance}0.3今天天气不错！{Dialogue.dialogueEnd}";
        
        // 解析对话字符串
        var dialogue = DialogueData.ParseDialogueString(dialogueString, "Abigail", "Characters/Dialogue/Abigail:Greeting");
        dialogue.Name = "阿比盖尔的解析对话";
        dialogue.Description = "从Stardew Valley对话字符串解析的对话";
        
        return dialogue;
    }

    /// <summary>
    /// 演示对话解析和反解析
    /// </summary>
    public static void DemonstrateParsing()
    {
        Console.WriteLine("=== 对话解析和反解析示例 ===\n");

        // 创建原始对话
        var originalDialogue = CreateExampleDialogue();
        Console.WriteLine("1. 原始对话:");
        Console.WriteLine($"对话行数: {originalDialogue.DialogueLines.Count}");
        Console.WriteLine($"玩家响应数: {originalDialogue.PlayerResponses.Count}");

        // 转换为Stardew Valley字符串
        var dialogueString = originalDialogue.ToDialogueString();
        Console.WriteLine($"\n2. 转换为Stardew Valley字符串:");
        Console.WriteLine($"字符串: {dialogueString}");

        // 重新解析字符串
        var parsedDialogue = DialogueData.ParseDialogueString(dialogueString, originalDialogue.SpeakerId, originalDialogue.TranslationKey);
        Console.WriteLine($"\n3. 重新解析的对话:");
        Console.WriteLine($"对话行数: {parsedDialogue.DialogueLines.Count}");
        Console.WriteLine($"玩家响应数: {parsedDialogue.PlayerResponses.Count}");

        // 验证解析结果
        var isValid = parsedDialogue.DialogueLines.Count == originalDialogue.DialogueLines.Count;
        Console.WriteLine($"\n4. 解析验证: {(isValid ? "成功" : "失败")}");
    }

    /// <summary>
    /// 序列化对话数据到JSON
    /// </summary>
    /// <param name="dialogue">对话数据</param>
    /// <returns>JSON字符串</returns>
    public static string SerializeDialogue(DialogueData dialogue)
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };

        return JsonConvert.SerializeObject(dialogue, settings);
    }

    /// <summary>
    /// 从JSON反序列化对话数据
    /// </summary>
    /// <param name="json">JSON字符串</param>
    /// <returns>对话数据</returns>
    public static DialogueData? DeserializeDialogue(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<DialogueData>(json);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"反序列化失败: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 创建对话数据集合并演示各种操作
    /// </summary>
    public static void DemonstrateDialogueCollection()
    {
        // 创建多个对话
        var dialogues = new List<DialogueData>
        {
            CreateExampleDialogue(),
            CreateComplexDialogue(),
            new()
            {
                Id = "simple_dialogue_001",
                SpeakerId = "Alex",
                Name = "亚历克斯的简单对话",
                Description = "简单的问候对话"
            }
        };

        // 创建对话集合
        var collection = new DialogueDataCollection(dialogues);

        // 演示各种查询操作
        Console.WriteLine($"总对话数: {collection.Count}");

        // 根据说话者查询
        var abigailDialogues = collection.GetDialoguesBySpeaker("Abigail");
        Console.WriteLine($"阿比盖尔的对话数: {abigailDialogues.Count()}");

        // 根据情绪查询
        var happyDialogues = collection.GetDialoguesByEmotion(DialogueEmotion.Happy);
        Console.WriteLine($"开心情绪的对话数: {happyDialogues.Count()}");

        // 根据命令类型查询
        var eventDialogues = collection.GetDialoguesByCommandType(DialogueCommandType.Event);
        Console.WriteLine($"包含事件的对话数: {eventDialogues.Count()}");

        // 文本搜索
        var searchResults = collection.SearchByText("你好");
        Console.WriteLine($"包含'你好'的对话数: {searchResults.Count()}");

        // 获取交互式对话
        var interactiveDialogues = collection.GetInteractiveDialogues();
        Console.WriteLine($"交互式对话数: {interactiveDialogues.Count()}");
    }

    /// <summary>
    /// 演示对话验证
    /// </summary>
    public static void DemonstrateValidation()
    {
        // 创建有效对话
        var validDialogue = CreateExampleDialogue();
        var validationResult = validDialogue.Validate();
        Console.WriteLine($"有效对话验证结果: {validationResult.IsValid}");
        if (!validationResult.IsValid)
        {
            Console.WriteLine($"错误: {string.Join(", ", validationResult.Errors)}");
        }

        // 创建无效对话
        var invalidDialogue = new DialogueData
        {
            Id = "", // 空ID
            SpeakerId = "", // 空说话者ID
            Name = "", // 空名称
            DialogueLines = new List<DialogueLineData>() // 空对话行
        };

        var invalidValidationResult = invalidDialogue.Validate();
        Console.WriteLine($"无效对话验证结果: {invalidValidationResult.IsValid}");
        if (!invalidValidationResult.IsValid)
        {
            Console.WriteLine($"错误: {string.Join(", ", invalidValidationResult.Errors)}");
        }
    }

    /// <summary>
    /// 演示对话操作
    /// </summary>
    public static void DemonstrateDialogueOperations()
    {
        var dialogue = CreateExampleDialogue();

        Console.WriteLine($"当前对话: {dialogue.GetCurrentDialogue()}");
        Console.WriteLine($"是否在最终对话: {dialogue.IsOnFinalDialogue()}");
        Console.WriteLine($"对话是否完成: {dialogue.IsDialogueFinished()}");
        Console.WriteLine($"当前对话是问题: {dialogue.IsCurrentDialogueAQuestion()}");
        Console.WriteLine($"肖像索引: {dialogue.GetPortraitIndex()}");

        // 模拟对话进行
        Console.WriteLine("\n--- 对话进行 ---");
        while (!dialogue.IsDialogueFinished())
        {
            Console.WriteLine($"当前对话: {dialogue.GetCurrentDialogue()}");
            var nextDialogue = dialogue.ExitCurrentDialogue();
            if (nextDialogue != null)
            {
                Console.WriteLine($"下一段对话: {nextDialogue}");
            }
        }

        Console.WriteLine("对话结束");
    }

    /// <summary>
    /// 演示Stardew Valley常量转换
    /// </summary>
    public static void DemonstrateConstantConversion()
    {
        Console.WriteLine("=== Stardew Valley 常量转换示例 ===\n");

        // 演示情绪常量转换
        Console.WriteLine("1. 情绪常量转换:");
        var happyConstant = Dialogue.dialogueHappy;
        var emotion = DialogueData.GetEmotionFromConstant(happyConstant);
        var backToConstant = DialogueData.GetConstantFromEmotion(emotion);
        Console.WriteLine($"常量: {happyConstant} -> 枚举: {emotion} -> 常量: {backToConstant}");

        // 演示命令类型常量转换
        Console.WriteLine("\n2. 命令类型常量转换:");
        var breakConstant = Dialogue.dialogueBreak;
        var commandType = DialogueData.GetCommandTypeFromConstant(breakConstant);
        Console.WriteLine($"常量: {breakConstant} -> 枚举: {commandType}");

        // 演示所有情绪常量
        Console.WriteLine("\n3. 所有情绪常量:");
        var emotionConstants = new[]
        {
            Dialogue.dialogueHappy,
            Dialogue.dialogueSad,
            Dialogue.dialogueUnique,
            Dialogue.dialogueLove,
            Dialogue.dialogueAngry,
            Dialogue.dialogueNeutral
        };

        foreach (var constant in emotionConstants)
        {
            var convertedEmotion = DialogueData.GetEmotionFromConstant(constant);
            Console.WriteLine($"{constant} -> {convertedEmotion}");
        }

        // 演示所有命令常量
        Console.WriteLine("\n4. 所有命令常量:");
        var commandConstants = new[]
        {
            Dialogue.dialogueBreak,
            Dialogue.dialogueEnd,
            Dialogue.dialogueKill,
            Dialogue.dialogueChance,
            Dialogue.dialogueDependingOnWorldState,
            Dialogue.dialogueEvent,
            Dialogue.dialogueQuickResponse,
            Dialogue.dialoguePrerequisite,
            Dialogue.dialogueSingle,
            Dialogue.dialogueGameStateQuery,
            Dialogue.dialogueGenderSwitch_startBlock,
            Dialogue.dialogueRunAction,
            Dialogue.dialogueStartConversationTopic,
            Dialogue.dialogueQuestion,
            Dialogue.dialogueResponse
        };

        foreach (var constant in commandConstants)
        {
            var convertedCommandType = DialogueData.GetCommandTypeFromConstant(constant);
            Console.WriteLine($"{constant} -> {convertedCommandType}");
        }
    }

    /// <summary>
    /// 运行所有示例
    /// </summary>
    public static void RunAllExamples()
    {
        Console.WriteLine("=== DialogueData 使用示例 ===\n");

        Console.WriteLine("1. 创建示例对话:");
        var dialogue = CreateExampleDialogue();
        Console.WriteLine($"对话ID: {dialogue.Id}");
        Console.WriteLine($"说话者: {dialogue.SpeakerId}");
        Console.WriteLine($"对话行数: {dialogue.DialogueLines.Count}");
        Console.WriteLine($"玩家响应数: {dialogue.PlayerResponses.Count}\n");

        Console.WriteLine("2. 序列化对话:");
        var json = SerializeDialogue(dialogue);
        Console.WriteLine($"JSON长度: {json.Length} 字符\n");

        Console.WriteLine("3. 反序列化对话:");
        var deserializedDialogue = DeserializeDialogue(json);
        Console.WriteLine($"反序列化成功: {deserializedDialogue != null}\n");

        Console.WriteLine("4. 对话集合操作:");
        DemonstrateDialogueCollection();
        Console.WriteLine();

        Console.WriteLine("5. 对话验证:");
        DemonstrateValidation();
        Console.WriteLine();

        Console.WriteLine("6. 对话操作:");
        DemonstrateDialogueOperations();
        Console.WriteLine();

        Console.WriteLine("7. Stardew Valley 常量转换:");
        DemonstrateConstantConversion();
        Console.WriteLine();

        Console.WriteLine("8. 对话解析和反解析:");
        DemonstrateParsing();
        Console.WriteLine();

        Console.WriteLine("9. 构建器模式示例:");
        var complexDialogue = CreateComplexDialogue();
        Console.WriteLine($"构建的复杂对话: {complexDialogue.Name}");
        Console.WriteLine($"对话行数: {complexDialogue.DialogueLines.Count}");
        Console.WriteLine($"条件数: {complexDialogue.DialogueLines.Count(dl => dl.CommandType == DialogueCommandType.Conditional)}");

        Console.WriteLine("\n10. 完整常量映射示例:");
        DemonstrateCompleteConstantMapping();
    }

    /// <summary>
    /// 演示完整的常量映射
    /// </summary>
    public static void DemonstrateCompleteConstantMapping()
    {
        Console.WriteLine("=== 完整常量映射演示 ===");

        // 演示特殊字符转换
        Console.WriteLine("\n1. 特殊字符转换:");
        var genderChar = Dialogue.genderDialogueSplitCharacter;
        var specialCharType = DialogueData.GetSpecialCharacterTypeFromConstant(genderChar);
        Console.WriteLine($"字符: {genderChar} -> 枚举: {specialCharType}");

        // 演示特殊标记转换
        Console.WriteLine("\n2. 特殊标记转换:");
        var spouseToken = Dialogue.spouseSpecialCharacter;
        var tokenType = DialogueData.GetSpecialTokenTypeFromConstant(spouseToken);
        Console.WriteLine($"标记: {spouseToken} -> 枚举: {tokenType}");

        // 演示获取所有常量
        Console.WriteLine("\n3. 所有对话常量 (前10个):");
        var allConstants = DialogueData.GetAllDialogueConstants();
        foreach (var constant in allConstants.Take(10))
        {
            Console.WriteLine($"  {constant.Key}: {constant.Value}");
        }

        Console.WriteLine("\n4. 所有特殊字符:");
        var allSpecialChars = DialogueData.GetAllSpecialCharacters();
        foreach (var specialChar in allSpecialChars)
        {
            Console.WriteLine($"  {specialChar.Key}: {specialChar.Value}");
        }

        Console.WriteLine("\n5. 所有数组常量:");
        var allArrays = DialogueData.GetAllArrayConstants();
        foreach (var array in allArrays)
        {
            Console.WriteLine($"  {array.Key}: [{string.Join(", ", array.Value.Take(3))}...] (共{array.Value.Length}项)");
        }
    }
}
