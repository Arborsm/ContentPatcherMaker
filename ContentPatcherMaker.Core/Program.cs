using System.Reflection;
using ContentPatcherMaker.Core.Examples;
using ContentPatcherMaker.Core.Services;

namespace ContentPatcherMaker.Core;

public static class Program
{
    private static readonly string[] RelativeAssemblyProbePaths =
    {
        "", // app directory
        "smapi-public"
    };
    
    // 添加递归保护机制
    private static readonly HashSet<string> _resolvingAssemblies = new();
    
    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        var dataModelManagerExample = new DataModelManagerExample();
        dataModelManagerExample.RunAllExamplesAsync().Wait();
    }
    
    /// <summary>Method called when assembly resolution fails, which may return a manually resolved assembly.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private static Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs e)
    {
        // 防止递归调用
        if (_resolvingAssemblies.Contains(e.Name))
        {
            Console.WriteLine($"检测到递归程序集解析: {e.Name}");
            return null;
        }

        // get assembly name
        var name = new AssemblyName(e.Name);
        if (name.Name == null)
            return null;

        // 添加到正在解析的程序集集合
        _resolvingAssemblies.Add(e.Name);

        try
        {
            // check search folders
            foreach (string relativePath in RelativeAssemblyProbePaths)
            {
                // get absolute path of search folder
                var path = GameHelper.GamePath;
                if (string.IsNullOrEmpty(path))
                    path = Path.GetDirectoryName(Environment.ProcessPath) ?? string.Empty;
                string searchPath = Path.Combine(path, relativePath);
                if (!Directory.Exists(searchPath)) continue;

                if (GetAssembly(searchPath, name, out var currentDomainAssemblyResolve)) 
                {
                    return currentDomainAssemblyResolve;
                }
            }

            return null;
        }
        finally
        {
            // 从正在解析的程序集集合中移除
            _resolvingAssemblies.Remove(e.Name);
        }
    }

    private static bool GetAssembly(string searchPath, AssemblyName name, out Assembly? currentDomainAssemblyResolve)
    {
        currentDomainAssemblyResolve = null;
        
        // try to resolve DLL
        try
        {
            foreach (var dll in new DirectoryInfo(searchPath).EnumerateFiles("*.dll"))
            {
                // 使用反射获取程序集名称，避免触发程序集加载
                string? dllAssemblyName = GetAssemblyNameFromFile(dll.FullName);
                if (dllAssemblyName == null) continue;

                // check for match
                if (name.Name == null ||
                    !name.Name.Equals(dllAssemblyName, StringComparison.OrdinalIgnoreCase)) continue;
                
                // 安全地加载程序集
                try
                {
                    currentDomainAssemblyResolve = Assembly.LoadFrom(dll.FullName);
                    return true;
                }
                catch (Exception loadEx)
                {
                    Console.WriteLine($"加载程序集失败 {dll.FullName}: {loadEx.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error resolving assembly: {ex}");
            return false; // 修改：异常时返回false而不是true
        }

        return false;
    }

    /// <summary>
    /// 安全地获取程序集名称，避免触发程序集加载
    /// </summary>
    private static string? GetAssemblyNameFromFile(string filePath)
    {
        try
        {
            // 使用文件名作为程序集名称的简单匹配
            // 这是一个简化的方法，避免复杂的元数据读取
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            
            // 如果文件名看起来像程序集名称，直接返回
            if (!string.IsNullOrEmpty(fileName) && fileName.Length > 0)
            {
                return fileName;
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }
}