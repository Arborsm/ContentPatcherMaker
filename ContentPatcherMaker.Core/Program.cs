using ContentPatcherMaker.Core.Examples;

namespace ContentPatcherMaker.Core;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== ContentPatcherMaker Core 示例程序 ===");
        Console.WriteLine();

        try
        {
            // 运行ContentPatcherMakerCore示例
            Console.WriteLine("运行ContentPatcherMakerCore示例...");
            var coreExample = new ContentPatcherMakerCoreExample();
            await coreExample.RunExampleAsync();
            Console.WriteLine("ContentPatcherMakerCore示例完成");
            Console.WriteLine();

            Console.WriteLine("所有示例运行完成！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"示例运行失败: {ex.Message}");
            Console.WriteLine($"详细信息: {ex}");
        }

        Console.WriteLine();
        Console.WriteLine("按任意键退出...");
        Console.ReadKey();
    }
}