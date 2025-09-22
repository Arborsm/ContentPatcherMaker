using System.ComponentModel.DataAnnotations;

namespace ContentPatcherMaker.Core.DataModels;

/// <summary>
/// 验证结果
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// 验证是否通过
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// 错误列表
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// 警告列表
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// 返回验证结果的字符串表示
    /// </summary>
    /// <returns>验证结果字符串</returns>
    public override string ToString()
    {
        var result = $"验证结果: {(IsValid ? "通过" : "失败")}";
        if (Errors.Any())
        {
            result += $"\n错误: {string.Join(", ", Errors)}";
        }
        if (Warnings.Any())
        {
            result += $"\n警告: {string.Join(", ", Warnings)}";
        }
        return result;
    }
}
