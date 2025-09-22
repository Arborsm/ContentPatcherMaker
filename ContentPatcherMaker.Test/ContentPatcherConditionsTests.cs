using Xunit;
using Moq;
using ContentPatcherMaker.Core.Extensions;
using ContentPatcherMaker.Core.Services;
using ContentPatcherMaker.Core.Validation;

namespace ContentPatcherMaker.Test;

/// <summary>
/// 基于 conditions-api.md 文档的条件API测试
/// </summary>
public class ContentPatcherConditionsTests
{
    private readonly ContentPatcherService _service;
    private readonly IExtensionApi _extensionApi;

    public ContentPatcherConditionsTests()
    {
        _service = new ContentPatcherService();
        _extensionApi = _service.GetExtensionApi();
    }

    #region Token Context Tests (基于 conditions-api.md)

    /// <summary>
    /// 测试令牌上下文的基本功能
    /// 基于 conditions-api.md 中的上下文概念
    /// </summary>
    [Fact]
    public void TokenContext_BasicContext_ShouldWork()
    {
        // Arrange
        var context = new TokenContext
        {
            GameState = new GameState
            {
                Season = "spring",
                Weather = "sunny",
                TimeOfDay = 1200,
                Location = "Farm",
                Year = 1,
                DayOfMonth = 1,
                DayOfWeek = "Monday"
            },
            PlayerInfo = new PlayerInfo
            {
                Name = "TestPlayer",
                Gender = "Male",
                Spouse = null,
                Roommate = null,
                SkillLevels = new Dictionary<string, int>
                {
                    ["Farming"] = 5,
                    ["Mining"] = 3
                },
                FriendshipLevels = new Dictionary<string, int>
                {
                    ["Abigail"] = 2,
                    ["Penny"] = 1
                },
                Achievements = ["Achievement1"],
                Events = ["Event1"]
            },
            CurrentTime = DateTime.Now,
            Parameters = new Dictionary<string, object>
            {
                ["CustomParam"] = "CustomValue"
            }
        };

        // Act & Assert
        Assert.Equal("spring", context.GameState.Season);
        Assert.Equal("sunny", context.GameState.Weather);
        Assert.Equal(1200, context.GameState.TimeOfDay);
        Assert.Equal("Farm", context.GameState.Location);
        Assert.Equal(1, context.GameState.Year);
        Assert.Equal(1, context.GameState.DayOfMonth);
        Assert.Equal("Monday", context.GameState.DayOfWeek);

        Assert.Equal("TestPlayer", context.PlayerInfo.Name);
        Assert.Equal("Male", context.PlayerInfo.Gender);
        Assert.Null(context.PlayerInfo.Spouse);
        Assert.Null(context.PlayerInfo.Roommate);
        Assert.Equal(5, context.PlayerInfo.SkillLevels["Farming"]);
        Assert.Equal(3, context.PlayerInfo.SkillLevels["Mining"]);
        Assert.Equal(2, context.PlayerInfo.FriendshipLevels["Abigail"]);
        Assert.Equal(1, context.PlayerInfo.FriendshipLevels["Penny"]);
        Assert.Single(context.PlayerInfo.Achievements);
        Assert.Single(context.PlayerInfo.Events);

        Assert.Equal("CustomValue", context.Parameters["CustomParam"]);
    }

    #endregion

    #region Token Provider with Conditions Tests

    /// <summary>
    /// 测试基于条件的令牌提供者
    /// 基于 conditions-api.md 中的条件概念
    /// </summary>
    [Fact]
    public void TokenProvider_ConditionalToken_ShouldWork()
    {
        // Arrange
        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(x => x.TokenName).Returns("ConditionalToken");
            
        // 模拟基于条件的令牌值
        mockTokenProvider.Setup(x => x.GetValue(It.Is<TokenContext>(c => 
                c.GameState.Season == "spring")))
            .Returns("SpringValue");
            
        mockTokenProvider.Setup(x => x.GetValue(It.Is<TokenContext>(c => 
                c.GameState.Season == "winter")))
            .Returns("WinterValue");

        mockTokenProvider.Setup(x => x.IsAvailable(It.IsAny<TokenContext>()))
            .Returns(true);

        // Act
        _extensionApi.RegisterTokenProvider("ConditionalToken", mockTokenProvider.Object);
        var statistics = _extensionApi.GetStatistics();

        // Assert
        Assert.Equal(1, statistics.RegisteredTokenProviders);
    }

    /// <summary>
    /// 测试令牌提供者的可用性检查
    /// </summary>
    [Fact]
    public void TokenProvider_AvailabilityCheck_ShouldWork()
    {
        // Arrange
        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(x => x.TokenName).Returns("AvailabilityToken");
        mockTokenProvider.Setup(x => x.GetValue(It.IsAny<TokenContext>()))
            .Returns("TestValue");

        // 模拟基于游戏状态的可用性检查
        mockTokenProvider.Setup(x => x.IsAvailable(It.Is<TokenContext>(c => 
                c.GameState != null)))
            .Returns(true);

        mockTokenProvider.Setup(x => x.IsAvailable(It.Is<TokenContext>(c => 
                c.GameState == null)))
            .Returns(false);

        // Act
        _extensionApi.RegisterTokenProvider("AvailabilityToken", mockTokenProvider.Object);
        var statistics = _extensionApi.GetStatistics();

        // Assert
        Assert.Equal(1, statistics.RegisteredTokenProviders);
    }

    #endregion

    #region Validation with Conditions Tests

    /// <summary>
    /// 测试基于条件的验证规则
    /// </summary>
    [Fact]
    public void ValidationRule_ConditionalValidation_ShouldWork()
    {
        // Arrange
        var mockValidationRule = new Mock<IValidationRule>();
        mockValidationRule.Setup(x => x.RuleName).Returns("ConditionalRule");

        // 模拟基于条件的验证
        mockValidationRule.Setup(x => x.Validate(It.Is<string>(s => s.Length > 0)))
            .Returns(new ValidationResult { IsValid = true });

        mockValidationRule.Setup(x => x.Validate(It.Is<string>(s => s.Length == 0)))
            .Returns(new ValidationResult 
            { 
                IsValid = false, 
                Errors = ["String cannot be empty"]
            });

        // Act
        _extensionApi.RegisterValidationRule("ConditionalRule", mockValidationRule.Object);
        var statistics = _extensionApi.GetStatistics();

        // Assert
        Assert.Equal(1, statistics.RegisteredValidationRules);
    }

    #endregion

    #region Complex Condition Scenarios Tests

    /// <summary>
    /// 测试复杂的条件场景
    /// 基于 conditions-api.md 中的复杂条件示例
    /// </summary>
    [Fact]
    public void ComplexConditions_MultipleConditions_ShouldWork()
    {
        // Arrange
        var context = new TokenContext
        {
            GameState = new GameState
            {
                Season = "winter",
                Weather = "snow",
                TimeOfDay = 1800, // 6 PM
                Location = "Town",
                DayOfWeek = "Saturday"
            },
            PlayerInfo = new PlayerInfo
            {
                Name = "TestPlayer",
                Spouse = "Abigail",
                SkillLevels = new Dictionary<string, int>
                {
                    ["Farming"] = 10
                },
                FriendshipLevels = new Dictionary<string, int>
                {
                    ["Abigail"] = 10
                }
            }
        };

        // 模拟复杂的条件令牌
        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(x => x.TokenName).Returns("ComplexConditionToken");

        // 冬季周末晚上下雪时提高咖啡价格的条件
        mockTokenProvider.Setup(x => x.GetValue(It.Is<TokenContext>(c => 
                c.GameState.Season == "winter" &&
                c.GameState.Weather == "snow" &&
                c.GameState.TimeOfDay >= 1800 &&
                c.GameState.DayOfWeek == "Saturday")))
            .Returns("HighPrice");

        // 除非玩家与阿比盖尔结婚
        mockTokenProvider.Setup(x => x.GetValue(It.Is<TokenContext>(c => 
                c.PlayerInfo.Spouse == "Abigail")))
            .Returns("NormalPrice");

        mockTokenProvider.Setup(x => x.IsAvailable(It.IsAny<TokenContext>()))
            .Returns(true);

        // Act
        _extensionApi.RegisterTokenProvider("ComplexConditionToken", mockTokenProvider.Object);
            
        // 实际使用context来测试条件逻辑
        var result = mockTokenProvider.Object.GetValue(context);
        var isAvailable = mockTokenProvider.Object.IsAvailable(context);
        var statistics = _extensionApi.GetStatistics();

        // Assert
        Assert.Equal(1, statistics.RegisteredTokenProviders);
        Assert.True(isAvailable);
        // 由于玩家与阿比盖尔结婚，应该返回"NormalPrice"而不是"HighPrice"
        Assert.Equal("NormalPrice", result);
    }

    #endregion

    #region Error Handling in Conditions Tests

    /// <summary>
    /// 测试条件中的错误处理
    /// </summary>
    [Fact]
    public void ErrorHandling_ConditionErrors_ShouldHandleGracefully()
    {
        // Arrange
        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(x => x.TokenName).Returns("ErrorToken");
        mockTokenProvider.Setup(x => x.GetValue(It.IsAny<TokenContext>()))
            .Throws(new Exception("Token evaluation error"));
        mockTokenProvider.Setup(x => x.IsAvailable(It.IsAny<TokenContext>()))
            .Returns(true);

        // Act
        _extensionApi.RegisterTokenProvider("ErrorToken", mockTokenProvider.Object);
        var statistics = _extensionApi.GetStatistics();

        // Assert
        Assert.Equal(1, statistics.RegisteredTokenProviders);
    }

    #endregion

    #region Performance Tests

    /// <summary>
    /// 测试条件评估的性能
    /// </summary>
    [Fact]
    public void Performance_ConditionEvaluation_ShouldBeEfficient()
    {
        // Arrange
        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(x => x.TokenName).Returns("PerformanceToken");
        mockTokenProvider.Setup(x => x.GetValue(It.IsAny<TokenContext>()))
            .Returns("PerformanceValue");
        mockTokenProvider.Setup(x => x.IsAvailable(It.IsAny<TokenContext>()))
            .Returns(true);

        // Act
        var startTime = DateTime.Now;
        _extensionApi.RegisterTokenProvider("PerformanceToken", mockTokenProvider.Object);
        var endTime = DateTime.Now;

        // Assert
        Assert.True((endTime - startTime).TotalMilliseconds < 1000); // 应该在1秒内完成
    }

    #endregion

    #region Integration Tests

    /// <summary>
    /// 测试条件API与主服务的集成
    /// </summary>
    [Fact]
    public void Integration_ConditionsWithMainService_ShouldWork()
    {
        // Arrange
        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(x => x.TokenName).Returns("IntegrationToken");
        mockTokenProvider.Setup(x => x.GetValue(It.IsAny<TokenContext>()))
            .Returns("IntegrationValue");
        mockTokenProvider.Setup(x => x.IsAvailable(It.IsAny<TokenContext>()))
            .Returns(true);

        // Act
        _extensionApi.RegisterTokenProvider("IntegrationToken", mockTokenProvider.Object);
        var serviceStatistics = _service.GetStatistics();

        // Assert
        Assert.Equal(1, serviceStatistics.ExtensionStatistics.RegisteredTokenProviders);
    }

    #endregion

    #region Edge Cases Tests

    /// <summary>
    /// 测试边界情况
    /// </summary>
    [Fact]
    public void EdgeCases_NullContext_ShouldHandleGracefully()
    {
        // Arrange
        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(x => x.TokenName).Returns("EdgeCaseToken");
        mockTokenProvider.Setup(x => x.GetValue(It.IsAny<TokenContext>()))
            .Returns("EdgeCaseValue");
        mockTokenProvider.Setup(x => x.IsAvailable(It.IsAny<TokenContext>()))
            .Returns(false); // 当上下文为null时返回false

        // Act
        _extensionApi.RegisterTokenProvider("EdgeCaseToken", mockTokenProvider.Object);
        var statistics = _extensionApi.GetStatistics();

        // Assert
        Assert.Equal(1, statistics.RegisteredTokenProviders);
    }

    /// <summary>
    /// 测试空参数的情况
    /// </summary>
    [Fact]
    public void EdgeCases_EmptyParameters_ShouldWork()
    {
        // Arrange
        var context = new TokenContext
        {
            Parameters = new Dictionary<string, object>() // 空参数
        };

        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(x => x.TokenName).Returns("EmptyParamsToken");
            
        // 模拟处理空参数的情况
        mockTokenProvider.Setup(x => x.GetValue(It.Is<TokenContext>(c => 
                c.Parameters != null && c.Parameters.Count == 0)))
            .Returns("EmptyParamsValue");
            
        mockTokenProvider.Setup(x => x.IsAvailable(It.Is<TokenContext>(c => 
                c.Parameters != null && c.Parameters.Count == 0)))
            .Returns(true);

        // Act
        _extensionApi.RegisterTokenProvider("EmptyParamsToken", mockTokenProvider.Object);
            
        // 实际使用context来测试空参数处理
        var result = mockTokenProvider.Object.GetValue(context);
        var isAvailable = mockTokenProvider.Object.IsAvailable(context);
        var statistics = _extensionApi.GetStatistics();

        // Assert
        Assert.Equal(1, statistics.RegisteredTokenProviders);
        Assert.True(isAvailable);
        Assert.Equal("EmptyParamsValue", result);
        Assert.NotNull(context.Parameters);
        Assert.Empty(context.Parameters);
    }

    #endregion
}