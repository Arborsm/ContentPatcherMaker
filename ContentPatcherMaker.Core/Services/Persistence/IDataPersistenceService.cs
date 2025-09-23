namespace ContentPatcherMaker.Core.Services.Persistence;

/// <summary>
/// 数据持久化服务接口
/// </summary>
public interface IDataPersistenceService
{
    /// <summary>
    /// 保存数据到文件
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="data">要保存的数据</param>
    /// <param name="filePath">文件路径</param>
    Task SaveAsync<T>(T data, string filePath);

    /// <summary>
    /// 从文件加载数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="filePath">文件路径</param>
    /// <returns>加载的数据</returns>
    Task<T?> LoadAsync<T>(string filePath);

    /// <summary>
    /// 检查文件是否存在
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>是否存在</returns>
    bool Exists(string filePath);

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    Task DeleteAsync(string filePath);

    /// <summary>
    /// 获取文件信息
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>文件信息</returns>
    FileInfo? GetFileInfo(string filePath);

    /// <summary>
    /// 备份文件
    /// </summary>
    /// <param name="filePath">原文件路径</param>
    /// <param name="backupPath">备份路径</param>
    Task BackupAsync(string filePath, string backupPath);

    /// <summary>
    /// 恢复文件
    /// </summary>
    /// <param name="backupPath">备份路径</param>
    /// <param name="filePath">目标文件路径</param>
    Task RestoreAsync(string backupPath, string filePath);
}
