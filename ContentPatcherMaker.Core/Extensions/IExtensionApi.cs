using ContentPatcherMaker.Core.Models;
using ContentPatcherMaker.Core.Validation;

namespace ContentPatcherMaker.Core.Extensions;

/// <summary>
/// 扩展API接口，允许第三方插件添加新的配置参数类型
/// </summary>
public interface IExtensionApi
{
    /// <summary>
    /// 注册自定义补丁类型
    /// </summary>
    /// <param name="patchType">补丁类型</param>
    /// <param name="validator">验证器</param>
    /// <param name="processor">处理器</param>
    void RegisterCustomPatchType(string patchType, ICustomPatchValidator validator, ICustomPatchProcessor processor);

    /// <summary>
    /// 注册自定义令牌提供者
    /// </summary>
    /// <param name="tokenName">令牌名称</param>
    /// <param name="provider">令牌提供者</param>
    void RegisterTokenProvider(string tokenName, ITokenProvider provider);

    /// <summary>
    /// 注册自定义验证规则
    /// </summary>
    /// <param name="ruleName">规则名称</param>
    /// <param name="validator">验证器</param>
    void RegisterValidationRule(string ruleName, IValidationRule validator);

    /// <summary>
    /// 注册自定义输出格式化器
    /// </summary>
    /// <param name="formatType">格式类型</param>
    /// <param name="formatter">格式化器</param>
    void RegisterOutputFormatter(string formatType, IOutputFormatter formatter);

    /// <summary>
    /// 获取所有已注册的补丁类型
    /// </summary>
    /// <returns>补丁类型列表</returns>
    IEnumerable<string> GetRegisteredPatchTypes();

    /// <summary>
    /// 获取所有已注册的令牌提供者
    /// </summary>
    /// <returns>令牌提供者列表</returns>
    IEnumerable<string> GetRegisteredTokenProviders();

    /// <summary>
    /// 验证自定义补丁
    /// </summary>
    /// <param name="patchType">补丁类型</param>
    /// <param name="patchData">补丁数据</param>
    /// <returns>验证结果</returns>
    ValidationResult ValidateCustomPatch(string patchType, Dictionary<string, object> patchData);

    /// <summary>
    /// 处理自定义补丁
    /// </summary>
    /// <param name="patchType">补丁类型</param>
    /// <param name="patchData">补丁数据</param>
    /// <returns>处理结果</returns>
    ProcessingResult ProcessCustomPatch(string patchType, Dictionary<string, object> patchData);

    /// <summary>
    /// 应用验证规则
    /// </summary>
    /// <param name="ruleName">规则名称</param>
    /// <param name="data">要验证的数据</param>
    /// <returns>验证结果</returns>
    ValidationResult ApplyValidationRule(string ruleName, object data);

    /// <summary>
    /// 获取扩展统计信息
    /// </summary>
    /// <returns>统计信息</returns>
    ExtensionStatistics GetStatistics();
}

/// <summary>
/// 自定义补丁验证器接口
/// </summary>
public interface ICustomPatchValidator
{
    /// <summary>
    /// 验证补丁数据
    /// </summary>
    /// <param name="patchData">补丁数据</param>
    /// <returns>验证结果</returns>
    ValidationResult Validate(Dictionary<string, object> patchData);
}

/// <summary>
/// 自定义补丁处理器接口
/// </summary>
public interface ICustomPatchProcessor
{
    /// <summary>
    /// 处理补丁数据
    /// </summary>
    /// <param name="patchData">补丁数据</param>
    /// <returns>处理结果</returns>
    ProcessingResult Process(Dictionary<string, object> patchData);
}

/// <summary>
/// 令牌提供者接口
/// </summary>
public interface ITokenProvider
{
    /// <summary>
    /// 令牌名称
    /// </summary>
    string TokenName { get; }

    /// <summary>
    /// 获取令牌值
    /// </summary>
    /// <param name="context">上下文</param>
    /// <returns>令牌值</returns>
    string GetValue(TokenContext context);

    /// <summary>
    /// 检查令牌是否可用
    /// </summary>
    /// <param name="context">上下文</param>
    /// <returns>是否可用</returns>
    bool IsAvailable(TokenContext context);
}

/// <summary>
/// 验证规则接口
/// </summary>
public interface IValidationRule
{
    /// <summary>
    /// 规则名称
    /// </summary>
    string RuleName { get; }

    /// <summary>
    /// 验证数据
    /// </summary>
    /// <param name="data">要验证的数据</param>
    /// <returns>验证结果</returns>
    ValidationResult Validate(object data);
}

/// <summary>
/// 输出格式化器接口
/// </summary>
public interface IOutputFormatter
{
    /// <summary>
    /// 格式类型
    /// </summary>
    string FormatType { get; }

    /// <summary>
    /// 格式化内容包
    /// </summary>
    /// <param name="contentPack">内容包</param>
    /// <returns>格式化后的字符串</returns>
    string Format(ContentPack contentPack);
}

/// <summary>
/// 令牌上下文
/// </summary>
public class TokenContext
{
    /// <summary>
    /// 游戏状态
    /// </summary>
    public GameState GameState { get; set; } = new();

    /// <summary>
    /// 玩家信息
    /// </summary>
    public PlayerInfo PlayerInfo { get; set; } = new();

    /// <summary>
    /// 当前时间
    /// </summary>
    public DateTime CurrentTime { get; set; }

    /// <summary>
    /// 自定义参数
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();
}

/// <summary>
/// 游戏状态
/// </summary>
public class GameState
{
    /// <summary>
    /// 当前季节
    /// </summary>
    public string Season { get; set; } = string.Empty;

    /// <summary>
    /// 当前天气
    /// </summary>
    public string Weather { get; set; } = string.Empty;

    /// <summary>
    /// 当前时间
    /// </summary>
    public int TimeOfDay { get; set; }

    /// <summary>
    /// 当前地点
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// 游戏年份
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// 游戏天数
    /// </summary>
    public int DayOfMonth { get; set; }

    /// <summary>
    /// 星期几
    /// </summary>
    public string DayOfWeek { get; set; } = string.Empty;
}

/// <summary>
/// 玩家信息
/// </summary>
public class PlayerInfo
{
    /// <summary>
    /// 玩家名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 玩家性别
    /// </summary>
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// 配偶
    /// </summary>
    public string? Spouse { get; set; }

    /// <summary>
    /// 室友
    /// </summary>
    public string? Roommate { get; set; }

    /// <summary>
    /// 技能等级
    /// </summary>
    public Dictionary<string, int> SkillLevels { get; set; } = new();

    /// <summary>
    /// 友谊等级
    /// </summary>
    public Dictionary<string, int> FriendshipLevels { get; set; } = new();

    /// <summary>
    /// 已完成的成就
    /// </summary>
    public List<string> Achievements { get; set; } = [];

    /// <summary>
    /// 已触发的事件
    /// </summary>
    public List<string> Events { get; set; } = [];
}


/// <summary>
/// 处理结果
/// </summary>
public class ProcessingResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 处理后的数据
    /// </summary>
    public Dictionary<string, object> ProcessedData { get; set; } = new();

    /// <summary>
    /// 错误消息
    /// </summary>
    public List<string> Errors { get; set; } = [];

    /// <summary>
    /// 警告消息
    /// </summary>
    public List<string> Warnings { get; set; } = [];
}

/// <summary>
/// 扩展统计信息
/// </summary>
public class ExtensionStatistics
{
    /// <summary>
    /// 已注册的补丁类型数量
    /// </summary>
    public int RegisteredPatchTypes { get; set; }
        
    /// <summary>
    /// 已注册的令牌提供者数量
    /// </summary>
    public int RegisteredTokenProviders { get; set; }
        
    /// <summary>
    /// 已注册的验证规则数量
    /// </summary>
    public int RegisteredValidationRules { get; set; }
        
    /// <summary>
    /// 已注册的输出格式化器数量
    /// </summary>
    public int RegisteredOutputFormatters { get; set; }

    /// <summary>
    /// 返回统计信息的字符串表示
    /// </summary>
    /// <returns>统计信息字符串</returns>
    public override string ToString()
    {
        return $"扩展统计: 补丁类型 {RegisteredPatchTypes}, 令牌提供者 {RegisteredTokenProviders}, " +
               $"验证规则 {RegisteredValidationRules}, 输出格式化器 {RegisteredOutputFormatters}";
    }
}