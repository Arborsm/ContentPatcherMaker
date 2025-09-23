using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using ContentPatcherMaker.Core.Services.Logging;
using Microsoft.Win32;

namespace ContentPatcherMaker.Core.Services;

public static class GameHelper
{
    private static string? _gamePath;

    public static string GamePath
    {
        get
        {
            if (_gamePath == null)
            {
                foreach (string candidate in GetCandidateGamePaths())
                {
                    if (TryGamePath(candidate) != null)
                    {
                        _gamePath = candidate;
                    }
                }
            }
            return _gamePath ?? string.Empty;
        }
    }

    /// <summary>
    /// 检查路径是否为有效的游戏路径
    /// </summary>
    private static string? TryGamePath(string? path)
    {
        if (path == null) return null;
        
        var gameDir = new DirectoryInfo(path);
        if (!gameDir.Exists) return null;

        // 检查是否有游戏文件
        bool hasGameDll = File.Exists(Path.Combine(gameDir.FullName, "Stardew Valley.dll"));
        if (!hasGameDll) return null;

        // 不是编译文件夹
        bool isCompileFolder = File.Exists(Path.Combine(gameDir.FullName, "StardewXnbHack.exe.config"));
        if (isCompileFolder) return null;

        return gameDir.FullName;
    }
    
    /// <summary>
    /// 获取可能的游戏路径
    /// </summary>
    private static IEnumerable<string> GetCandidateGamePaths(string? specifiedPath = null)
    {
        // 用户指定的路径
        if (!string.IsNullOrWhiteSpace(specifiedPath))
            yield return specifiedPath;

        // 当前工作目录
        yield return AppDomain.CurrentDomain.BaseDirectory;
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            const string steamAppId = "413150";
            IDictionary<string, string> registryKeys = new Dictionary<string, string>
            {
                [@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App " + steamAppId] = "InstallLocation",
                [@"SOFTWARE\WOW6432Node\GOG.com\Games\1453375253"] = "PATH"
            };
            foreach (var pair in registryKeys)
            {
                var path = GetLocalMachineRegistryValue(pair.Key, pair.Value);
                if (!string.IsNullOrWhiteSpace(path))
                    yield return path;
            }

            var steamPath = GetCurrentUserRegistryValue(@"Software\Valve\Steam", "SteamPath");
            if (steamPath != null)
                yield return Path.Combine(steamPath.Replace('/', '\\'), @"steamapps\common\Stardew Valley");

            foreach (var programFiles in new[] { @"C:\Program Files", @"C:\Program Files (x86)" })
            {
                yield return $@"{programFiles}\GalaxyClient\Games\Stardew Valley";
                yield return $@"{programFiles}\GOG Galaxy\Games\Stardew Valley";
                yield return $@"{programFiles}\GOG Games\Stardew Valley";
                yield return $@"{programFiles}\Steam\steamapps\common\Stardew Valley";
            }

            for (var driveLetter = 'C'; driveLetter <= 'H'; driveLetter++)
                yield return $@"{driveLetter}:\Program Files\ModifiableWindowsApps\Stardew Valley";
        }

        yield return Environment.CurrentDirectory;
    }

    [SupportedOSPlatform("windows")]
    private static string? GetLocalMachineRegistryValue(string key, string name)
    {
        var localMachine = Environment.Is64BitOperatingSystem
            ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
            : Registry.LocalMachine;
        var openKey = localMachine.OpenSubKey(key);
        if (openKey == null)
            return null;
        using (openKey)
        {
            return (string?)openKey.GetValue(name);
        }
    }

    [SupportedOSPlatform("windows")]
    private static string? GetCurrentUserRegistryValue(string key, string name)
    {
        var currentUser = Environment.Is64BitOperatingSystem
            ? RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)
            : Registry.CurrentUser;
        var openKey = currentUser.OpenSubKey(key);
        if (openKey == null)
            return null;
        using (openKey)
        {
            return (string?)openKey.GetValue(name);
        }
    }
}

/// <summary>
/// 游戏路径检测服务
/// 基于StardewXnbHack的PlatformContext实现
/// </summary>
public class GamePathDetectionService
{
    private readonly LoggingService _loggingService;

    /// <summary>
    /// 初始化游戏路径检测服务
    /// </summary>
    /// <param name="loggingService">日志服务</param>
    public GamePathDetectionService(LoggingService loggingService)
    {
        _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
    }

    /// <summary>
    /// 检测游戏路径和内容路径
    /// </summary>
    /// <param name="specifiedPath">用户指定的游戏路径，如果为null则自动检测</param>
    /// <returns>游戏路径检测结果</returns>
    public GamePathDetectionResult DetectGamePaths(string? specifiedPath = null)
    {
        try
        {
            _loggingService.LogInformation("开始检测Stardew Valley游戏路径", "GamePathDetectionService");

            var result = new GamePathDetectionResult();
            var platform = DetectPlatform();

            // 检测路径
            var curGamePath = GameHelper.GamePath;
            if (string.IsNullOrEmpty(curGamePath)) throw new Exception("未找到游戏目录");
            string? curContentPath = FindContentPath(curGamePath, platform);

            // 找到有效的游戏安装
            if (curGamePath != null && curContentPath != null)
            {
                result.GamePath = curGamePath;
                result.ContentPath = curContentPath;
                result.IsSuccess = true;
                result.Platform = platform;
                    
                _loggingService.LogInformation($"成功检测到游戏路径: {curGamePath}", "GamePathDetectionService");
                _loggingService.LogInformation($"内容路径: {curContentPath}", "GamePathDetectionService");
                return result;
            }

            // 如果游戏文件夹存在但没有内容文件夹，记录第一个找到的游戏路径
            result.GamePath ??= curGamePath;

            result.IsSuccess = false;
            result.ErrorMessage = result.GamePath == null
                ? "无法找到Stardew Valley文件夹。" : $"在 {result.GamePath} 找不到内容文件夹。游戏安装是否正确？";

            _loggingService.LogError(result.ErrorMessage, context: "GamePathDetectionService");
            return result;
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"检测游戏路径时发生异常: {ex.Message}", ex, "GamePathDetectionService");
            return new GamePathDetectionResult
            {
                IsSuccess = false,
                ErrorMessage = $"检测游戏路径时发生异常: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// 检测当前平台
    /// </summary>
    private static Platform DetectPlatform()
    {
        bool isWindows = Environment.OSVersion.Platform != PlatformID.MacOSX && 
                        Environment.OSVersion.Platform != PlatformID.Unix;
        bool isMac = Environment.OSVersion.Platform == PlatformID.MacOSX;
        
        if (isWindows) return Platform.Windows;
        if (isMac) return Platform.Mac;
        return Platform.Linux;
    }

    /// <summary>
    /// 查找内容路径
    /// </summary>
    private static string? FindContentPath(string? gamePath, Platform platform)
    {
        if (gamePath == null) return null;

        foreach (string relativePath in GetPossibleRelativeContentPaths(platform))
        {
            var folder = new DirectoryInfo(Path.Combine(gamePath, relativePath));
            if (folder.Exists)
                return folder.FullName;
        }

        return null;
    }

    /// <summary>
    /// 获取可能的内容路径
    /// </summary>
    private static IEnumerable<string> GetPossibleRelativeContentPaths(Platform platform)
    {
        // 大多数平台下在游戏文件夹内
        if (platform != Platform.Mac)
            yield return "Content";

        // macOS
        else
        {
            // Steam路径
            // - 游戏路径: StardewValley/Contents/MacOS
            // - 内容:   StardewValley/Contents/Resources/Content
            yield return "../Resources/Content";

            // GOG路径
            // - 游戏路径: Stardew Valley.app/Contents/MacOS
            // - 内容:   Stardew Valley.app/Resources/Content
            yield return "../../Resources/Content";
        }
    }
}

/// <summary>
/// 平台类型
/// </summary>
public enum Platform
{
    /// <summary>
    /// Windows平台
    /// </summary>
    Windows,
    
    /// <summary>
    /// Linux平台
    /// </summary>
    Linux,
    
    /// <summary>
    /// macOS平台
    /// </summary>
    Mac
}

/// <summary>
/// 游戏路径检测结果
/// </summary>
public class GamePathDetectionResult
{
    /// <summary>
    /// 是否成功检测到路径
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 游戏路径
    /// </summary>
    public string? GamePath { get; set; }

    /// <summary>
    /// 内容路径
    /// </summary>
    public string? ContentPath { get; set; }

    /// <summary>
    /// 检测到的平台
    /// </summary>
    public Platform Platform { get; set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 验证路径是否有效
    /// </summary>
    public bool ValidatePaths()
    {
        if (!IsSuccess) return false;
        if (string.IsNullOrEmpty(GamePath) || string.IsNullOrEmpty(ContentPath)) return false;
        
        return Directory.Exists(GamePath) && Directory.Exists(ContentPath);
    }
}
