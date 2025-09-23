using ContentPatcherMaker.Core.Examples;

namespace ContentPatcherMaker.Examples;

/// <summary>
/// ContentPatcherMaker示例程序
/// 演示如何从Stardew Valley游戏目录加载数据和解包XNB文件
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== ContentPatcherMaker 示例程序 ===\n");

        // 检查命令行参数
        if (args.Length > 0)
        {
            string command = args[0].ToLower();
            
            if (command == "unpack" && args.Length > 1)
            {
                // XNB解包模式
                string gamePath = args[1];
                string outputDir = args.Length > 2 ? args[2] : "unpacked_content";
                
                Console.WriteLine($"XNB解包模式 - 游戏路径: {gamePath}, 输出目录: {outputDir}");
                
                var unpackingExample = new XnbUnpackingExample();
                await unpackingExample.RunWithSpecifiedPathAsync(gamePath, outputDir);
            }
            else if (command == "data" && args.Length > 1)
            {
                // 数据加载模式（智能模式）
                string gamePath = args[1];
                Console.WriteLine($"数据加载模式 (智能模式) - 游戏路径: {gamePath}");
                
                var dataExample = new DataModelManagerExample();
                await dataExample.RunWithSpecifiedPathAsync(gamePath);
            }
            else if (command == "auto")
            {
                // 自动检测模式（智能模式）
                Console.WriteLine("自动检测模式 (智能模式)");
                
                var dataExample = new DataModelManagerExample();
                await dataExample.RunAllExamplesAsync();
            }
            else
            {
                // 使用指定的游戏路径（默认智能模式）
                string gamePath = args[0];
                Console.WriteLine($"使用指定的游戏路径 (智能模式): {gamePath}");
                
                var dataExample = new DataModelManagerExample();
                await dataExample.RunWithSpecifiedPathAsync(gamePath);
            }
        }
        else
        {
            // 显示帮助信息
            ShowHelp();
        }

        Console.WriteLine("\n按任意键退出...");
        Console.ReadKey();
    }

    static void ShowHelp()
    {
        Console.WriteLine("ContentPatcherMaker 示例程序");
        Console.WriteLine();
        Console.WriteLine("用法:");
        Console.WriteLine("  ContentPatcherMaker.Examples [命令] [参数]");
        Console.WriteLine();
        Console.WriteLine("命令:");
        Console.WriteLine("  data <游戏路径>            - 数据加载模式（智能模式）");
        Console.WriteLine("  auto                       - 自动检测模式（智能模式）");
        Console.WriteLine("  unpack <游戏路径> [输出目录] - XNB解包模式");
        Console.WriteLine("  <游戏路径>                 - 使用指定游戏路径运行数据加载示例（智能模式）");
        Console.WriteLine();
        Console.WriteLine("示例:");
        Console.WriteLine("  ContentPatcherMaker.Examples data \"C:\\Program Files\\Steam\\steamapps\\common\\Stardew Valley\"");
        Console.WriteLine("  ContentPatcherMaker.Examples auto");
        Console.WriteLine("  ContentPatcherMaker.Examples unpack \"C:\\Program Files\\Steam\\steamapps\\common\\Stardew Valley\" unpacked");
        Console.WriteLine("  ContentPatcherMaker.Examples \"C:\\Program Files\\Steam\\steamapps\\common\\Stardew Valley\"");
        Console.WriteLine();
        Console.WriteLine("模式说明:");
        Console.WriteLine("  智能模式 - 自动判断使用JSON或XNB模式，无需手动选择");
        Console.WriteLine();
        Console.WriteLine("注意:");
        Console.WriteLine("  - 游戏路径应该是Stardew Valley的安装目录");
        Console.WriteLine("  - 输出目录是可选的，默认为当前目录下的unpacked_content文件夹");
        Console.WriteLine("  - 程序会自动检测Content文件夹的位置");
        Console.WriteLine("  - XNB模式直接从游戏目录加载，JSON模式需要解包后的文件");
    }
}
