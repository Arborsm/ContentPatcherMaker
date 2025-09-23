using ContentPatcherMaker.Core.Extensions;
using ContentPatcherMaker.Core.Models;
using ContentPatcherMaker.Core.Services;
using ContentPatcherMaker.Core.Validation;
using Moq;
using Xunit;

namespace ContentPatcherMaker.Test;

/// <summary>
/// 基于 extensibility.md 文档的扩展API测试
/// </summary>
public class ContentPatcherExtensionTests
{
    private readonly ContentPatcherService _service;
    private readonly IExtensionApi _extensionApi;

    public ContentPatcherExtensionTests()
    {
        _service = new ContentPatcherService();
        _extensionApi = _service.GetExtensionApi();
    }

    #region Token Provider Tests (基于 extensibility.md)

    /// <summary>
    /// 测试基本令牌提供者注册
    /// 基于 extensibility.md 中的 PlayerName 示例
    /// </summary>
    [Fact]
    public void TokenProvider_RegisterPlayerNameToken_ShouldWork()
    {
        // Arrange
        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(x => x.TokenName).Returns("PlayerName");
        mockTokenProvider.Setup(x => x.GetValue(It.IsAny<TokenContext>()))
            .Returns("TestPlayer");
        mockTokenProvider.Setup(x => x.IsAvailable(It.IsAny<TokenContext>()))
            .Returns(true);

        // Act
        _extensionApi.RegisterTokenProvider("PlayerName", mockTokenProvider.Object);
        var statistics = _extensionApi.GetStatistics();

        // Assert
        Assert.Equal(1, statistics.RegisteredTokenProviders);
    }

    /// <summary>
    /// 测试高级令牌提供者注册
    /// 基于 extensibility.md 中的 InitialsToken 示例
    /// </summary>
    [Fact]
    public void TokenProvider_RegisterInitialsToken_ShouldWork()
    {
        // Arrange
        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(x => x.TokenName).Returns("InitialsToken");
        mockTokenProvider.Setup(x => x.GetValue(It.IsAny<TokenContext>()))
            .Returns("TP"); // TestPlayer 的首字母
        mockTokenProvider.Setup(x => x.IsAvailable(It.IsAny<TokenContext>()))
            .Returns(true);

        // Act
        _extensionApi.RegisterTokenProvider("InitialsToken", mockTokenProvider.Object);
        var statistics = _extensionApi.GetStatistics();

        // Assert
        Assert.Equal(1, statistics.RegisteredTokenProviders);
    }

    /// <summary>
    /// 测试令牌提供者的上下文处理
    /// </summary>
    [Fact]
    public void TokenProvider_TokenContext_ShouldBePassedCorrectly()
    {
        // Arrange
        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(x => x.TokenName).Returns("TestToken");
        
        // 模拟基于特定上下文的返回值
        mockTokenProvider.Setup(x => x.GetValue(It.Is<TokenContext>(c => 
            c.GameState.Season == "spring" && 
            c.PlayerInfo.Name == "TestPlayer")))
            .Returns("SpringPlayerValue");
        
        mockTokenProvider.Setup(x => x.IsAvailable(It.Is<TokenContext>(c => 
            c.GameState != null && c.PlayerInfo != null)))
            .Returns(true);

        _extensionApi.RegisterTokenProvider("TestToken", mockTokenProvider.Object);

        // Act
        var context = new TokenContext
        {
            GameState = new GameState
            {
                Season = "spring",
                Weather = "sunny"
            },
            PlayerInfo = new PlayerInfo
            {
                Name = "TestPlayer"
            }
        };

        // 实际使用context来测试token provider的功能
        var result = mockTokenProvider.Object.GetValue(context);
        var isAvailable = mockTokenProvider.Object.IsAvailable(context);
        var statistics = _extensionApi.GetStatistics();

        // Assert
        Assert.Equal(1, statistics.RegisteredTokenProviders);
        Assert.True(isAvailable);
        Assert.Equal("SpringPlayerValue", result);
        Assert.Equal("spring", context.GameState.Season);
        Assert.Equal("TestPlayer", context.PlayerInfo.Name);
    }

    #endregion

    #region Custom Patch Type Tests

    /// <summary>
    /// 测试自定义补丁类型注册
    /// </summary>
    [Fact]
    public void CustomPatchType_RegisterCustomPatch_ShouldWork()
    {
        // Arrange
        var mockValidator = new Mock<ICustomPatchValidator>();
        mockValidator.Setup(x => x.Validate(It.IsAny<Dictionary<string, object>>()))
            .Returns(new ValidationResult { IsValid = true });

        var mockProcessor = new Mock<ICustomPatchProcessor>();
        mockProcessor.Setup(x => x.Process(It.IsAny<Dictionary<string, object>>()))
            .Returns(new ProcessingResult { IsSuccess = true });

        // Act
        _extensionApi.RegisterCustomPatchType("CustomPatch", mockValidator.Object, mockProcessor.Object);
        var statistics = _extensionApi.GetStatistics();

        // Assert
        Assert.Equal(1, statistics.RegisteredPatchTypes);
    }

    /// <summary>
    /// 测试自定义补丁验证
    /// </summary>
    [Fact]
    public void CustomPatchType_ValidateCustomPatch_ShouldWork()
    {
        // Arrange
        var mockValidator = new Mock<ICustomPatchValidator>();
        mockValidator.Setup(x => x.Validate(It.IsAny<Dictionary<string, object>>()))
            .Returns(new ValidationResult { IsValid = true });

        var mockProcessor = new Mock<ICustomPatchProcessor>();
        mockProcessor.Setup(x => x.Process(It.IsAny<Dictionary<string, object>>()))
            .Returns(new ProcessingResult { IsSuccess = true });

        _extensionApi.RegisterCustomPatchType("CustomPatch", mockValidator.Object, mockProcessor.Object);

        var patchData = new Dictionary<string, object>
        {
            ["TestField"] = "TestValue"
        };

        // Act
        var validationResult = _extensionApi.ValidateCustomPatch("CustomPatch", patchData);

        // Assert
        Assert.True(validationResult.IsValid);
    }

    /// <summary>
    /// 测试自定义补丁处理
    /// </summary>
    [Fact]
    public void CustomPatchType_ProcessCustomPatch_ShouldWork()
    {
        // Arrange
        var mockValidator = new Mock<ICustomPatchValidator>();
        mockValidator.Setup(x => x.Validate(It.IsAny<Dictionary<string, object>>()))
            .Returns(new ValidationResult { IsValid = true });

        var mockProcessor = new Mock<ICustomPatchProcessor>();
        mockProcessor.Setup(x => x.Process(It.IsAny<Dictionary<string, object>>()))
            .Returns(new ProcessingResult { IsSuccess = true });

        _extensionApi.RegisterCustomPatchType("CustomPatch", mockValidator.Object, mockProcessor.Object);

        var patchData = new Dictionary<string, object>
        {
            ["TestField"] = "TestValue"
        };

        // Act
        var processingResult = _extensionApi.ProcessCustomPatch("CustomPatch", patchData);

        // Assert
        Assert.True(processingResult.IsSuccess);
    }

    #endregion

    #region Validation Rule Tests

    /// <summary>
    /// 测试自定义验证规则注册
    /// </summary>
    [Fact]
    public void ValidationRule_RegisterCustomRule_ShouldWork()
    {
        // Arrange
        var mockValidationRule = new Mock<IValidationRule>();
        mockValidationRule.Setup(x => x.RuleName).Returns("CustomRule");
        mockValidationRule.Setup(x => x.Validate(It.IsAny<object>()))
            .Returns(new ValidationResult { IsValid = true });

        // Act
        _extensionApi.RegisterValidationRule("CustomRule", mockValidationRule.Object);
        var statistics = _extensionApi.GetStatistics();

        // Assert
        Assert.Equal(1, statistics.RegisteredValidationRules);
    }

    /// <summary>
    /// 测试验证规则的应用
    /// </summary>
    [Fact]
    public void ValidationRule_ApplyValidationRule_ShouldWork()
    {
        // Arrange
        var mockValidationRule = new Mock<IValidationRule>();
        mockValidationRule.Setup(x => x.RuleName).Returns("CustomRule");
        mockValidationRule.Setup(x => x.Validate(It.IsAny<object>()))
            .Returns(new ValidationResult { IsValid = true });

        _extensionApi.RegisterValidationRule("CustomRule", mockValidationRule.Object);

        // Act
        var validationResult = _extensionApi.ApplyValidationRule("CustomRule", "test data");

        // Assert
        Assert.True(validationResult.IsValid);
    }

    #endregion

    #region Output Formatter Tests

    /// <summary>
    /// 测试自定义输出格式化器注册
    /// </summary>
    [Fact]
    public void OutputFormatter_RegisterCustomFormatter_ShouldWork()
    {
        // Arrange
        var mockFormatter = new Mock<IOutputFormatter>();
        mockFormatter.Setup(x => x.FormatType).Returns("CustomFormat");
        mockFormatter.Setup(x => x.Format(It.IsAny<ContentPack>()))
            .Returns("Custom formatted output");

        // Act
        _extensionApi.RegisterOutputFormatter("CustomFormat", mockFormatter.Object);
        var statistics = _extensionApi.GetStatistics();

        // Assert
        Assert.Equal(1, statistics.RegisteredOutputFormatters);
    }

    #endregion

    #region Statistics Tests

    /// <summary>
    /// 测试扩展统计信息的准确性
    /// </summary>
    [Fact]
    public void Statistics_MultipleRegistrations_ShouldTrackCorrectly()
    {
        // Arrange
        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(x => x.TokenName).Returns("TestToken");
        mockTokenProvider.Setup(x => x.GetValue(It.IsAny<TokenContext>())).Returns("TestValue");
        mockTokenProvider.Setup(x => x.IsAvailable(It.IsAny<TokenContext>())).Returns(true);

        var mockValidator = new Mock<ICustomPatchValidator>();
        mockValidator.Setup(x => x.Validate(It.IsAny<Dictionary<string, object>>()))
            .Returns(new ValidationResult { IsValid = true });

        var mockProcessor = new Mock<ICustomPatchProcessor>();
        mockProcessor.Setup(x => x.Process(It.IsAny<Dictionary<string, object>>()))
            .Returns(new ProcessingResult { IsSuccess = true });

        var mockValidationRule = new Mock<IValidationRule>();
        mockValidationRule.Setup(x => x.RuleName).Returns("TestRule");
        mockValidationRule.Setup(x => x.Validate(It.IsAny<object>()))
            .Returns(new ValidationResult { IsValid = true });

        var mockFormatter = new Mock<IOutputFormatter>();
        mockFormatter.Setup(x => x.FormatType).Returns("TestFormat");
        mockFormatter.Setup(x => x.Format(It.IsAny<ContentPack>()))
            .Returns("Test output");

        // Act
        _extensionApi.RegisterTokenProvider("TestToken", mockTokenProvider.Object);
        _extensionApi.RegisterCustomPatchType("TestPatch", mockValidator.Object, mockProcessor.Object);
        _extensionApi.RegisterValidationRule("TestRule", mockValidationRule.Object);
        _extensionApi.RegisterOutputFormatter("TestFormat", mockFormatter.Object);

        var statistics = _extensionApi.GetStatistics();

        // Assert
        Assert.Equal(1, statistics.RegisteredTokenProviders);
        Assert.Equal(1, statistics.RegisteredPatchTypes);
        Assert.Equal(1, statistics.RegisteredValidationRules);
        Assert.Equal(1, statistics.RegisteredOutputFormatters);
    }

    #endregion

    #region Error Handling Tests

    /// <summary>
    /// 测试注册不存在的补丁类型时的错误处理
    /// </summary>
    [Fact]
    public void ErrorHandling_ValidateNonExistentPatchType_ShouldReturnError()
    {
        // Arrange
        var patchData = new Dictionary<string, object>
        {
            ["TestField"] = "TestValue"
        };

        // Act
        var validationResult = _extensionApi.ValidateCustomPatch("NonExistentPatch", patchData);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains("未找到补丁类型", validationResult.Errors.First());
    }

    /// <summary>
    /// 测试注册不存在的验证规则时的错误处理
    /// </summary>
    [Fact]
    public void ErrorHandling_ApplyNonExistentValidationRule_ShouldReturnError()
    {
        // Act
        var validationResult = _extensionApi.ApplyValidationRule("NonExistentRule", "test data");

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains("未找到验证规则", validationResult.Errors.First());
    }

    #endregion

    #region Integration Tests

    /// <summary>
    /// 测试扩展API与主服务的集成
    /// </summary>
    [Fact]
    public void Integration_ExtensionApiWithMainService_ShouldWork()
    {
        // Arrange
        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(x => x.TokenName).Returns("TestToken");
        mockTokenProvider.Setup(x => x.GetValue(It.IsAny<TokenContext>())).Returns("TestValue");
        mockTokenProvider.Setup(x => x.IsAvailable(It.IsAny<TokenContext>())).Returns(true);

        // Act
        _extensionApi.RegisterTokenProvider("TestToken", mockTokenProvider.Object);
        var statistics = _service.GetStatistics();

        // Assert
        Assert.Equal(1, statistics.ExtensionStatistics.RegisteredTokenProviders);
    }

    #endregion
}