using System.Reflection;
using Xunit;
using Xunit.Abstractions;
// ReSharper disable All

namespace ContentPatcherMaker.Test;

/// <summary>
/// 测试运行器，用于执行所有基于md文档的测试
/// </summary>
public class TestRunner
{
    private readonly ITestOutputHelper _output;

    public TestRunner(ITestOutputHelper output)
    {
        _output = output;
    }

    /// <summary>
    /// 运行所有测试
    /// </summary>
    [Fact]
    public void RunAllDocumentationTests()
    {
        _output.WriteLine("=== ContentPatcher 文档测试运行器 ===");
        _output.WriteLine($"开始时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        _output.WriteLine("");

        var testResults = new List<TestResult>();

        // 运行所有测试类
        var testClasses = GetTestClasses();
        foreach (var testClass in testClasses)
        {
            _output.WriteLine($"运行测试类: {testClass.Name}");
            var classResults = RunTestClass(testClass);
            testResults.AddRange(classResults);
            _output.WriteLine($"  完成 {testClass.Name}: {classResults.Count} 个测试");
            _output.WriteLine("");
        }

        // 输出总结
        OutputSummary(testResults);
    }

    /// <summary>
    /// 获取所有测试类
    /// </summary>
    private List<Type> GetTestClasses()
    {
        var assembly = Assembly.GetExecutingAssembly();
        return assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<CollectionAttribute>() != null || 
                        t.GetMethods().Any(m => m.GetCustomAttribute<FactAttribute>() != null || 
                                                m.GetCustomAttribute<TheoryAttribute>() != null))
            .Where(t => t.Namespace == "ContentPatcherMaker.Test")
            .ToList();
    }

    /// <summary>
    /// 运行测试类中的所有测试
    /// </summary>
    private List<TestResult> RunTestClass(Type testClass)
    {
        var results = new List<TestResult>();
        var testMethods = testClass.GetMethods()
            .Where(m => m.GetCustomAttribute<FactAttribute>() != null || 
                        m.GetCustomAttribute<TheoryAttribute>() != null)
            .ToList();

        foreach (var method in testMethods)
        {
            var result = RunTestMethod(testClass, method);
            results.Add(result);
        }

        return results;
    }

    /// <summary>
    /// 运行单个测试方法
    /// </summary>
    private TestResult RunTestMethod(Type testClass, MethodInfo method)
    {
        var result = new TestResult
        {
            ClassName = testClass.Name,
            MethodName = method.Name,
            StartTime = DateTime.Now
        };

        try
        {
            // 创建测试实例
            var instance = Activator.CreateInstance(testClass);
                
            // 运行测试方法
            method.Invoke(instance, null);
                
            result.Status = TestStatus.Passed;
            result.Message = "测试通过";
        }
        catch (Exception ex)
        {
            result.Status = TestStatus.Failed;
            result.Message = ex.Message;
            result.Exception = ex;
        }
        finally
        {
            result.EndTime = DateTime.Now;
            result.Duration = result.EndTime - result.StartTime;
        }

        return result;
    }

    /// <summary>
    /// 输出测试总结
    /// </summary>
    private void OutputSummary(List<TestResult> results)
    {
        _output.WriteLine("=== 测试总结 ===");
        _output.WriteLine($"总测试数: {results.Count}");
        _output.WriteLine($"通过: {results.Count(r => r.Status == TestStatus.Passed)}");
        _output.WriteLine($"失败: {results.Count(r => r.Status == TestStatus.Failed)}");
        _output.WriteLine($"总耗时: {results.Sum(r => r.Duration.TotalMilliseconds):F2} ms");
        _output.WriteLine("");

        // 输出失败的测试
        var failedTests = results.Where(r => r.Status == TestStatus.Failed).ToList();
        if (failedTests.Any())
        {
            _output.WriteLine("=== 失败的测试 ===");
            foreach (var failedTest in failedTests)
            {
                _output.WriteLine($"❌ {failedTest.ClassName}.{failedTest.MethodName}");
                _output.WriteLine($"   错误: {failedTest.Message}");
                if (failedTest.Exception != null)
                {
                    _output.WriteLine($"   异常: {failedTest.Exception}");
                }
                _output.WriteLine("");
            }
        }

        // 输出通过的测试
        var passedTests = results.Where(r => r.Status == TestStatus.Passed).ToList();
        if (passedTests.Any())
        {
            _output.WriteLine("=== 通过的测试 ===");
            foreach (var passedTest in passedTests)
            {
                _output.WriteLine($"✅ {passedTest.ClassName}.{passedTest.MethodName} ({passedTest.Duration.TotalMilliseconds:F2} ms)");
            }
        }

        _output.WriteLine("");
        _output.WriteLine($"结束时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
    }

    /// <summary>
    /// 测试结果
    /// </summary>
    private class TestResult
    {
        public string ClassName { get; set; } = string.Empty;
        public string MethodName { get; set; } = string.Empty;
        public TestStatus Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public Exception? Exception { get; set; }
        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
    }

    /// <summary>
    /// 测试状态
    /// </summary>
    private enum TestStatus
    {
        Passed,
        Failed
    }
}

/// <summary>
/// 基于md文档的测试集合
/// </summary>
[Collection("DocumentationTests")]
public class DocumentationTestCollection
{
    /// <summary>
    /// 验证所有md文档都有对应的测试
    /// </summary>
    [Fact]
    public void VerifyAllDocumentationHasTests()
    {
        // 这个测试确保我们为所有重要的md文档创建了测试
        var testClasses = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.Namespace == "ContentPatcherMaker.Test")
            .Where(t => t.Name.EndsWith("Tests"))
            .ToList();

        Assert.True(testClasses.Count >= 4, "应该有至少4个测试类");
            
        var expectedTestClasses = new[]
        {
            "ContentPatcherDocumentationTests",
            "ContentPatcherActionTests", 
            "ContentPatcherExtensionTests",
            "ContentPatcherConditionsTests"
        };

        foreach (var expectedClass in expectedTestClasses)
        {
            Assert.True(testClasses.Any(t => t.Name == expectedClass), 
                $"缺少测试类: {expectedClass}");
        }
    }

    /// <summary>
    /// 验证测试覆盖了所有主要的ContentPatcher功能
    /// </summary>
    [Fact]
    public void VerifyTestCoverage()
    {
        var testMethods = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.Namespace == "ContentPatcherMaker.Test")
            .SelectMany(t => t.GetMethods())
            .Where(m => m.GetCustomAttribute<FactAttribute>() != null || 
                        m.GetCustomAttribute<TheoryAttribute>() != null)
            .ToList();

        // 验证覆盖了所有主要操作
        var actionTests = testMethods.Where(m => m.Name.Contains("Action")).ToList();
        Assert.True(actionTests.Count >= 5, "应该测试所有5种主要操作类型");

        // 验证覆盖了扩展功能
        var extensionTests = testMethods.Where(m => m.Name.Contains("Extension")).ToList();
        Assert.True(extensionTests.Count >= 3, "应该测试扩展API的主要功能");

        // 验证覆盖了条件功能
        var conditionTests = testMethods.Where(m => m.Name.Contains("Condition")).ToList();
        Assert.True(conditionTests.Count >= 3, "应该测试条件API的主要功能");
    }
}