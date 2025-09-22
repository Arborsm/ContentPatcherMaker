using System;
using ContentPatcherMaker.Services.Abstractions;

namespace ContentPatcherMaker.Services.Logging;

public sealed class SimpleConsoleLogger : IAppLogger
{
    public void Info(string message) => Console.WriteLine($"[INFO] {message}");
    public void Warn(string message) => Console.WriteLine($"[WARN] {message}");
    public void Error(string message, Exception? ex = null)
    {
        Console.WriteLine($"[ERROR] {message}");
        if (ex != null)
        {
            Console.WriteLine(ex);
        }
    }
}

