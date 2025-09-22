namespace ContentPatcherMaker.Services.Abstractions;

public interface IAppLogger
{
    void Info(string message);
    void Warn(string message);
    void Error(string message, System.Exception? ex = null);
}

